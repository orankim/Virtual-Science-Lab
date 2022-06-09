using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move_onlyoneway : MonoBehaviour
{
    float move = 0.1f;
    bool isAlive = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) transform.position = transform.position + new Vector3(-move, 0, 0);
            //if (Input.GetKey(KeyCode.RightArrow)) transform.position = transform.position + new Vector3(move, 0, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        isAlive = false;
    }
}
