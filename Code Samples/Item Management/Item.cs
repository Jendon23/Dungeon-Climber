using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Item : NetworkBehaviour {

    public InventoryManager.InventoryItem itemType;
    public Transform grabPoint;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Remove()
    {
        if(GetComponent<NetworkIdentity>().isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

}
