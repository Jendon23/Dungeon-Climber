using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLocationManager : MonoBehaviour {
    [System.Serializable]
    public class DamageRule
    {
        public int lowerBound;
        public int upperBound;
        public Condition.ConditionType conditionType;
        private Condition condition;

        public void UpdateDamageCondition(int health, ActorController controller)
        {
            if(condition == null && health >= lowerBound && health <= upperBound)
            {
                condition = Condition.TypeToCondition(conditionType);
                controller.AddCondition(condition);
            }
            else if(condition != null && health < lowerBound || health > upperBound)
            {
                controller.RemoveCondition(condition);
                condition = null;
            }
        }
    }
    [System.Serializable]
    public class HitLocation
    {
        private ActorController controller;
        public string name;
        [SerializeField]
        private int health;
        [SerializeField]
        private string animation;
        public int maxHealth;
        public Collider collider;
        List<Wound> wounds = new List<Wound>();

        public float[] weaknesses;
        public DamageRule[] damageRules;

        public ActorController Controller { get { return controller; } set { controller = value; } }

        public int Health { get { return health; } set { health = value; if (health < 0) health = 0; else if (health > maxHealth) health = maxHealth; } }

        public void InflictWound(HitInfo info)
        {
            Debug.Log(controller.transform.root.name + " hit on " + name + " with " + info.damageType + " damage");
            float weaknessMod = 1;
            if (weaknesses.Length > (int)info.damageType)
            {
                weaknessMod = weaknesses[(int)info.damageType];
            }
            int damage = (int)(info.damage * weaknessMod);
            //health -= damage;
            controller.Damage(damage,info.source);
            if (animation != "")
            {
                if(controller.nAnim)
                    controller.nAnim.SetTrigger(animation);
            }

            for(int i = 0; i < damageRules.Length; ++i)
            {
                damageRules[i].UpdateDamageCondition(health, controller);
            }
        }
    }

    public ActorController controller;
    public HitLocation[] hitLocations;

    public bool invulnerable;
    
	void Start ()
    {
		foreach(HitLocation hl in hitLocations)
        {
            hl.Controller = controller;
        }
	}
	
	void Update () {
		
	}
    public void InflictWound(Collider c, HitInfo info)
    {
        if (invulnerable)
            return;
        HitLocation hitLocation = null;
        foreach(HitLocation h in hitLocations)
        {
            if(h.collider == c)
            {
                hitLocation = h;
                break;
            }
        }
        if(hitLocation == null)
        {
            Debug.Log("Could not find collider in " + transform.root + " when hit");
            return;
        }
        //Debug.Log(info.source.transform.name + " inflicted " + info.damage + " " + info.damageType.ToString() + "ing damage to " + transform.name + "'s " + hitLocation.name));
        Debug.Log(info.damageType);
        hitLocation.InflictWound(info);
    }
}
