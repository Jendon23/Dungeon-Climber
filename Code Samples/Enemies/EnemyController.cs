using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public abstract class EnemyController : ActorController
{
    protected NavMeshAgent navAgent;
    public ActorController hostileTarget;
    public PathNode nextNode;
    protected Rigidbody rb;

    public float walkSpeed = 3.5f;
    public float runSpeed = 3.5f;

    public enum State
    {
        Idle,
        Combat
    }

    public State state;
    public LayerMask hitLayer;

    // Use this for initialization
    public override void Start () {
        base.Start();
        rb = GetComponent<Rigidbody>();
        if(ni.isServer)
        {
            //StartCoroutine(Mumble());
        }
	}
	public virtual void Initialize()
    {
        navAgent = GetComponent<NavMeshAgent>();
        state = State.Idle;

        if (GetComponent<NetworkIdentity>().isServer)
        {
            StartCoroutine(FSM());
        }
    }
	// Update is called once per frame

    IEnumerator FSM()
    {
        while (true)
            yield return StartCoroutine(state.ToString());
    }
    public virtual IEnumerator Idle()
    {
        yield return null;
    }
    public virtual IEnumerator Attack()
    {
        yield return null;
    }
    public virtual IEnumerator Combat()
    {
        yield return null;
    }
    public virtual void MoveTo(Vector3 position)
    {
        navAgent.SetDestination(position);
    }
    public override IEnumerator Spawn()
    {
        navAgent.enabled = true;
        MoveTo(spawnPoint);
        yield return StartCoroutine(base.Spawn());
    }
    public override IEnumerator Die()
    {
        navAgent.enabled = false;
        hostileTarget = null;
        state = State.Idle;
        nAnim.SetTrigger("Death");
        yield return StartCoroutine(base.Die());
    }
    IEnumerator Mumble()
    {

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 10));
            gm.sm.RpcMakeSoundEffect(mumbleSound, ni, true);
        }
    }
    public override void Damage(int damage, ActorController source = null)
    {
        if(source)
        {
            hostileTarget = source;
        }
        base.Damage(damage,source);
    }
}
