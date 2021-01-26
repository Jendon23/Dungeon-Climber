using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMotor : MonoBehaviour
{
    public PartBodyController pbc;
    public bool isLeft;
    Rigidbody rb;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c)
    {
        Vector3 rV = rb.velocity;
        //if you want to do some relative velocity magic, do it here
        if(c.GetComponent<Rigidbody>())
        {
        }
        if(rV.magnitude > 1)
        {
            StartCoroutine(pbc.Recoil(isLeft, -rV));
        }
    }
}
