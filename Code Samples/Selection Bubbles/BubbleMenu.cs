using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMenu : Bubble {

    [System.Serializable]
    class SubMenu
    {
        public Bubble[] options;
    }

    public Transform user;
    public bool deactivateOutOfRange;
    [SerializeField]
    SubMenu[] menu;
    int currentMenu;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if(user)
        {
            if(Vector3.Distance(user.position,transform.position) > 10)
            {
                if(deactivateOutOfRange)
                {
                    Deactivate();
                }

                ChangeCurrentMenu(0);
            }
        }
    }

    public override bool OnGrabbed()
    {
        if(!base.OnGrabbed()){return false; }

        return true;
    }

    public override void Deactivate()
    {
        for (int i = 0; i < menu.Length; ++i)
        {
            for (int j = 0; j < menu[i].options.Length; ++j)
            {
                menu[i].options[j].Deactivate();
            }
        }
        ChangeCurrentMenu(0);
        base.Deactivate();
    }
    void ChangeCurrentMenu(int newMenu)
    {
        for(int i = 0; i < menu[currentMenu].options.Length; ++i)
        {
            Debug.Log(menu[currentMenu] + ", " + menu[currentMenu].options[i]);
            menu[currentMenu].options[i].Deactivate();
        }
        currentMenu = newMenu;
        for (int i = 0; i < menu[currentMenu].options.Length; ++i)
        {
            menu[currentMenu].options[i].Activate();
        }
    }
    void ChangeCurrentMenu(string newMenu)
    {
        int result;
        int.TryParse(newMenu, out result);
        ChangeCurrentMenu(result);
    }
}
