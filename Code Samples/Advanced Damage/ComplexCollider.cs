using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexCollider : MonoBehaviour
{

    public bool showGizmos = true;
    public bool showDebug = true;

    [System.Serializable]
    public class ExtraPartInfo
    {
        public Vector3 position;
        public Vector3 rotation;
        public float length;
    }

    [System.Serializable]
    public class EdgeInfo
    {
        public Vector3 position;
        public Vector3 rotation;
        public float sharpness;
        public float length;
    }
    [System.Serializable]
    public class VolumeInfo
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
    [System.Serializable]
    public class PointInfo
    {
        public Vector3 position;
        public Vector3 rotation;
        public float sharpness;
        public float length;
    }

    public List<ExtraPartInfo> extraParts;
    public List<EdgeInfo> edges;
    public List<VolumeInfo> volumes;
    public List<PointInfo> points;

    public List<Vector3> impactPoints;

    public LayerMask targetLayer;
    public LayerMask hitBoxLayer;

    public bool colliding = false;

    private Vector3 lastPosition;
    public ActorController holder;
    public Transform grabPoint;
    private Rigidbody rigidbody;

    // Use this for initialization
    void Start()
    {
        lastPosition = transform.position;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.DrawRay(transform.position, rigidbody.velocity, Color.blue, 1f);
        impactPoints.Clear();
        int hitsThisFrame = 0;
        for (int i = 0; i < edges.Count; i++)       //edge detection
        {
            RaycastHit[] hits;
            Quaternion euler = Quaternion.Euler(edges[i].rotation.x, edges[i].rotation.y, edges[i].rotation.z);
            hits = Physics.RaycastAll(transform.position + transform.rotation * edges[i].position, transform.rotation * euler * Vector3.right, edges[i].length, hitBoxLayer);

            if (hits.Length > 0)
            {
                //Debug.Log("Edge " + i + " of " + gameObject.name + " hit " + hits.Length + " thing(s)");
                hitsThisFrame += hits.Length;
                if (!colliding)
                {
                    for (int j = 0; j < hits.Length; j++)
                    {
                        if (!hits[j].transform.root.GetComponent<HitLocationManager>())
                        {
                            break;
                        }
                        if (targetLayer != (targetLayer | (1 << hits[j].transform.root.gameObject.layer)))
                        {
                            break;
                        }
                        Vector3 localHit = Quaternion.Inverse(transform.rotation) * (hits[j].point - transform.position);
                        impactPoints.Add(hits[j].point);

                        Vector3 otherVel = (hits[j].rigidbody) ? hits[j].rigidbody.GetPointVelocity(hits[j].point) : Vector3.zero;
                        Vector3 relVel = GetComponent<Rigidbody>().GetPointVelocity(hits[j].point) - otherVel;
                        relVel = Quaternion.Inverse(transform.rotation) * relVel;
                        relVel = Quaternion.Inverse(Quaternion.Euler(edges[i].rotation)) * relVel;
                        //Vector3 relVel = (hits[j].rigidbody) ? hits[j].rigidbody.velocity-GetComponent<Rigidbody>().velocity : -1*GetComponent<Rigidbody>().velocity;
                        float finalAngle = Vector3.Angle(relVel, Vector3.up);
                        float velXAngle = Vector3.Angle(Vector3.up, new Vector3(relVel.x, 0, 0));
                        float velZAngle = Vector3.Angle(Vector3.up, new Vector3(0, 0, relVel.z));

                        float angleScore = TestEdgeAngle(new Vector3(velXAngle, 0, velZAngle), relVel);
                        int resultingDamage = Mathf.FloorToInt(angleScore); //y is ignored for now  
                        //Debug.Log(angleScore);
                        if (resultingDamage > 0)
                        {
                            HitInfo newHit = new HitInfo();
                            newHit.damageType = HitInfo.DamageType.Cut;
                            newHit.damage = resultingDamage;
                            newHit.impactVelocty = relVel.magnitude;
                            if(holder)
                            {
                                newHit.source = holder;
                            }
                            Debug.Log("Inflicting " + resultingDamage + " " + newHit.damageType + " damage");
                            hits[j].transform.root.GetComponent<HitLocationManager>().InflictWound(hits[j].collider, newHit);
                        }
                    }
                }
            }
            //Debug.DrawLine(transform.position + transform.rotation * edges[i].position, transform.rotation * euler * Vector3.up);
        }
        for (int i = 0; i < volumes.Count; i++)       //volumes detection
        {
            RaycastHit[] hits;
            Quaternion euler = Quaternion.Euler(volumes[i].rotation.x, volumes[i].rotation.y, volumes[i].rotation.z);
            hits = Physics.BoxCastAll(transform.position + transform.rotation * volumes[i].position, new Vector3(volumes[i].scale.x / 2, volumes[i].scale.y / 2, volumes[i].scale.z / 2), transform.rotation * euler * Vector3.up, transform.rotation * euler, 0f, hitBoxLayer);

            if (hits.Length > 0)
            {
                //Debug.Log("Volume " + i + " of " + gameObject.name + " hit " + hits.Length + " thing(s)");
                hitsThisFrame += hits.Length;
                if (!colliding)
                {
                    for (int j = 0; j < hits.Length; j++)
                    {
                        if (!hits[j].transform.root.GetComponent<HitLocationManager>())
                        {
                            break;
                        }
                        if (targetLayer != (targetLayer | (1 << hits[j].transform.root.gameObject.layer)))
                        {
                            break;
                        }
                        //Vector3 localHit = Quaternion.Inverse(transform.rotation) * (hits[j].point - transform.position);
                        impactPoints.Add(hits[j].point);

                        Vector3 worldHitPoint = transform.localToWorldMatrix.MultiplyPoint(hits[j].point);

                        Vector3 otherVel = Vector3.zero; //(hits[j].rigidbody) ? hits[j].rigidbody.GetPointVelocity(hits[j].point) : Vector3.zero;
                        Vector3 relVel = GetComponent<Rigidbody>().GetPointVelocity(worldHitPoint) - otherVel;
                        //Debug.Log("velocity " + GetComponent<Rigidbody>().velocity);
                        //Debug.DrawLine(worldHitPoint, worldHitPoint + Vector3.up * 1,Color.red,1f);
                        //Debug.DrawRay(worldHitPoint, rigidbody.velocity, Color.blue, 1f);
                        relVel = Quaternion.Inverse(transform.rotation) * relVel;
                        relVel = Quaternion.Inverse(Quaternion.Euler(volumes[i].rotation)) * relVel;
                        //Vector3 relVel = (hits[j].rigidbody) ? hits[j].rigidbody.velocity-GetComponent<Rigidbody>().velocity : -1*GetComponent<Rigidbody>().velocity;
                        float finalAngle = Vector3.Angle(relVel, Vector3.up);
                        float velXAngle = Vector3.Angle(Vector3.up, new Vector3(relVel.x, 0, 0));
                        float velZAngle = Vector3.Angle(Vector3.up, new Vector3(0, 0, relVel.z));
                        int resultingDamage = Mathf.FloorToInt(TestVolumeAngle(new Vector3(velXAngle, 0, velZAngle), relVel)); //y is ignored for now
                        if (resultingDamage > 0)
                        {
                            HitInfo newHit = new HitInfo();
                            newHit.damageType = HitInfo.DamageType.Bash;
                            newHit.damage = resultingDamage;
                            newHit.impactVelocty = relVel.magnitude;
                            newHit.source = GetComponent<ActorController>();
                            Debug.Log("Inflicting " + resultingDamage + " " + newHit.damageType + " damage");
                            hits[j].transform.root.GetComponent<HitLocationManager>().InflictWound(hits[j].collider, newHit);
                        }
                    }
                }
            }
        }
        for (int i = 0; i < points.Count; i++)       //point detection
        {
            RaycastHit[] hits;
            Quaternion euler = Quaternion.Euler(points[i].rotation.x, points[i].rotation.y, points[i].rotation.z);
            hits = Physics.RaycastAll(transform.position + transform.rotation * points[i].position, transform.rotation * euler * Vector3.up, points[i].length, hitBoxLayer);

            if (hits.Length > 0)
            {
                //Debug.Log("Point " + i + " of " + gameObject.name + " hit " + hits.Length + " thing(s)");
                hitsThisFrame += hits.Length;
                if (!colliding)
                {
                    for (int j = 0; j < hits.Length; j++)
                    {
                        if (!hits[j].transform.root.GetComponent<HitLocationManager>())
                        {
                            break;
                        }
                        if (targetLayer != (targetLayer | (1 << hits[j].transform.root.gameObject.layer)))
                        {
                            break;
                        }
                        Vector3 localHit = Quaternion.Inverse(transform.rotation) * (hits[j].point - transform.position);
                        impactPoints.Add(hits[j].point);

                        Vector3 otherVel = (hits[j].rigidbody) ? hits[j].rigidbody.GetPointVelocity(hits[j].point) : Vector3.zero;
                        Vector3 relVel = GetComponent<Rigidbody>().GetPointVelocity(hits[j].point) - otherVel;
                        relVel = Quaternion.Inverse(transform.rotation) * relVel;
                        //Vector3 relVel = (hits[j].rigidbody) ? hits[j].rigidbody.velocity-GetComponent<Rigidbody>().velocity : -1*GetComponent<Rigidbody>().velocity; //next level sorcery shit here no its not nub
                        float finalAngle = Vector3.Angle(relVel, Vector3.up);
                        float velXAngle = Vector3.Angle(Vector3.up, new Vector3(relVel.x, 0, 0));
                        float velZAngle = Vector3.Angle(Vector3.up, new Vector3(0, 0, relVel.z));
                        int resultingDamage = Mathf.FloorToInt(TestPointAngle(new Vector3(velXAngle, 0, velZAngle), relVel)); //y is ignored for now
                        if (resultingDamage > 0)
                        {
                            HitInfo newHit = new HitInfo();
                            newHit.damageType = HitInfo.DamageType.Pierce;
                            newHit.damage = resultingDamage;
                            newHit.impactVelocty = relVel.magnitude;
                            newHit.source = GetComponent<ActorController>();
                            Debug.Log("Inflicting " + resultingDamage + " " + newHit.damageType + " damage");
                            hits[j].transform.root.GetComponent<HitLocationManager>().InflictWound(hits[j].collider, newHit);
                        }
                    }
                }
            }
            //Debug.DrawRay(transform.position + transform.rotation * points[i].position, transform.rotation * euler * Vector3.up);
        }
        if (hitsThisFrame > 0)
        {

            colliding = true;
        }
        else
        {
            colliding = false;
        }
        lastPosition = transform.position;
    }
    float TestEdgeAngle(Vector3 vel, Vector3 relVel)
    {
       // Debug.Log("hit with " + relVel.magnitude);
        //rate individual angle squished
        if (relVel.x <= 90 && relVel.z <= 20)
        {
            return 1;
        }
        return 0;
    }
    float TestVolumeAngle(Vector3 vel, Vector3 relVel)
    {
        //Debug.Log("hit with " + relVel.magnitude);
        if (relVel.magnitude > 1)
        {
            return 1;
        }
        return 0;
    }
    float TestPointAngle(Vector3 vel, Vector3 relVel)
    {
       // Debug.Log("hit with " + relVel.magnitude);
        if (vel.x <= 20 && vel.z <= 20 && relVel.magnitude > 1)
        {
            return 1;
        }
        return 0;
    }
    void RateStab(float angle)
    {
        if (angle > 160f)
        {
            Debug.log("Stab succeeded. Nice!")
        }
        else
        {
            Debug.log("That was barely a poke. Try again!")
        }
    }
    public void Equip(ActorController ac)
    {
        //Debug.Log("derp" + ac);
        holder = ac;
    }
    public void Unequip(ActorController ac)
    {
        //Debug.Log("derpaderp");
        holder = null;
    }
    void OnDrawGizmosSelected()
    {
        if (!showGizmos)
            return;
        Gizmos.color = Color.white;
        for (int i = 0; i < extraParts.Count; i++)
        {
            Quaternion euler = Quaternion.Euler(extraParts[i].rotation.x, extraParts[i].rotation.y, extraParts[i].rotation.z);
            Vector3 position1 = extraParts[i].position;
            Vector3 position2 = position1 + (euler * Vector3.up * extraParts[i].length);

            Gizmos.DrawWireSphere(transform.position + transform.rotation * position1, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * position2, 0.01f);

            Gizmos.DrawLine(transform.position + transform.rotation * position1, transform.position + transform.rotation * position2);
        }
        Gizmos.color = Color.blue;
        for (int i = 0; i < edges.Count; i++)
        {
            Quaternion euler = Quaternion.Euler(edges[i].rotation.x, edges[i].rotation.y, edges[i].rotation.z);
            Vector3 position1 = edges[i].position;
            Vector3 position2 = position1 + (euler * Vector3.right * edges[i].length);

            Gizmos.DrawWireSphere(transform.position + transform.rotation * position1, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * position2, 0.01f);

            Gizmos.DrawLine(transform.position + transform.rotation * position1, transform.position + transform.rotation * position2);

            for (int j = 0; j < edges[i].length * 10; j++)
            {
                Vector3 subposition1 = position1 + (euler * Vector3.right * ((edges[i].length / 10) * j));
                Vector3 subposition2 = subposition1 + (euler * Vector3.up * 0.1f);
                Gizmos.DrawLine(transform.position + transform.rotation * subposition1, transform.position + transform.rotation * subposition2);
            }
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < volumes.Count; i++)
        {
            Quaternion euler = Quaternion.Euler(volumes[i].rotation.x, volumes[i].rotation.y, volumes[i].rotation.z);
            Vector3 center = volumes[i].position;

            Vector3 position1 = center + (euler * Vector3.forward * (volumes[i].scale.z / 2)) + (euler * Vector3.right * (volumes[i].scale.x / 2)) - (euler * Vector3.up * volumes[i].scale.y / 2);
            Vector3 position2 = center - (euler * Vector3.forward * (volumes[i].scale.z / 2)) + (euler * Vector3.right * (volumes[i].scale.x / 2)) - (euler * Vector3.up * volumes[i].scale.y / 2);
            Vector3 position3 = center - (euler * Vector3.forward * (volumes[i].scale.z / 2)) - (euler * Vector3.right * (volumes[i].scale.x / 2)) - (euler * Vector3.up * volumes[i].scale.y / 2);
            Vector3 position4 = center + (euler * Vector3.forward * (volumes[i].scale.z / 2)) - (euler * Vector3.right * (volumes[i].scale.x / 2)) - (euler * Vector3.up * volumes[i].scale.y / 2);

            Vector3 subposition1 = position1 + (euler * Vector3.up * volumes[i].scale.y);
            Vector3 subposition2 = position2 + (euler * Vector3.up * volumes[i].scale.y);
            Vector3 subposition3 = position3 + (euler * Vector3.up * volumes[i].scale.y);
            Vector3 subposition4 = position4 + (euler * Vector3.up * volumes[i].scale.y);

            Gizmos.DrawWireSphere(transform.position + transform.rotation * position1, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * position2, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * position3, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * position4, 0.01f);

            Gizmos.DrawWireSphere(transform.position + transform.rotation * subposition1, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * subposition2, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * subposition3, 0.01f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * subposition4, 0.01f);


            //Gizmos.DrawWireCube(transform.position + transform.rotation * center, new Vector3(0.1f, 0.2f, 0.3f));
            Gizmos.DrawLine(transform.position + transform.rotation * position1, transform.position + transform.rotation * position2);
            Gizmos.DrawLine(transform.position + transform.rotation * position2, transform.position + transform.rotation * position3);
            Gizmos.DrawLine(transform.position + transform.rotation * position3, transform.position + transform.rotation * position4);
            Gizmos.DrawLine(transform.position + transform.rotation * position4, transform.position + transform.rotation * position1);

            Gizmos.DrawLine(transform.position + transform.rotation * position1, transform.position + transform.rotation * subposition1);
            Gizmos.DrawLine(transform.position + transform.rotation * position2, transform.position + transform.rotation * subposition2);
            Gizmos.DrawLine(transform.position + transform.rotation * position3, transform.position + transform.rotation * subposition3);
            Gizmos.DrawLine(transform.position + transform.rotation * position4, transform.position + transform.rotation * subposition4);

            Gizmos.DrawLine(transform.position + transform.rotation * subposition1, transform.position + transform.rotation * subposition2);
            Gizmos.DrawLine(transform.position + transform.rotation * subposition2, transform.position + transform.rotation * subposition3);
            Gizmos.DrawLine(transform.position + transform.rotation * subposition3, transform.position + transform.rotation * subposition4);
            Gizmos.DrawLine(transform.position + transform.rotation * subposition4, transform.position + transform.rotation * subposition1);
        }
        Gizmos.color = Color.green;
        for (int i = 0; i < points.Count; i++)
        {
            Quaternion euler = Quaternion.Euler(points[i].rotation.x, points[i].rotation.y, points[i].rotation.z);
            Vector3 position1 = points[i].position;
            Vector3 position2 = (position1 + (euler * Vector3.up * points[i].length));

            Gizmos.DrawWireSphere(transform.position + transform.rotation * position1, 0.01f);
            Gizmos.DrawLine(transform.position + transform.rotation * position1, transform.position + transform.rotation * position2);
        }
        Gizmos.color = Color.white;
        for (int i = 0; i < impactPoints.Count; i++)
        {
            Gizmos.DrawWireSphere(impactPoints[i], 0.01f);
        }
    }

}
