using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Moveonlyoneway : MonoBehaviour
{
    Quaternion defaultRotation;

    void Awake()
    {
        defaultRotation = transform.rotation;
    }
    void LateUpdate()
    {
        transform.rotation = defaultRotation;
        transform.position = new Vector3(transform.position.x, 1.5f, 0f);
    }

    public Text status;
    void Start()
    {
        status.text="Start";
        
    }
}
