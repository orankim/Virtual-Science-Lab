using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positionlock : MonoBehaviour
{
    Quaternion defaultRotation;

    void Awake()
    {
        defaultRotation = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = defaultRotation;
        transform.position = new Vector3(0.8f, 1.5f, -5f);
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
