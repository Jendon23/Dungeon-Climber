using UnityEngine;
using UnityEngine.Networking;

public class TransformMotion : NetworkBehaviour
{
    
    private Vector3 lastPos;
    private Vector3 nextPos;
    private Quaternion lastRot;
    private Quaternion nextRot;
    private Vector3 lastVel;
    private Transform myTransform;
    private float lastSync;
    private float syncDelay;
    private bool hasRB = false;
    Rigidbody rb;
    [SerializeField]
    private float lerpRate = 10;
    [SerializeField]
    private float posThreshold = 0.5f;
    [SerializeField]
    private float rotThreshold = 5;
    [SerializeField]
    private float velThreshold = 0.5f;

    // Use this for initialization
    void Start()
    {
        myTransform = transform;
        if (GetComponent<Rigidbody>())
        {
            hasRB = true;
            rb = GetComponent<Rigidbody>();

            if (!GetComponent<NetworkIdentity>().isServer)
            {
                rb.isKinematic = true;
            }
        }
    }
    // Update is called once per frame
    /*
    void Update()
    {
        if (!GetComponent<NetworkIdentity>().isServer && hasRB && !GetComponent<Rigidbody>().isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    */
    void Update()
    {
        if(GetComponent<NetworkIdentity>().isServer)
        {
            RpcTransmitMotion(transform.position,transform.rotation);
        }
    }
    void FixedUpdate()
    {
        if (!Network.isServer)
        {
            LerpMotion();
        }
    }

    [ClientRpc]
    void RpcTransmitMotion(Vector3 pos, Quaternion rot)
    {
        lastPos = pos;
        lastRot = rot;
        //nextRot = transform.rotation * Quaternion.Euler(angVel);

        syncDelay = Time.time - lastSync;
        lastSync = Time.time;

        /*if (hasAuthority)
        {
            if (Vector3.Distance(myTransform.position, lastPos) > posThreshold || Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold || (hasRB && Vector3.Distance(GetComponent<Rigidbody>().velocity,lastVel) > velThreshold))
            {

                Cmd_ProvidePositionToServer(myTransform.position, myTransform.rotation, (hasRB) ? GetComponent<Rigidbody>().velocity : Vector3.zero);

                lastPos = myTransform.position;
                lastRot = myTransform.rotation;
                lastVel = (hasRB) ? GetComponent<Rigidbody>().velocity : Vector3.zero;
            }
        }*/
    }

    void LerpMotion()
    {
        if (!hasAuthority)
        {
            float lastUpdateProgress = (Time.time - lastSync) / syncDelay;
            //transform.position = Vector3.Lerp(lastPos, nextPos, lastUpdateProgress);
            //myTransform.rotation = Quaternion.Lerp(lastRot, nextRot, lastUpdateProgress);
            transform.position = lastPos;
            transform.rotation = lastRot;
            /*if(hasRB)
            {
                GetComponent<Rigidbody>().velocity = syncVel;
            }*/
        }
    }
}