using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RedCoinKey : KeyBubble {

    private RedCoin[] redCoins;
    public int redCoinsLeft;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        redCoins = GameObject.FindObjectsOfType<RedCoin>();
        redCoinsLeft = redCoins.Length;
        StartCoroutine(SetIntWhenAnimatorIsReady("Forming", 2));
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override bool OnGrabbed()
    {
        if(!base.OnGrabbed()){return false; }

        return true;
    }

    public override void Deactivate()
    {
        foreach(RedCoin r in redCoins)
        {
            r.Deactivate();
        }
        base.Deactivate();
    }

    public void DepleteRedCoins(Vector3 pos)
    {
        redCoinsLeft--;
        if(redCoinsLeft == 0)
        {
            //Activate();
            //StartCoroutine(SetIntWhenAnimatorIsReady("Forming",1));
            anim.SetInteger("Forming", 1);
            RpcAnimForm(1);
            MoveTo(pos+Vector3.up);
        }
    }
    [ClientRpc]
    void RpcAnimForm(int state)
    {
        anim.SetInteger("Forming", state);
    }
    IEnumerator SetIntWhenAnimatorIsReady(string intName, int value)
    {
        yield return new WaitUntil(() => anim.isInitialized);
        anim.SetInteger(intName, value);
    }
}
