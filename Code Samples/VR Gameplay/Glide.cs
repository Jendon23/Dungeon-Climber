using UnityEngine;
using System.Collections;

public class Glide : MonoBehaviour {
    Rigidbody rb;
    public Vector3 wind;
    SteamVR_Controller.Device device;
    public bool left;
    public float butterForce = 1.2f;
    // Use this for initialization
    void Start () {
        //   if(rb == null)
        rb = GetComponent<Rigidbody>();
        if(left)
            device = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost));
        else
            device = SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost));
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("flying");
            float surfaceArea = transform.lossyScale.x * transform.lossyScale.z;


            float a = Vector3.Dot(rb.velocity.normalized, transform.up * -1);
            float cl = 2 * Mathf.PI * (a * Mathf.Deg2Rad);
            float l = cl * (butterForce * rb.velocity.sqrMagnitude) / 2 * surfaceArea;

            float aWindy = Vector3.Dot(wind * -1, transform.up * -1);
            float clWindy = 2 * Mathf.PI * (aWindy * Mathf.Deg2Rad);
            float lWindy = clWindy * (1.2F * wind.sqrMagnitude) / 2 * surfaceArea;

            //rb.AddForceAtPosition(1 * transform.up, transform.localPosition + transform.forward * (transform.localScale.z / 2));
            rb.AddForceAtPosition(transform.up * l, transform.position);
            rb.AddForceAtPosition(transform.up * lWindy, transform.position);
            //   print(Vector3.Dot(rb.velocity.normalized, transform.forward) * -1);
        }
    }
    void OnDrawGizmos()
    {
        if (rb != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.up*5);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + rb.velocity * 5);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + wind * 5);
        }
    }
}
