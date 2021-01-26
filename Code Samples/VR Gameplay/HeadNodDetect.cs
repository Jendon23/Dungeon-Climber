using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;

public class HeadNodDetect : MonoBehaviour {

    public Transform head;
    public float threshold;
    private Vector3 startAngle;
    private Vector3[] angles;
    private int index;
    public int recordFrames = 40;

    public NetworkIdentity ni;

	// Use this for initialization
	void Start () {
        head = transform;
        ResetGesture();
	}
	
	// Update is called once per frame
	void Update () {
        angles[index] = head.rotation.eulerAngles;
        index++;
        if(index == angles.Length)
        {
            CheckGesture();
            ResetGesture();
        }
	}
    void CheckGesture()
    {
        bool down = false, up = false, left = false, right = false;

        for(int i = 0; i < angles.Length; i++)
        {
            if(angles[i].x < startAngle.x - threshold)
            {
                down = true;
            }
            else if (angles[i].x > startAngle.x + threshold)
            {
                up = true;
            }
            if (angles[i].y < startAngle.y - threshold)
            {
                left = true;
            }
            else if (angles[i].y > startAngle.y + threshold)
            {
                right = true;
            }
        }

        if (down && up && !(left && right))
        {
            //nod detected
            Debug.Log("YES");
            GameObject.FindObjectOfType<GameManager>().sm.RpcMakeSoundEffect("Coin", ni, true);
        }
        if (!(down && up) && left && right)
        {
            //shake detected
            Debug.Log("NO");
            GameObject.FindObjectOfType<GameManager>().sm.RpcMakeSoundEffect("Die", ni, true);
        }
    }
    void ResetGesture()
    {
        startAngle = head.rotation.eulerAngles;
        angles = new Vector3[recordFrames];
        index = 0;
    }
}
