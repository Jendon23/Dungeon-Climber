using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
This script's purpose is to ease player climbing onto ledges, since the player can't rotate their body like in real life. While climbing, the player's torso and legs
will pass through terrain, and will move back to the player's original height when they let go of the ground. Without this script, a player would have to move
their hands down to their feet to pull themselves over a ledge, now they can just drag themselves forward.

*/
public class PlayerStabilizer : MonoBehaviour
{

    CapsuleCollider capsuleCollider;

    float headSize = 0.35f;

    public float distanceToGround;
    public float distanceToBottom;


    [Range(0f, 1f)]
    public float colliderSize = 1f;

    public LayerMask EBP; //everything but player


    public PlayerMotor pm;
    public PartBodyController pbc;
    // Use this for initialization
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        pm = GetComponent<PlayerMotor>();
        pbc = GetComponent<PartBodyController>();
    }

    // Update is called once per frame
    void Update()
    {
        //figure out position of bottom of head
        float bottomOfHead = pbc.head.transform.localPosition.y - (headSize / 2);
        //figure out distance from bottom of head to bottom of box
        distanceToBottom = bottomOfHead + 1;
        //figure out distance from bottom of head to supposed ground
        RaycastHit groundHit;
        //Debug.DrawLine(GetComponent<PartBodyController>().head.transform.position - new Vector3(0, (headSize / 2) + 0.01f, 0), GetComponent<PartBodyController>().head.transform.position - new Vector3(0, (headSize / 2) + 0.01f - distanceToBottom, 0));
        if (Physics.Raycast(pbc.head.transform.position - new Vector3(0, (headSize / 2), 0), Vector3.down, out groundHit, distanceToBottom+0.1f, EBP))
        {
            if (colliderSize < 1f &&
                (!pm.playerGripL.grabbedObject || (pm.playerGripL.grabbedObject && pm.playerGripL.grabbedObject.GetComponent<Rigidbody>())) &&
                (!pm.playerGripR.grabbedObject || (pm.playerGripR.grabbedObject && pm.playerGripR.grabbedObject.GetComponent<Rigidbody>())))
            {
                /*
                 * if grabbed does exist and rigidbody exists
                 */
                colliderSize = distanceToGround/distanceToBottom;
                colliderSize += 0.01f;
                if(colliderSize > 1f) { colliderSize = 1f; }
            }
            distanceToGround = Vector3.Distance(pbc.head.transform.position - new Vector3(0, (headSize / 2), 0), groundHit.point);
            //Debug.Log("name = " + groundHit.transform.name + " bottom of head = " + bottomOfHead + " distance to bottom = " + distanceToBottom + " distance to ground = " + distanceToGround);
            //do math to calculate the center Y position and height of the collider
            float colliderHeight = distanceToBottom * colliderSize;
            float colliderCenter = bottomOfHead - ((distanceToBottom / 2) * colliderSize);
            //Debug.Log(colliderCenter);
            capsuleCollider.height = colliderHeight;
            capsuleCollider.center = new Vector3(pbc.head.transform.localPosition.x, colliderCenter, pbc.head.transform.localPosition.z);

            capsuleCollider.isTrigger = false;
        }
        else
        {
            colliderSize = 0f;
            distanceToGround = -1;
            capsuleCollider.isTrigger = true;
            capsuleCollider.height = 0;
            //Debug.Log("something");
            capsuleCollider.center = new Vector3(pbc.head.transform.localPosition.x, 0, pbc.head.transform.localPosition.z);
        }
    }
}
