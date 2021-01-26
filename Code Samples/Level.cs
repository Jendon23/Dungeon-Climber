using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
    public string sceneName;
    public bool[] keysCollected;

    public void LoadLevel()
    {
        KeyBubble[] keysInLevel = GameObject.FindObjectsOfType<KeyBubble>();
        for(int i = 0; i < keysInLevel.Length; ++i)
        {
            if(keysCollected[keysInLevel[i].keyIndex])
            {
                Debug.Log(keysInLevel[i].name);
                keysInLevel[i].Deactivate();
            }
        }
    }
    public bool AcquireKey(int keyIndex)
    {
        if(!keysCollected[keyIndex])
        {
            keysCollected[keyIndex] = true;
            return true;
        }

        return false;
    }
    public void ResetKeys()
    {
        for(int i = 0; i < keysCollected.Length; ++i)
        {
            keysCollected[i] = false;
        }
    }
}
