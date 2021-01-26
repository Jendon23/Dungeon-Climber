using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    class WeaponHitInfo
    {
        public RaycastHit i1;
        public RaycastHit i2;
        public Vector3 start;
        public Vector3 end;
        public Vector3 forward;
        public bool exit;
        public float i1d;
        public float i2d;
    }

    public float thresh = 5;
    public float tempMult;
    public Collider weaponCollider;
    public Vector3 weaponStart;
    public float weaponLength;
    public float weaponWidth;
    public GameObject hitIndicator;
    Rigidbody rb;
    Collider other;
    Vector3 forceToApply = Vector3.zero;
    Vector3 positionToApply = Vector3.zero;
    bool isDeadly;
    bool inObject;

    //public List<RaycastHit> firstIntersects = new List<RaycastHit>();
    //public List<RaycastHit> secondIntersects = new List<RaycastHit>();
    List<WeaponHitInfo> weaponHitInfoList = new List<WeaponHitInfo>();

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //Vector3 worldWeaponStart = GetWeaponStartPosition();
        //Debug.DrawLine(worldWeaponStart, worldWeaponStart + weaponCollider.transform.forward * weaponLength,Color.red);

        /*if(firstIntersects.Count > 1)
        {
            for(int i = 0; i < firstIntersects.Count-1; ++i)
            {
                Debug.DrawLine(firstIntersects[i].point, firstIntersects[i + 1].point, Color.red);
            }
        if (secondIntersects.Count > 1)
            for (int i = 0; i < secondIntersects.Count - 1; ++i)
            {
                Debug.DrawLine(secondIntersects[i].point, secondIntersects[i + 1].point, Color.red);
            }
        }*/
    }
    void FixedUpdate()
    {
        /*if (forceToApply != Vector3.zero)
        {
            rb.AddForceAtPosition(forceToApply, positionToApply);
            forceToApply = Vector3.zero;
            positionToApply = Vector3.zero;
        }*/
        SetWeaponTrigger(PassesThreshold());
    }

    Vector3 GetWeaponStartPosition()
    {
        return weaponCollider.transform.rotation*weaponStart + weaponCollider.transform.position;
    }

    void SetWeaponTrigger(bool toSet)
    {
        if(toSet)
        {
            weaponCollider.isTrigger = true;
            weaponCollider.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
        else if(!inObject)
        {
            weaponCollider.isTrigger = false;
            weaponCollider.gameObject.GetComponent<Renderer>().material.color = Color.black;
        }
        else
        {
            weaponCollider.gameObject.GetComponent<Renderer>().material.color = Color.black;
        }
    }

    WeaponHitInfo CalculateIntersects(Collider c)
    {
        Vector3 start = GetWeaponStartPosition();
        Vector3 end = start + weaponCollider.transform.forward * weaponLength;

        RaycastHit i1;
        RaycastHit i2;
        
        if (!c.Raycast(new Ray(start, weaponCollider.transform.forward), out i1, weaponLength))
            return null;
        bool exit = c.Raycast(new Ray(end, -weaponCollider.transform.forward), out i2, weaponLength);

        WeaponHitInfo info = new WeaponHitInfo();
        info.i1 = i1;
        info.i1.point -= c.transform.position;
        info.i2 = i2;
        info.i2.point -= c.transform.position;
        info.start = start - c.transform.position;
        info.end = end - c.transform.position;
        info.forward = weaponCollider.transform.forward;
        info.exit = exit;
        info.i1d = (i1.point - start).magnitude;
        
        if (!exit)
        {
            info.i2d = weaponLength;
        }
        else
        {
            info.i2d = (i2.point-start).magnitude;
        }

        return info;
    }

    void MakeGraphicOfWound(int index1, int index2)
    {
        Vector3 entryPoint = other.transform.position + weaponHitInfoList[index1].i1.point;
        Vector3 exitPoint = other.transform.position + weaponHitInfoList[index2].i1.point;
        Vector3 midPoint = (entryPoint + exitPoint) / 2;
        float woundLength = (entryPoint - exitPoint).magnitude;

        Quaternion rot = Quaternion.FromToRotation(entryPoint, exitPoint);
        GameObject graphic = Instantiate(hitIndicator, midPoint, Quaternion.identity) as GameObject;
        //Debug.Log(midPoint);
        graphic.transform.localScale = new Vector3(0.01f, 0.01f, woundLength);
        graphic.transform.LookAt(exitPoint);
        graphic.transform.parent = other.transform;

        //Debug.Log("made graphic");
    }
    void MakeMultiGraphicOfWound()
    {
        if(weaponHitInfoList.Count > 1)
        {
            for(int i = 0; i < weaponHitInfoList.Count-1;++i)
            {
                MakeGraphicOfWound(i, i + 1);
            }
        }
    }

    Vector3 GetClosestVertexOnBounds(Collider c, Vector3 p)
    {
        Vector3 nearestVertex = Vector3.zero;
        float nearestDist = Mathf.Infinity;
        foreach (Vector3 vertex in c.GetComponent<MeshFilter>().mesh.vertices)
        {
            Vector3 diff = p - vertex;
            float distSqr = diff.sqrMagnitude;
            if (distSqr < nearestDist)
            {
                nearestDist = distSqr;
                nearestVertex = vertex;
            }
        }
        return nearestVertex;
    }

    void ApplyDragAtPositionByAngle(Vector3 p, float d, Vector3 e)
    {
        Vector3 v = rb.velocity;
        float drag = d;
        Vector3 final = v.normalized * -drag;
        /*if (final.magnitude > v.magnitude)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            r.AddForceAtPosition(v.normalized * -drag, p);
        }*/
        //r.AddForceAtPosition(v.normalized * -drag, p,ForceMode.VelocityChange);
        float finalDrag = drag - v.magnitude;
        if (finalDrag < 0)
            finalDrag = v.magnitude;
        /*if(!weaponCollider.bounds.Contains(p+rb.velocity))
        {
            forceToApply = v.normalized * -finalDrag;
            positionToApply = p;
        }
        else
        {
            Debug.Log("success");
        }*/
        float angle = Vector3.Angle(rb.GetPointVelocity(p), Vector3.up);
        if(angle > 45)
        {
            forceToApply = v.normalized * -finalDrag;
            positionToApply = p;
        }
        else
        {
            Debug.Log("success, angle is < 45");
        }
    }

    bool PassesThreshold()
    {
        Vector3 midPoint = GetWeaponStartPosition() + weaponCollider.transform.forward * (weaponLength / 2);
        Vector3 v = rb.GetPointVelocity(midPoint);

        if (v.magnitude > thresh)
            return true;

        return false;
    }

    void OnTriggerEnter(Collider c)
    {
        weaponCollider.gameObject.GetComponent<Renderer>().material.color = Color.yellow;

        WeaponHitInfo hit = CalculateIntersects(c);
        if (hit == null)
        {
            return;
        }
        inObject = true;
        weaponHitInfoList.Clear();
        weaponHitInfoList.Add(hit);
        //Debug.Log("entered");

        other = c;

        //Vector3 midPoint = hit.start + ((hit.i1d + hit.i2d) / 2) * hit.forward;
        //ApplyDragAtPositionByAngle(midPoint, tempMult, Vector3.zero);
    }
    void OnTriggerStay(Collider c)
    {
        if (c != other)
            return;
        WeaponHitInfo hit = CalculateIntersects(c);
        if (hit == null)
        {
            return;
        }
        weaponHitInfoList.Add(hit);

        //Vector3 midPoint = hit.start + ((hit.i1d + hit.i2d) / 2) * hit.forward;
        //ApplyDragAtPositionByAngle(midPoint, tempMult, Vector3.zero);
    }
    void OnTriggerExit(Collider c)
    {
        if (c != other)
            return;
        inObject = false;
        WeaponHitInfo hit = CalculateIntersects(c);
        if (hit != null)
            weaponHitInfoList.Add(hit);
        //Debug.Log("exited");
        //if (weaponHitInfoList.Count > 0)
        //    MakeMultiGraphicOfWound();
    }
}
