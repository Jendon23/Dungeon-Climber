using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartBodyController : MonoBehaviour
{

    public Rigidbody head;
    public Rigidbody handL;
    public Rigidbody handR;

    public ConfigurableJoint headJoint;
    public ConfigurableJoint handLJoint;
    public ConfigurableJoint handRJoint;

    public Vector3 controllerHeadPosition;
    public Vector3 controllerHandLPosition;
    public Vector3 controllerHandRPosition;

    public bool headSync = true;
    public bool leftHandSync = true;
    public bool rightHandSync = true;

    public bool tempActivate;

    NetworkInputHandler.InputPackage currentPose;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(tempActivate)
        {
            StartCoroutine(Recoil(true, Vector3.left));
            tempActivate = false;
        }
        /*if (controllerHead)
        {
            headJoint.targetPosition = controllerHead.localPosition - new Vector3(0, transform.localScale.y, 0);
            headJoint.targetRotation = controllerHead.localRotation;
        }
        if (controllerHandL)
        {
            handLJoint.targetPosition = controllerHandL.localPosition - new Vector3(0, transform.localScale.y, 0);
            handLJoint.targetRotation = controllerHandL.localRotation;
        }
        if (controllerHandR)
        {
            handRJoint.targetPosition = controllerHandR.localPosition - new Vector3(0, transform.localScale.y, 0);
            handRJoint.targetRotation = controllerHandR.localRotation;
        }*/

        //GetComponent<CapsuleCollider>().center = new Vector3(controllerHead.localPosition.x, 0, controllerHead.localPosition.z);
        //GetComponent<CapsuleCollider>().height = controllerHead.localPosition.y;
    }

    public void SetPartTransforms(NetworkInputHandler.InputPackage poseInput)
    {
        currentPose = poseInput;

        if (headSync)
        {
            headJoint.targetPosition = poseInput.headControllerPosition - new Vector3(0, transform.localScale.y, 0);
            headJoint.targetRotation = poseInput.headControllerRotation;
        }
        
        if (leftHandSync)
        {
            handLJoint.targetPosition = poseInput.leftControllerPosition - new Vector3(0, transform.localScale.y, 0);
            handLJoint.targetRotation = poseInput.leftControllerRotation;
        }
        if (rightHandSync)
        {
            handRJoint.targetPosition = poseInput.rightControllerPosition - new Vector3(0, transform.localScale.y, 0);
            handRJoint.targetRotation = poseInput.rightControllerRotation;
        }
        
        //Debug.Log(poseInput.headControllerPosition - new Vector3(0, transform.localScale.y, 0));

        //GetComponent<CapsuleCollider>().center = new Vector3(poseInput.headControllerPosition.x, GetComponent<CapsuleCollider>().center.y, poseInput.headControllerPosition.z);

        //GetComponent<CapsuleCollider>().height = (poseInput.headControllerPosition.y - transform.localScale.y/2)*2;
    }

    public IEnumerator Recoil(bool left, Vector3 force)
    {
        ConfigurableJoint joint;
        Rigidbody hand;
        Vector3 differenceToHand = Vector3.zero;
        float minRecoverTime = force.magnitude;
        Vector3 deltaThisTick = Vector3.zero;
        if(left)
        {
            leftHandSync = false;
            joint = handLJoint;
            hand = handL;
        }
        else
        {
            rightHandSync = false;
            joint = handRJoint;
            hand = handR;
        }
        Debug.Log(differenceToHand.sqrMagnitude + ", " + minRecoverTime);

        while (minRecoverTime > 0 || (minRecoverTime <= 0 && differenceToHand.sqrMagnitude > 0.1))
        {
            differenceToHand = (transform.position + currentPose.leftControllerPosition - new Vector3(0, transform.localScale.y, 0)) - hand.position;
            
            minRecoverTime -= Time.fixedDeltaTime*10;
            Debug.Log(differenceToHand.sqrMagnitude+", "+minRecoverTime);
            if (minRecoverTime > 0)
            {
                //implement drag equation? (m^2 * d)/2
                joint.targetPosition += force/200;
                force = force.normalized * minRecoverTime;
                Debug.Log(-force);
            }

            yield return new WaitForFixedUpdate();
        }

        if(left)
        {
            leftHandSync = true;
        }
        else
        {
            rightHandSync = true;
        }
    }
}
