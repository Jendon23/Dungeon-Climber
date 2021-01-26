using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMenuBubble : Bubble {

    enum ButtonType {Host,Client,Server };
    public int buttonType;

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
        if (!base.OnGrabbed()) { return false; }

        switch (buttonType)
        {
            case 0:
                {
                    gm.StartHost();
                    break;
                }
            case 1:
                {
                    gm.StartClient();
                    break;
                }
            case 2:
                {
                    gm.StartServer();
                    break;
                }
        }

        return true;
    }
}
