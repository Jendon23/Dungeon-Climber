using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBubble : Bubble {
    
    public int keyIndex;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override bool OnGrabbed()
    {
        if(!base.OnGrabbed()){return false;}
        gm.AcquireKeyAtActiveLevel(keyIndex);
        FindObjectOfType<GameManager>().sm.RpcMakeSoundEffect("Key",ni, autoParent: false);
        return true;
    }

    public override void Deactivate()
    {
        //poof effect
        //turn to ghost key
        base.Deactivate();
    }
    IEnumerator SetBoolWhenAnimatorIsReady(string boolName, bool value)
    {
        yield return new WaitUntil(() => anim.isInitialized);
        anim.SetBool(boolName, value);
    }
    public void EnemiesClear()
    {
        Activate();
        StartCoroutine(SetBoolWhenAnimatorIsReady("Forming", true));
    }
}
