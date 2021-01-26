using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandSticker : MonoBehaviour
{
    GameManager gm;
    public PlayerController controller;
    public Text keys;
    public Text coins;
    public Image[] healthImages;

	// Use this for initialization
	void Start () {
        gm = GameObject.FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        keys.text = gm.im.KeysAcquired.ToString();
        coins.text = gm.im.inventory[(int)InventoryManager.InventoryItem.Coin].ToString();
        for(int i = 0; i < healthImages.Length; ++i)
        {
            if (controller.health >= i + 1)
                healthImages[i].enabled = true;
            else
                healthImages[i].enabled = false;
        }
	}
}
