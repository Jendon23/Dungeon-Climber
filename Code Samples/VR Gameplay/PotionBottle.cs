using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBottle : MonoBehaviour {

    [SerializeField]
    private Condition condition;
    [SerializeField]
    private int amount;
    [SerializeField]
    private int capacity;

    public Transform top;

    private bool open = true;
    private bool held = true;
    private bool tilted;

	// Use this for initialization
	void Start () {
        held = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(held)
        {
            if (!tilted && open && Vector3.Angle(Vector3.down, transform.up) < 80)
            {
                tilted = true;
                StartCoroutine(Dump());
            }
        }
	}

    IEnumerator Dump()
    {
        while(tilted && held && open)
        {
            float angle = Vector3.Angle(Vector3.down, transform.up);
            if(angle > 80)
            {
                tilted = false;
            }
            else
            {
                RaycastHit hit;
                Physics.Raycast(top.position, Vector3.down, out hit);
                Debug.DrawLine(top.position, hit.point, Color.red, 0.1f);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void ApplyTo(ActorController target)
    {
        target.AddCondition(condition);
    }
    public void OnGrabbed()
    {
        held = true;
    }
    public void OnReleased()
    {
        held = false;
    }
}
