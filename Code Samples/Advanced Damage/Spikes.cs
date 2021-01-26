using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnCollisionEnter(Collision collision)
    {
        HitLocationManager hitLocations = collision.transform.root.GetComponent<HitLocationManager>();
        if (hitLocations)
        {
            HitInfo hitInfo = new HitInfo();
            hitInfo.damageType = HitInfo.DamageType.Pierce;
            hitInfo.impactVelocty = collision.relativeVelocity.magnitude;
            hitInfo.damage = 1;
            hitLocations.InflictWound(collision.collider, hitInfo);
        }
    }
}
