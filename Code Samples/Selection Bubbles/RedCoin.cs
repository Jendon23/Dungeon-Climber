using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCoin : Bubble
{
    public RedCoinKey key;

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
	}
    public override bool OnGrabbed()
    {
        if(!base.OnGrabbed()){return false;}
        key.DepleteRedCoins(transform.position);

        return true;
    }
}
