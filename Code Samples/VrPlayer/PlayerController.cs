using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : ActorController
{
    NetworkInputHandler nih;
    PlayerMotor pm;

    //[SyncVar]
    public int tempHealth;
    //[SyncVar]
    public int tempMaxHealth;

    AssistedFlight aF;
	// Use this for initialization
	public override void Start ()
    {
        base.Start();
        nih = GetComponent<NetworkInputHandler>();
        pm = GetComponent<PlayerMotor>();
        aF = GetComponent<AssistedFlight>();
	}
	
	// Update is called once per frame
	void Update () // fixedupdate?
    {
		if(GetComponent<NetworkIdentity>().isServer)
        {
            HandleServerActions();
        }
	}

    void HandleServerActions()
    {
        if (!Alive)
            return;
        //handle walk
        if (nih.CurrentInput.input_move != Vector2.zero || nih.CurrentInput.input_jump)
        {
            pm.Move(nih.CurrentInput.input_move, nih.CurrentInput.input_jump);
            if(nih.CurrentInput.input_jump)
            {
                GameObject.FindObjectOfType<GameManager>().sm.RpcMakeSoundEffect("Jump", ni, autoParent: true);
            }
        }
        //handle grip
        switch(nih.CurrentInput.input_gripLState)
        {
            case 0:
                {
                    pm.playerGripL.EndGrab();
                    break;
                }
            case 1:
                {
                    pm.playerGripL.BeginGrab();
                    break;
                }
        }
        switch (nih.CurrentInput.input_gripRState)
        {
            case 0:
                {
                    pm.playerGripR.EndGrab();
                    break;
                }
            case 1:
                {
                    pm.playerGripR.BeginGrab();
                    break;
                }
        }

        //handle flying
        if (nih.CurrentInput.input_fly)
        {
            //aF.Fly(nih.CurrentInput.leftControllerPosition,nih.CurrentInput.leftControllerRotation);
            /*if (GetComponent<NetworkIdentity>().isServer)
            {
                CameraMarker marker = GameObject.FindObjectOfType<CameraMarker>();
                marker.transform.position = transform.position;
                marker.teleported = true;
            }*/

        }
    }

    //called on the client by NIH to see if the player is doing anything right now
    public NetworkInputHandler.InputPackage HandleActions(NetworkInputHandler.InputPackage inputPackage, SteamVR_Controller.Device leftDevice, SteamVR_Controller.Device rightDevice)
    {
        //Check Walk
        if (leftDevice.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            inputPackage.input_move = leftDevice.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            
        }
        else
        {
            inputPackage.input_move = Vector2.zero;
        }

        //Check Jump
        if (rightDevice.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            inputPackage.input_jump = true;
        }
        else
        {
            inputPackage.input_jump = false;
        }

        //Check Left Grab
        if (leftDevice.GetPress(SteamVR_Controller.ButtonMask.Grip) || leftDevice.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            inputPackage.input_gripLState = 1;
        }
        /*else if(leftDevice.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            inputPackage.input_gripLState = 2;
        }*/
        else
        {
            inputPackage.input_gripLState = 0;
        }

        //Check Right Grab
        if (rightDevice.GetPress(SteamVR_Controller.ButtonMask.Grip) || rightDevice.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            inputPackage.input_gripRState = 1;
        }
        /*else if (rightDevice.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            inputPackage.input_gripRState = 2;
        }*/
        else
        {
            inputPackage.input_gripRState = 0;
        }

        if(rightDevice.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            inputPackage.input_fly = true;
        }
        else
        {
            inputPackage.input_fly = false;
        }

        //portfolio temporary reset
        if (ni.isServer && rightDevice.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            gm.im.ModifyInventory(InventoryManager.InventoryItem.Key, -gm.im.inventory[(int)InventoryManager.InventoryItem.Key]);
            gm.im.ModifyInventory(InventoryManager.InventoryItem.Coin, -gm.im.inventory[(int)InventoryManager.InventoryItem.Coin]);
            gm.ResetKeys();
            gm.LoadLevel("MainHub", spawnTag: "OriginSpawn");
        }

        return inputPackage;
    }
    public override IEnumerator Die()
    {
        pm.playerGripL.EndGrab();
        pm.playerGripR.EndGrab();
        yield return StartCoroutine(base.Die());
    }
}
