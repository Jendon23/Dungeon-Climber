using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayerLoader : NetworkBehaviour
{
    public GameObject cameraRig;
    PartBodyController partController;


    public GameObject headGraphic;
    public GameObject bodyGraphic;

	// Use this for initialization
	void Start ()
    {
        partController = GetComponent<PartBodyController>();
		if(!GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            cameraRig.SetActive(false);
        }
        else
        {
            headGraphic.GetComponent<MeshRenderer>().enabled = false;
            bodyGraphic.GetComponent<MeshRenderer>().enabled = false;
        }
        if(!GetComponent<NetworkIdentity>().isServer)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            partController.head.isKinematic = true;
            partController.handL.isKinematic = true;
            partController.handR.isKinematic = true;
        }
        else
        {
            GetComponent<PartBodyController>().enabled = true;
            GetComponent<PlayerMotor>().enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	}
}
