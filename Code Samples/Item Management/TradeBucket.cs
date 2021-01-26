using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeBucket : Bucket
{
    public override bool TakeItem(GameObject itemObject)
    {
        if (!base.TakeItem(itemObject))
            return false;



        return true;
    }
}
