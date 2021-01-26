using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BubbleButton : Bubble {

    public GameObject target;
    public string methodName;
    public string value;

    // Use this for initialization
    public override void Start () {
        base.Start();
        if (target == null)
            target = gm.gameObject;
	}

    // Update is called once per frame
    public override void Update () {
        base.Update();
	}

    public override bool OnGrabbed()
    {
        if(!base.OnGrabbed()){return false;}
        if (value == "")
        {
            target.SendMessage(methodName);
        }
        else
        {
            target.SendMessage(methodName,value, SendMessageOptions.DontRequireReceiver);
        }

        return true;
    }
}
