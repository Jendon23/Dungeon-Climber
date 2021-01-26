using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActorController : NetworkBehaviour {

    public Transform center; //defaults to transform if unspecified, used to find the center for targeting for nonstandard actors
    protected NetworkIdentity ni;
    public NetworkAnimator nAnim;
    protected GameManager gm;

    [SyncVar]
    public int health = 3;
    public int maxHealth = 3;
    private bool alive = true;

    public List<Condition> conditions = new List<Condition>();
    public Vector3 spawnPoint;
    public bool doesRespawn;

    public string hurtSound;
    public string deathSound;
    public string jumpSound;
    public string spawnSound;
    public string mumbleSound;

    float lastHit;
    float hitCooldown = 0.5f;

    public Dictionary<string, float> factionStanding = new Dictionary<string, float>();

    public int Health { get { return health; } set { health = value; if (health <= 0) { health = 0; StartCoroutine(Die()); } else if (health > maxHealth) health = maxHealth; } }

    public bool Alive
    {
        get
        {
            return alive;
        }

        set
        {
            alive = value;
        }
    }

    // Use this for initialization
    public virtual void Start ()
    {
        spawnPoint = transform.position;
        ni = GetComponent<NetworkIdentity>();
        nAnim = GetComponent<NetworkAnimator>();

        if (center == null)
            center = transform;

        gm = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddCondition(Condition condition)
    {
        conditions.Add(condition);
        condition.Subject = this;
    }
    public void RemoveCondition(Condition condition)
    {
        conditions.Remove(condition);
    }
    public virtual void Damage(int damage, ActorController source = null)
    {
        if (Time.time - lastHit > hitCooldown && alive)
        {
            if (hurtSound != "")
                gm.sm.RpcMakeSoundEffect(hurtSound,ni, autoParent: true);
            Health -= damage;
            Debug.Log("Hurt" + damage);
            lastHit = Time.time;
            //hurt animations
        }
    }
    public virtual IEnumerator Spawn()
    {

        //screen fade from white
        transform.position = spawnPoint;
        health = maxHealth;
        gm.sm.RpcMakeSoundEffect(spawnSound,ni, autoParent: true);
        yield return new WaitForSeconds(2f);
        Alive = true;
    }
    public virtual IEnumerator Die()
    {
        if (deathSound != "")
            gm.sm.RpcMakeSoundEffect(deathSound,ni, autoParent: true);
        Alive = false;
        //death animations
        //screen fade to black
        yield return new WaitForSeconds(3);
        if (doesRespawn)
        {
            StartCoroutine(Spawn());
        }
    }
    public virtual void TeleportTo(Vector3 position)
    {
        transform.position = position;
    }
    public void Deactivate()
    {
    }
}
