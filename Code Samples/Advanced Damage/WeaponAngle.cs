using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAngle : MonoBehaviour
{
    Rigidbody rb;
    public Quaternion angleOffset;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if(Physics.Raycast(transform.position,angleOffset*transform.forward,out hit,5))
        {
            AngleByHit(hit);
            Debug.Log("derp");
        }
        Debug.DrawLine(transform.position, transform.position+ angleOffset * transform.forward * 5);
    }
    void AngleByHit(RaycastHit hit)
    {
        Vector3 relativeHitPoint = Quaternion.Inverse(transform.rotation) * (hit.point - transform.position);
        Vector3 relativeVelocity = rb.GetRelativePointVelocity(relativeHitPoint)*Time.deltaTime;
        //angleOffset = Quaternion.FromToRotation(Vector3.forward, relativeHitPoint-relativeVelocity);
        angleOffset.SetLookRotation(relativeHitPoint);
    }
}
