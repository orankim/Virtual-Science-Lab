using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rotationlock : MonoBehaviour
{
    Quaternion defaultRotation;
    void Awake()
    {
        defaultRotation = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = defaultRotation;
        transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
    }    
    // Start is called before the first frame update
    public Text status;
    void Start()
    {
        status.text="Start";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
