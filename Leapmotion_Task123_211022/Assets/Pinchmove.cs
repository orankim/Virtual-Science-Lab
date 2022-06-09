using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinchmove : MonoBehaviour
{
    //주먹쥐면 크레인을 자식으로 만들기
    public GameObject target;
    public GameObject newparent;

    public void Setparent()
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        target.transform.parent = newparent.transform;
    }

    public void Detachparent()
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
        target.transform.parent = null;
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
