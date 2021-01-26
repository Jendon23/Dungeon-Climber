using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class GoombaController : EnemyController
{
    public float attackRange;
    public float attackAngle;
    public float aggroRange;
    public float wanderRange;
    public float maxChaseRange;

    public ParticleSystem particles;
    // Use this for initialization
    public override void Start () {
        base.Start();
        base.Initialize();
        if (center == null)
            center = transform;
    }
	
	// Update is called once per frame
	void Update () {
	}

    IEnumerator FSM()
    {
        while (true) // it doesnt actually return to this often, so this is just supposed to redirect when the loop comes back. it directs it to the current state coroutine!
            yield return StartCoroutine(state.ToString());
    }
    public override IEnumerator Idle()
    {
        navAgent.speed = walkSpeed;

        Collider[] foundTargets;
        StartCoroutine(Wander());

        while (!hostileTarget && Alive)
        {
            foundTargets = Physics.OverlapSphere(transform.position, aggroRange, hitLayer);
            if (foundTargets.Length > 0)
            {
                if (Vector3.Distance(foundTargets[0].transform.position, spawnPoint) < maxChaseRange)
                {
                    hostileTarget = foundTargets[0].transform.root.GetComponent<ActorController>();
                }
            }
            yield return null;
        }
        
        state = State.Combat;
    }
    IEnumerator Wander()
    {
        Vector3 nextPosition = spawnPoint + new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));
        NavMeshHit hit;
        NavMesh.SamplePosition(nextPosition, out hit, 1.0f, 1);
        MoveTo(hit.position);

        while (hostileTarget == null && Alive)
        {
            if(navAgent.remainingDistance < 1f)
            {
                yield return new WaitForSeconds(2);
                if(hostileTarget == null && Alive)
                {
                    nextPosition = spawnPoint + new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));
                    NavMesh.SamplePosition(nextPosition, out hit, 5.0f, 1);
                    MoveTo(hit.position);
                }
            }
            yield return null;
        }
    }
    public override IEnumerator Attack()
    {
        nAnim.SetTrigger("Attack1");
        yield return new WaitForSeconds(2);
    }
    public override IEnumerator Combat()
    {
        //navAgent.enabled = false;
        navAgent.speed = runSpeed;

        while(hostileTarget && hostileTarget.Alive && Alive)
        {
            float distance = Vector3.Distance(transform.position, hostileTarget.center.position);
            if (distance <= attackRange && Alive)
            {
                //Debug.Log("Succeeded distance check with " + distance);
                MoveTo(transform.position);
                RotateTowards(hostileTarget.center);
                float angle = Vector2.Angle(new Vector2(transform.forward.x, transform.forward.z), (new Vector2(hostileTarget.center.position.x, hostileTarget.center.position.z) - new Vector2(transform.position.x, transform.position.z)));
                if (angle <= attackAngle)
                {
                    //Debug.Log("Succeeded angle check with " + angle);
                    yield return StartCoroutine(Attack());
                }
            }
            else if (distance > aggroRange)
            {
                hostileTarget = null;
            }
            else
            {
                MoveTo(hostileTarget.center.position);
            }
            yield return null;
        }

        //navAgent.enabled = true;
        state = State.Idle;
    }
    public override void MoveTo(Vector3 position)
    {
        float distanceFromSpawn = Vector3.Distance(position, spawnPoint);
        if (distanceFromSpawn > maxChaseRange)
        {
            state = State.Idle;
            return;
            //position = spawnPoint+(position-spawnPoint).normalized*maxChaseRange;
        }

        if (navAgent.enabled)
        {
            navAgent.SetDestination(position);
        }
        else
        {
            //MoveTowards(position - transform.position);
        }
    }
    private void MoveTowards(Vector3 direction)
    {
        direction.y = 0;
        rb.AddForce(direction * navAgent.speed, ForceMode.Acceleration);
    }
    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1);// Time.deltaTime * navAgent.angularSpeed);
    }
    public void berp()
    {
        particles.Emit(50);
    }
    public void derp()
    {
        TeleportTo(new Vector3(0, -20, 0));
        StartCoroutine(KillSlowly());
    }
    public IEnumerator KillSlowly()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

}
