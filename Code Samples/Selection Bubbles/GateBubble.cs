using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GateBubble : Bubble {

    public int keysRequired;
    public string connectedScene;
    public string connectedSpawn;
    public LayerMask layer;
    public float radius;
    public enum State
    {
        Disabled,
        Inactive,
        Standby
    }
    public State state = State.Inactive;
    
    private bool ready;
    private bool animating;
    

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        anim = transform.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (!gm.im)
            return;
        anim.SetInteger("status", (int)state);
        //Debug.Log((int)state);
        switch (state)
        {
            case State.Disabled:
                break;
            case State.Inactive:
                if (gm.im.KeysAcquired >= keysRequired)
                {
                    text.text = "Grab to Activate";
                }
                else
                {
                    if(keysRequired > 1)
                        text.text = keysRequired + " Keys Required";
                    else
                        text.text = keysRequired + " Key Required";
                }
                break;
            case State.Standby:
                Collider[] VrPlayersOnGate = Physics.OverlapSphere(transform.position, radius, layer);
                int playersRequired = gm.GetNumVrPlayers();
                anim.SetInteger("PlayersOnGate", VrPlayersOnGate.Length);
                anim.SetBool("EnoughPlayers", VrPlayersOnGate.Length == playersRequired);
                if (VrPlayersOnGate.Length > 0)
                {
                    if (gm.im.KeysAcquired >= keysRequired)
                    {
                        //glowystuff
                        if (VrPlayersOnGate.Length == playersRequired)
                        {
                            ready = true;
                            text.text = "Grab to Enter";
                        }
                        else
                        {
                            text.text = VrPlayersOnGate.Length + " / " + playersRequired;
                        }
                    }
                    else
                    {
                        text.text = keysRequired + " Keys Required";
                    }
                }
                else
                {
                    ready = false;
                    text.text = connectedScene;
                    //make invisible
                }
                break;
            
        }
    }
    public override bool OnGrabbed()
    {
        if(!base.OnGrabbed()){return false;}

        RpcGrabAnim();
        GrabAnim();

        return true;
    }
    void GrabAnim()
    {
        switch (state)
        {
            case State.Disabled: //non functional, disabled
                break;
            case State.Inactive: //functional, but inactive
                if (gm.im.KeysAcquired >= keysRequired && !animating)
                {
                    animating = true;
                    StartCoroutine(Collecting());
                }
                break;
            case State.Standby: //active, ready to warp
                if (!animating)
                {
                    if (ready)
                    {
                        StartCoroutine(EnterGate(connectedScene));
                    }
                    else
                    {
                        //play error sound
                    }
                }
                break;
        }
    }
    [ClientRpc]
    public void RpcGrabAnim()
    {
        GrabAnim();
    }
    IEnumerator Collecting()
    {
        anim.SetTrigger("Activate");
        yield return new WaitForSeconds(7f);
        state = State.Standby;
        animating = false;
    }
    IEnumerator EnterGate(string scene)
    {
        animating = true;
        gm.sm.RpcMakeSoundEffect("Teleport",ni,autoParent:true);
        anim.SetTrigger("Activate");
        yield return new WaitForSeconds(2f);
        if (ready)
        {
            gm.LoadLevel(connectedScene,connectedSpawn);
        }
        else
        {
            //play error sound
        }
        animating = false;
    }
}
