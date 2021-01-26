using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : Bubble {

    public int worth;

    public override bool OnGrabbed()
    {
        if (!base.OnGrabbed()){return false; }
        FindObjectOfType<GameManager>().sm.RpcMakeSoundEffect("Coin", ni, autoParent: false);
        im.ModifyInventory(InventoryManager.InventoryItem.Coin, worth);
        return true;
    }
    public override void Update()
    {
        base.Update();
        //transform.localRotation = Quaternion.Euler(transform.localRotation.x, Time.time * 100,transform.localRotation.z);
    }
}
