using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PracticeBalloon : NetworkBehaviour {

    [SyncVar]
    public bool dead = false;
    public Material deadColor;
    public Material aliveColor;
    public float lifeTime = 60;
    public Transform[] parts;
    Material originalColor;
    // Use this for initialization
	void Start () {
        if (!GetComponent<NetworkIdentity>().isServer)
            enabled = false;

        //aliveColor = gameObject.GetComponent<Renderer>().material;
        StartCoroutine(DestroyAtEndOfLife());
	}
	
	// Update is called once per frame
	void Update () {
	}
    void OnCollisionEnter(Collision col)
    {
        if (dead) { return; }
        if (col.collider.gameObject.layer == LayerMask.NameToLayer("HitBox"))
        {
            RpcKill();
        }
        else if(col.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("HIT PLAYER");
        }
    }
    IEnumerator Blink()
    {
        int secondsToDie = 3;
        bool deadColored = false;

        for(int s = 0; s < secondsToDie*2; ++s)
        {
            for (int i = 0; i < parts.Length; ++i)
            {
                if (deadColored)
                {
                    parts[i].gameObject.GetComponent<Renderer>().material = aliveColor;
                }
                else
                {
                    parts[i].gameObject.GetComponent<Renderer>().material = deadColor;
                }
            }
            deadColored = !deadColored;
            yield return new WaitForSeconds(0.5f);
        }

        NetworkServer.Destroy(gameObject);
    }
    IEnumerator DestroyAtEndOfLife()
    {
        for (int s = 0; s < lifeTime; ++s)
        {
            yield return new WaitForSeconds(1);
        }
        NetworkServer.Destroy(gameObject);
    }
    [ClientRpc]
    void RpcKill()
    {
        Debug.Log("died");
        dead = true;
        StartCoroutine(Blink());
    }
}
