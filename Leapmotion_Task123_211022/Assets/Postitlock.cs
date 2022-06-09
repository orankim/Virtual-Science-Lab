using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Postitlock : MonoBehaviour
{

    Quaternion defaultRotation;

    void Awake()
    {
        defaultRotation = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = defaultRotation;
        transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
