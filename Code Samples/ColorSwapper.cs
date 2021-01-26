using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwapper : MonoBehaviour {
    

    private Renderer rend;

    public Color ColorChannel_1;
    public Color ColorChannel_2;
    public Color ColorChannel_3;
    public Color ColorChannel_4;
    public Color ColorChannel_5;
    public Color ColorChannel_6;
    public Color ColorChannel_7;
    public Color ColorChannel_8; //yup, this is sub-optimal! spoooky repeated code that coders should hiss at. but if you think about it, the "cleaner" alternative is just this inside a list or array, which is more stuff and takes more operations to change. its not the cleanest, but it is considered optimized for its usage, since nothing will ever need more than 8 of them.
                                 //plus, the highly intelligent Hlapi apparently doesn't know what an array is, meaning I can't use any.
    public int ColorPosition_1;
    public int ColorPosition_2;
    public int ColorPosition_3;
    public int ColorPosition_4;
    public int ColorPosition_5;
    public int ColorPosition_6;
    public int ColorPosition_7;
    public int ColorPosition_8;

    private Color[] refColor;
    

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        InitializeChannels();
    }
	
	// Update is called once per frame
	void Update () {
        CheckForChanges();
	}
    
    void CheckForChanges()
    {
        if(ColorChannel_1 != refColor[0]) //alright, this is bad. your not smart for saying this is bad. its just not good, but extenuating circumstances took away my creativitity to think of a better alternative, and ability to give a fuck.
        {
            rend.material.SetColor("_ColorChannel1", ColorChannel_1);
            refColor[0] = ColorChannel_1;
        }
        if (ColorChannel_2 != refColor[1])
        {
            rend.material.SetColor("_ColorChannel2", ColorChannel_2);
            refColor[1] = ColorChannel_2;
        }
        if (ColorChannel_3 != refColor[2]) //its suprising how bad code looks when you take away arrays and lists.
        {
            rend.material.SetColor("_ColorChannel3", ColorChannel_3);
            refColor[2] = ColorChannel_3;
        }
        if (ColorChannel_4 != refColor[3])
        {
            rend.material.SetColor("_ColorChannel4", ColorChannel_4);
            refColor[3] = ColorChannel_4;
        }
        if (ColorChannel_5 != refColor[4]) //this is actually painful to write
        {
            rend.material.SetColor("_ColorChannel5", ColorChannel_5);
            refColor[4] = ColorChannel_5;
        }
        if (ColorChannel_6 != refColor[5])
        {
            rend.material.SetColor("_ColorChannel6", ColorChannel_6);
            refColor[5] = ColorChannel_6;
        }
        if (ColorChannel_7 != refColor[6])
        {
            rend.material.SetColor("_ColorChannel7", ColorChannel_7);
            refColor[6] = ColorChannel_7;
        }
        if (ColorChannel_8 != refColor[7]) //i have a stomach ache now
        {
            rend.material.SetColor("_ColorChannel8", ColorChannel_8);
            refColor[7] = ColorChannel_8;
        }
    }
    void InitializeChannels()
    {
        refColor = new Color[8];

        rend.material.SetInt("_ColorPosition1", ColorPosition_1);
        rend.material.SetInt("_ColorPosition2", ColorPosition_2);
        rend.material.SetInt("_ColorPosition3", ColorPosition_3);
        rend.material.SetInt("_ColorPosition4", ColorPosition_4);
        rend.material.SetInt("_ColorPosition5", ColorPosition_5);
        rend.material.SetInt("_ColorPosition6", ColorPosition_6);
        rend.material.SetInt("_ColorPosition7", ColorPosition_7);
        rend.material.SetInt("_ColorPosition8", ColorPosition_8);
    }
}
