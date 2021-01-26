using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBucket : Bucket
{
    public InventoryManager.InventoryItem acceptedItem;

    public override bool TakeItem(GameObject itemObject)
    {
        if (!base.TakeItem(itemObject))
            return false;

        Item item = itemObject.GetComponent<Item>();
        if (item != null && item.itemType == acceptedItem)
        {
            item.Remove();
            gm.im.ModifyInventory(item.itemType, 1);
            return true;
        }

        return false;
    }
}
