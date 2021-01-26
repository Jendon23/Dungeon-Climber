using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AssistedFlight : MonoBehaviour
{

    //public Transform tempHead;
    //public Transform tempLHand;

    Rigidbody rb;
    public float mult;
    public Transform flyFocus;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Fly(Vector3 input,Quaternion inputRot)
    {
        Vector3 direction = (input - (flyFocus.localPosition+new Vector3(0,transform.localScale.y,0))).normalized;
        float distance = (input - (flyFocus.localPosition + new Vector3(0, transform.localScale.y, 0))).sqrMagnitude;
        Quaternion rot = inputRot;

        rb.AddForce(direction*distance*mult,ForceMode.VelocityChange);
        //rb.AddTorque(rot.eulerAngles.normalized * rot.eulerAngles.sqrMagnitude);
    }
}
