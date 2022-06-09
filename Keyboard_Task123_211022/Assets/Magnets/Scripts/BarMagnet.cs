using UnityEngine;
using System.Collections;

public class BarMagnet : MonoBehaviour {
	
	public float dipoleMoment = 5.0f;		
	public bool driver = false;
	
	private bool collide = false;	
	private int indexKey = 0;
	private Rigidbody rb;
	private BarMagnet colBm;
	
	public Rigidbody RB 
	{
		get { return rb; }
		set { rb = value; }
	}		
	
	void Start () {
		indexKey = MagneticSystem.Instance.AddMagnet(this);
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		if (colBm) {
			float checkDist = (colBm.transform.position - transform.position).sqrMagnitude;
			if (checkDist > 100.0f) {
				collide = false;
				colBm = null;
			}			
		}
	}
			
	void FixedUpdate () {		
		if (!driver && !collide) {
			Vector3 mforce = MagneticSystem.Instance.CalcForceToMagnet(indexKey);
			Vector3 mTorque = MagneticSystem.Instance.CalcTorqueToMagnet(indexKey);
			if (rb) {
				rb.AddForce(mforce);
				rb.AddTorque(mTorque);
				//Debug.Log(mforce + "and" + mTorque);
			}
		}
	}
	
	void OnCollisionEnter(Collision theObject) {
		
		Transform p = theObject.transform;
		
		if (p) {
			BarMagnet bm = p.GetComponent("BarMagnet") as BarMagnet;
			if (bm) {
				colBm = bm;
				collide = true;
				rb.Sleep();
			}
		}
	}
	
	public Vector3 GetMagneticMoment () {
		Transform nt = transform.Find("NorthPole");
		Transform st = transform.Find("SouthPole");
		Vector3 dir = nt.position - st.position;
				
		return dipoleMoment * dir.normalized;
	}
	
	public Vector3 GetClosestEdgePosition (Vector3 pos) {
		float NDist = (GetNorthEdgePosition() - pos).sqrMagnitude;
		float SDist = (GetSouthEdgePosition() - pos).sqrMagnitude;
		
		if (NDist <= SDist)
			return GetNorthEdgePosition();
		
		return GetSouthEdgePosition();
	}
	
	private Vector3 GetNorthEdgePosition () {
		Transform nt = transform.Find("NorthPole");
		Vector3 dir = nt.position - transform.position;
		
		return transform.position + 2.0f * dir;
	}
	
	private Vector3 GetSouthEdgePosition () {
		Transform st = transform.Find("SouthPole");
		Vector3 dir = st.position - transform.position;
		
		return transform.position + 2.0f * dir;	
	}
	
	public bool GetDriverSetting () {
		return driver;
	}
	
	public bool IsCollide () {
		return collide;
	}
		
	public int GetIndexKey () {
		return indexKey;
	}
}
