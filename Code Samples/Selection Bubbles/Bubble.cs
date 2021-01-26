using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Bubble : NetworkBehaviour {
    
    public bool followHead;
    public bool deactivateOnUse;
    public bool grabOnce;
    private bool locked;
    public Text text; 
    protected GameManager gm;
    protected InventoryManager im;
    private GameObject cam;
    protected NetworkIdentity ni;
    protected Animator anim;
    private float lastGrabTime;
    private float grabCooldown = 1f;

    // Use this for initialization
    public virtual void Start()
    {
        gm = FindObjectOfType<GameManager>();
        im = FindObjectOfType<InventoryManager>();
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        ni = GetComponent<NetworkIdentity>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
   public virtual void Update () {
        if(!cam)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera");
            return;
        }
        if(followHead)
        {
            transform.LookAt(cam.transform);
        }
    }

    public virtual bool OnGrabbed()
    {
        if (!gameObject.activeSelf)
            return false;
        if (!CanBeGrabbedNow())
            return false;

        if (locked)
            return false;

        if (grabOnce)
            locked = true;

        lastGrabTime = Time.time;
        
        GrabGraphic();
        if(ni && ni.isServer)
            RpcGrabGraphic();

        return true;
    }

    [ClientRpc]
    void RpcGrabGraphic()
    {
        GrabGraphic();
    }
    void GrabGraphic()
    {
        if(anim)
            anim.SetTrigger("Collect");

        if (deactivateOnUse)
            Deactivate();
    }
    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }
    public virtual void Activate()
    {
        gameObject.SetActive(true);
        RpcActivate();
    }
    [ClientRpc]
    void RpcActivate()
    {
        gameObject.SetActive(true);
    }
    public void MoveTo(Vector3 pos)
    {
        transform.position = pos;
        RpcMoveTo(pos);
    }
    [ClientRpc]
    public void RpcMoveTo(Vector3 pos)
    {
        transform.position = pos;
    }

    public bool CanBeGrabbedNow()
    {
        if (Time.time - lastGrabTime < grabCooldown)
            return false;

        return true;
    }
}
