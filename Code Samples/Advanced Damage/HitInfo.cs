using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitInfo
{
    public enum DamageType { Cut, Pierce, Bash}
    public DamageType damageType;
    public float impactVelocty;
    public int damage;
    public ActorController source;
}
