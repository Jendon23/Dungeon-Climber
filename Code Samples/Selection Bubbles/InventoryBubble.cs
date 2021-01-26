using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBubble : MonoBehaviour
{
    GameManager gm;
    public InventoryManager.InventoryItem itemType;
    public GameObject itemPrefab;
    public Transform spawnPoint;
    public bool hideAtEmpty;
    public Renderer graphic;
    public Renderer emptyGraphic;
    private float lastGrabTime;
    private float grabCooldown = 1f;
    // Use this for initialization
    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
    }
    // Update is called once per frame
    public void Update()
    {
        if (hideAtEmpty)
        {
            if (gm.im.inventory[(int)itemType] <= 0 && graphic.enabled)
            {
                graphic.enabled = false;
                if (emptyGraphic)
                    emptyGraphic.enabled = true;
            }
            else if (gm.im.inventory[(int)itemType] > 0 && !graphic.enabled)
            {
                graphic.enabled = true;
                if (emptyGraphic)
                    emptyGraphic.enabled = false;
            }
        }
        SetText();
    }

    void SetText()
    {
        //text.text = currency.ToString() + ": " + gm.im.inventory[(int)currency];
    }

    public bool CanBeGrabbedNow()
    {
        if (Time.time - lastGrabTime < grabCooldown)
            return false;

        return true;
    }
    public bool OnGrabbed()
    {
        if (gm.im.inventory[(int)itemType] < 1)
            return false;

        if (!CanBeGrabbedNow())
            return false;

        lastGrabTime = Time.time;
        //gm.im.ModifyInventory(itemType, -1);
        GameObject newItem = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        gm.NetworkSpawnObject(newItem);

        return true;
    }
}
