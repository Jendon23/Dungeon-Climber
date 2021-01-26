using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrip : MonoBehaviour
{
    public float grabRadius;

    public PlayerController pc;
    ConfigurableJoint grabJoint;
    public GameObject grabbedObject;
    public GameObject grabPointPrefab;
    bool usingGrabPoint;
    Collider grabbableInRange;

    void Start ()
    {
	}
	
	void Update ()
    {
        grabbableInRange = null;
        Collider[] grabbables = Physics.OverlapSphere(transform.position, grabRadius);
        foreach (Collider c in grabbables)
        {
            if(c.CompareTag("GrabButton"))
            {
                if (c.GetComponent<Bubble>())
                {
                    if (c.GetComponent<Bubble>().CanBeGrabbedNow())
                    {
                        grabbableInRange = c;
                        break;
                    }
                }
                else if(c.GetComponent<InventoryBubble>())
                {
                    if (c.GetComponent<InventoryBubble>().CanBeGrabbedNow())
                    {
                        grabbableInRange = c;
                        break;
                    }
                }
            }
            if (c.transform.root != transform.root && c.CompareTag("Grabbable"))
            {
                grabbableInRange = c;
                if(c.transform.root.gameObject.GetComponent<Rigidbody>() || c.gameObject.GetComponent<Rigidbody>())
                {
                    break;
                }
            }
        }
    }
    public void BeginGrab()
    {
        if (grabbableInRange == null || grabbedObject != null)
            return;

        grabbableInRange.gameObject.SendMessage("OnGrabbed", SendMessageOptions.DontRequireReceiver);

        if (grabbableInRange.CompareTag("GrabButton"))
        {
            return;
        }

        if(grabbableInRange.GetComponent<ComplexCollider>())
        {
            grabbableInRange.GetComponent<ComplexCollider>().Equip((ActorController)pc);
            /*Item grabItem = grabbableInRange.GetComponent<Item>();
            if (grabItem != null)
            {
                Vector3 worldOffset = new Vector3(grabItem.grabPoint.localPosition.x * grabItem.transform.localScale.x, grabItem.grabPoint.localPosition.y * grabItem.transform.localScale.y, grabItem.grabPoint.localPosition.z * grabItem.transform.localScale.z);
                grabItem.transform.position = transform.position-worldOffset;
                grabItem.transform.rotation = Quaternion.identity;
            }*/
        }

        grabJoint = gameObject.AddComponent<ConfigurableJoint>();
        SetupManipulatableJoint(ref grabJoint);
        grabbedObject = grabbableInRange.gameObject;
        Rigidbody connected = grabbedObject.GetComponent<Rigidbody>();
        if(connected == null)
        {
            connected = grabbedObject.transform.root.gameObject.GetComponent<Rigidbody>();
        }
        if (connected != null)
        {
            grabJoint.connectedBody = connected;
        }
        else
        {
            GameObject newGrabPoint = Instantiate(grabPointPrefab, transform.position, transform.rotation, grabbableInRange.transform) as GameObject;
            grabJoint.connectedBody = newGrabPoint.GetComponent<Rigidbody>();
            usingGrabPoint = true;
        }
    }
    public void EndGrab()
    {
        if (grabbedObject == null)
            return;

        grabbedObject.gameObject.SendMessage("OnReleased", SendMessageOptions.DontRequireReceiver);
        if (grabbedObject.GetComponent<ComplexCollider>())
        {
            grabbedObject.GetComponent<ComplexCollider>().Unequip(pc);
        }

        if (usingGrabPoint)
        {
            Destroy(grabJoint.connectedBody);
            usingGrabPoint = false;
        }
        Destroy(grabJoint);

        Bucket bucket = CheckForBucket();
        if(bucket != null)
        {
            bucket.TakeItem(grabbedObject.gameObject);
        }

        grabbedObject = null;
    }

    Bucket CheckForBucket()
    {
        Bucket foundBucket = null;
        Collider[] potentialBuckets = Physics.OverlapSphere(transform.position, grabRadius);
        foreach (Collider p in potentialBuckets)
        {
            foundBucket = p.GetComponent<Bucket>();
            if(foundBucket != null)
            {
                break;
            }
        }

        return foundBucket;
    }

    public static void SetupManipulatableJoint(ref ConfigurableJoint joint)
    {
        joint.anchor = Vector3.zero;
        //joint.autoConfigureConnectedAnchor = false;
        //joint.connectedAnchor = Vector3.zero;

        JointDrive drive = new JointDrive();
        drive.positionSpring = 1000000;
        drive.positionDamper = 10;
        drive.maximumForce = float.MaxValue;

        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;
        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;

        joint.swapBodies = true;
    }
}
