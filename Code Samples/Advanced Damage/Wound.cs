using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wound
{
    HitLocationManager.HitLocation hitLocation;
    Vector3 position;
    float length;
    float depth;

    int damage;

    public Wound(HitLocationManager.HitLocation hitLocation, HitInfo hitInfo)
    {
        this.hitLocation = hitLocation;
        damage = hitInfo.damage;
        Inflict();
    }
    private void Inflict()
    {
        hitLocation.Health -= damage;
    }
    public void Remove()
    {
        hitLocation.Health += damage;
    }
}
