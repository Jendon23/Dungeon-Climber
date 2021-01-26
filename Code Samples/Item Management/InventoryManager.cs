using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryManager : NetworkBehaviour
{
    public enum InventoryItem { Key,Coin, Sword, Lantern };
    [SerializeField]
    public int[] inventory;
    public int KeysAcquired { get { return inventory[0]; } set { inventory[0] = value; } }
    [SyncVar]
    public int playerCount;
    // Use this for initialization
    void Awake()
    {
        GameObject.FindObjectOfType<GameManager>().im = this;
        //DontDestroyOnLoad(this);
    }
    void Start ()
    {
        if(GetComponent<NetworkIdentity>().isServer)
        {
            RpcSyncInventory(inventory);
        }
	}
	
	// Update is called once per frame
	void Update () {

    }
    public void LoadInventory(int[] inv)
    {
        inventory = inv;
    }
    public void ModifyInventory(InventoryItem item, int amount)
    {
        inventory[(int)item] += amount;
        if (inventory[(int)item] < 0)
        {
            inventory[(int)item] = 0;
        }
        RpcSyncInventory(inventory);
    }
    [ClientRpc]
    void RpcSyncInventory(int[] inv)
    {
        inventory = inv;
    }
}
