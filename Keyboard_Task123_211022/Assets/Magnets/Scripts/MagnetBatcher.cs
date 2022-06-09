using UnityEngine;
using System.Collections;

public class MagnetBatcher : MonoBehaviour {
	
	public BarMagnet 	driverMagnet;
	
	private bool		isGravitySimul = false;
	private float	    hitDist = 49.0f;
	
	// Use this for initialization
	void Start () {
		driverMagnet = null;
		
		if (Application.loadedLevelName == "FerromagneticMatter3") {
			isGravitySimul = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown("Fire1")) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			float castDist = 100.0f;	
			if (isGravitySimul) {
				castDist = 1000.0f;				
			}
						
			if (Physics.Raycast(ray, out hitInfo, castDist)) {
				
				if (isGravitySimul) {
					hitDist = hitInfo.distance;
				}
				
				if (hitInfo.collider.transform.parent) {
					driverMagnet = hitInfo.collider.transform.parent.GetComponent("BarMagnet") as BarMagnet;
					if (driverMagnet && !driverMagnet.GetDriverSetting()) {
						driverMagnet = null;
					}
				}
			}
		}
		
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (driverMagnet) {
				driverMagnet.transform.Rotate(Vector3.forward, 90, Space.Self);
			}
		}
		
		if (driverMagnet && Input.GetMouseButton(0)) {
			
			Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, hitDist));
			
			if (isGravitySimul) {
				float x = driverMagnet.transform.position.x;
				float y = pos.y;
				float z = driverMagnet.transform.position.z;					
								
				driverMagnet.transform.position = new Vector3(x, y, z);
								
			} else {				
				pos.y = 1;
				driverMagnet.transform.position = pos;			
			}
		}
		
		if (Input.GetButtonUp("Fire1")) {
			if (driverMagnet) {
				driverMagnet = null;
			}
		}
		
	}
}
