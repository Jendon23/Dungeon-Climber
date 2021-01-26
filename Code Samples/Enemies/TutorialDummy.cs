using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDummy : EnemyController {

    public float interactRange;
    public float sight;

    public override void Start()
    {
        base.Start();
        base.Initialize();
    }

    public override IEnumerator Idle()
    {
        Collider[] foundTargets;
        //StartCoroutine(Wander());

        while (Alive)
        {
            foundTargets = Physics.OverlapSphere(transform.position, sight, hitLayer);
            if (foundTargets.Length > 0)
            {
                float shortestDist = float.MaxValue;
                int closest = 0;
                for(int i = 0; i < foundTargets.Length; ++i)
                {
                    float distance = Vector3.Distance(transform.position, foundTargets[i].transform.position);
                    if(distance < shortestDist)
                    {
                        shortestDist = distance;
                        closest = i;
                    }
                }
                hostileTarget = foundTargets[closest].transform.root.GetComponent<ActorController>();

                if(shortestDist < interactRange)
                {
                    state = State.Combat;
                    break;
                }
            }
            yield return null;
        }

        state = State.Combat;
    }
    /*IEnumerator Wander()
    {
        while (state == State.Idle && Alive)
        {
            if (hostileTarget)
            {
                yield return new WaitForSeconds(Random.Range(3,7));
                if (hostileTarget == null && Alive)
                {
                    nextPosition = spawnPoint + new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));
                    NavMesh.SamplePosition(nextPosition, out hit, 5.0f, 1);
                    MoveTo(hit.position);
                }
            }
            yield return null;
        }
    }*/
}
