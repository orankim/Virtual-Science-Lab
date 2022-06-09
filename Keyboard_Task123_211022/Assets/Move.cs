using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    float move = 0.1f;

    //public Text status;
    void Start()
    {
        //status.text="Start";
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.LeftArrow)) transform.position = transform.position + new Vector3(-move, 0, 0);
        if (Input.GetKey(KeyCode.RightArrow)) transform.position = transform.position + new Vector3(move, 0, 0);
        //if (Input.GetKey(KeyCode.UpArrow)) transform.position = transform.position + new Vector3(0, move, 0);
        //if (Input.GetKey(KeyCode.DownArrow)) transform.position = transform.position + new Vector3(0, -move, 0);
        if (Input.GetKey(KeyCode.UpArrow)) transform.position = transform.position + new Vector3(0, 0, move);
        if (Input.GetKey(KeyCode.DownArrow)) transform.position = transform.position + new Vector3(0, 0, -move);

    }
}
