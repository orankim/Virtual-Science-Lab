using UnityEngine;
using System.Collections;

public class FerroMagnet : MonoBehaviour {
	
	public float permiability = 700.0f;
	
	//private bool collide = false;
	
	private Rigidbody rb;
	private float inverseU0;
	private float radius;
	
	public float Radius 
	{
		get { return radius; }
		set { radius = value; }
	}		
	
	public class MagnetInfluence {
		
		private BarMagnet bm;
				
		public BarMagnet Bar
		{
			get { return bm; }
			set { bm = value; }
		}
 		
		public Vector3 ma = new Vector3();
		
		public Vector3 Magnetization
		{
			get { return ma;  }
			set { ma = value; }
		}
		
		private FerroMagnet fm;
		
		public FerroMagnet FerroSphere
		{
			get { return fm; }
			set { fm = value; }
		}
		
		public bool attached = false;
		
		public MagnetInfluence(BarMagnet bar, FerroMagnet fs, Vector3 magnetiztion) {
			bm = bar;
			fm = fs;
			ma = magnetiztion;			
		}		
	}
	
	private class MICollection : IEnumerable {
		private ArrayList arMI = new ArrayList();
		
		public MagnetInfluence this[int index] {
			get { return (MagnetInfluence)arMI[index]; }
			set { arMI.Insert(index, value); }
		}
		
		public void AddMagnet(MagnetInfluence m) {
			arMI.Add(m);
		}
		
		public int Count {
			get { return arMI.Count; }
		}
					
		IEnumerator IEnumerable.GetEnumerator()
		{ return arMI.GetEnumerator(); }
	}
	
	private MICollection magnetInfluences = new MICollection();
			
	void Start () {
		rb = GetComponent<Rigidbody>();
		SphereCollider spc = GetComponent<SphereCollider>();
		if (spc) {
			radius = spc.radius * transform.localScale.x;
		} else {
			radius = 0.0f;
		}
		inverseU0 = 1.0f / (MagneticSystem.Instance.GetFieldModifier() * 4 * Mathf.PI);
	}
		
	void FixedUpdate () {
				
		Vector3 ma = ComputeInducedMagneticMoment();						
		Vector3 mforce = MagneticSystem.Instance.CalcForceToFerro(ma, transform.position, this);
		
		//Debug.Log( "Test" + ((ma.x > 0) ? 1 : -1) );
		// 구 모형이라 한정 짓고 토크 생략
		if (rb) {						
			rb.AddForce(mforce);
			
			/*
			if (this.name == "Sphere") {
				Debug.Log( mforce + "and" + ma + this.name );
			}
			*/			
		}			
	}
	
	void OnCollisionEnter(Collision theObject) {
		
		Transform p = theObject.transform;
		
		if (p) {
			BarMagnet bm = p.GetComponent("BarMagnet") as BarMagnet;
			FerroMagnet fm = p.GetComponent("FerroMagnet") as FerroMagnet;
			
			if (!bm && p.transform.parent) {
				bm = p.transform.parent.GetComponent("BarMagnet") as BarMagnet;
			}
			
			if (bm) {
							
				rb.Sleep();				
				bm.RB.Sleep();
				FixedJoint fj0 = rb.gameObject.AddComponent<FixedJoint>() as FixedJoint;
				fj0.connectedBody = bm.RB;				
				fj0.anchor = theObject.contacts[0].point;
				
				FixedJoint fj1 = bm.RB.gameObject.AddComponent<FixedJoint>() as FixedJoint;
				fj1.connectedBody = rb;				
				fj1.anchor = theObject.contacts[0].point;
				
				for (int i = 0; i < magnetInfluences.Count; ++i) {
					MagnetInfluence mi = magnetInfluences[i];					
					if (mi.Bar && mi.Bar.Equals(bm)) {
						mi.attached = true;
						mi.Magnetization = ComputeInducedMagneticMoment(bm, null);
						
						//Debug.Log("Last" + mi.Magnetization);
						
						MagneticSystem.Instance.AddFerroMagnet(this);
					}
				}				
			} else if (fm) {
				
				rb.Sleep();
				//fm.rb.Sleep();
				
				/*
				for (int i = 0; i < magnetInfluences.Count; ++i) {
					MagnetInfluence mi = magnetInfluences[i];					
					if (mi.FerroSphere) {
						mi.attached = true;
						mi.Magnetization = ComputeInducedMagneticMoment(null, fm);						
					}
				}
				*/						
				
				/*
				FixedJoint fj0 = rb.gameObject.AddComponent("FixedJoint") as FixedJoint;
				fj0.connectedBody = fm.rb;
				fj0.anchor = theObject.contacts[0].point;
				
				FixedJoint fj1 = fm.rb.gameObject.AddComponent("FixedJoint") as FixedJoint;
				fj1.connectedBody = rb;
				fj1.anchor = theObject.contacts[0].point;								
				*/
			}
		}
	}
	
	private Vector3 ComputeInducedMagneticMoment() {
		
		// 0은 기본으로 막대자석이라 가정한다.
		BarMagnet inflBar = magnetInfluences.Count > 0 ? magnetInfluences[0].Bar : null;
				
		if (!inflBar) {						
			return new Vector3(0.0f, 0.0f, 0.0f);
		}
				
		Vector3 dir = inflBar.transform.position - transform.position;
		float dist = dir.sqrMagnitude;
				
		if (magnetInfluences[0].attached) {
			dist = float.MaxValue;
		}
	
		FerroMagnet inflFm = null;
		
		for (int i = 1; i < magnetInfluences.Count; ++i) {
			MagnetInfluence mi = magnetInfluences[i];
			dir = mi.Bar ? (mi.Bar.transform.position - transform.position) : (mi.FerroSphere.transform.position - transform.position);
			if (!mi.attached && dir.sqrMagnitude < dist) {
				dist = dir.sqrMagnitude;
				inflBar = mi.Bar ? mi.Bar : null;
				inflFm  = mi.FerroSphere ? mi.FerroSphere : null;
			}
		}
				
		Vector3 cmm = ComputeInducedMagneticMoment(inflBar, inflFm);
		Vector3 imm = GetInducedMagneticMoment();
	
		BarMagnet attachedBar = null;
		float maxDipole = 0.0f;
		
		for (int i = 0; i < magnetInfluences.Count; ++i) {
			MagnetInfluence mi = magnetInfluences[i];
			if (mi.Bar && mi.attached && (maxDipole < mi.Bar.dipoleMoment)) {
				attachedBar = mi.Bar;
			}
		}						
				
		if (inflBar && attachedBar && dist > 50.0f) {
			//Debug.Log (imm);
			return imm;
		}
		
		//Debug.Log(cmm);
		
		return cmm;
		
		/*
		if (imm.sqrMagnitude < cmm.sqrMagnitude) {
			Debug.Log ("cmm");
			return cmm;
		}
		
		BarMagnet attachedBar = null;
		float maxDipole = 0.0f;
		
		for (int i = 0; i < magnetInfluences.Count; ++i) {
			MagnetInfluence mi = magnetInfluences[i];
			if (mi.attached && (maxDipole < mi.Bar.dipoleMoment)) {
				attachedBar = mi.Bar;
			}
		}		
		
		if (attachedBar && dist < 50.0f)	{	
			Debug.Log(dist + "imm" + imm.sqrMagnitude);
			return imm * 10.0f;
			//return ComputeInducedMagneticMoment(attachedBar, inflFm);
		}
		
		Debug.Log("null");
		return new Vector3(0.0f, 0.0f, 0.0f);		
		*/
	}
	
	private Vector3 ComputeInducedMagneticMoment(BarMagnet bm, FerroMagnet fm) {
				
		if (radius < 0.1f) {
			return new Vector3(0.0f, 0.0f, 0.0f);
		}
				
		float volume = 1.333f * Mathf.PI * radius * radius * radius;
		float calcScale = 3 * permiability / (3 + permiability);
				
		Vector3 magnetizeVal = new Vector3(0.0f, 0.0f, 0.0f);
		
		if (bm) {
			Vector3 bmEdgePos = bm.GetClosestEdgePosition(transform.position);
			Vector3 spEdgePos = GetClosestEdgePosition(bmEdgePos);		
			magnetizeVal = calcScale * inverseU0 * MagneticSystem.Instance.CalcMField(spEdgePos, bm);
			//Debug.Log("bm" + magnetizeVal);
		} else {
			Vector3 spEdgePosFM = fm.GetClosestEdgePosition(transform.position);
			Vector3 spEdgePos = GetClosestEdgePosition(spEdgePosFM);
			magnetizeVal = calcScale * inverseU0 * MagneticSystem.Instance.CalcMField(spEdgePos, fm);
			//Debug.Log("fm" + magnetizeVal);
		}
		
		return volume * magnetizeVal;	
	}
	
	public Vector3 GetClosestEdgePosition (Vector3 pos) {

		Vector3 dir = pos - transform.position;					
		return transform.position + radius * dir.normalized;
	}
	
	// 자석에 붙은 경우만 고려한다.
	public Vector3 GetInducedMagneticMoment () {
		
		Vector3 mm = new Vector3(0.0f, 0.0f, 0.0f);
		
		for (int i = 0; i < magnetInfluences.Count; ++i) {
			MagnetInfluence mi = magnetInfluences[i];
			
			if (mi.attached) {								
				mm += transform.localScale.x * mi.Magnetization;				
			}
		}
		
		return mm;
	}
	
	public bool IsAttachedBar(BarMagnet bm) {
		
		bool attached = false;
		for (int i = 0; i < magnetInfluences.Count; ++i) {
			MagnetInfluence mi = magnetInfluences[i];
			
			if (mi.Bar && mi.Bar.Equals(bm) && mi.attached) {
				attached = true;
				break;
			}
		}		
		
		return attached;
	}
		
	/*
	public bool IsCollide () {
		return collide;
	}
	
	public void CollideReset () {
		collide = false;
	}
	*/
	
	public void AddMagnet (BarMagnet bm) {
		magnetInfluences.AddMagnet(new MagnetInfluence(bm, null, ComputeInducedMagneticMoment(bm, null)));
	}
	
	public void AddMagnet (FerroMagnet fm) {
		if (!fm.Equals(this)) {
			magnetInfluences.AddMagnet(new MagnetInfluence(null, fm, ComputeInducedMagneticMoment(null, fm)));
		}
	}
}
