using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bucket : MonoBehaviour
{
    protected GameManager gm;
	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual bool TakeItem(GameObject itemObject)
    {
        if (gm.im)
            return true;
        else
            return false;
    }
}
