using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : NetworkBehaviour {

    public GameObject soundSourcePrefab;

    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

    void Awake()
    {
        //DontDestroyOnLoad(this);
        GameObject.FindObjectOfType<GameManager>().sm = this;
        Object[] loadedSounds = Resources.LoadAll("Sounds", typeof(AudioClip));
        for (int i = 0; i < loadedSounds.Length; ++i)
        {
            AudioClip loadedClip = (AudioClip)loadedSounds[i];
            sounds.Add(loadedClip.name, loadedClip);
        }
    }
    // Use this for initialization
    void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    [ClientRpc]
    public void RpcMakeSoundEffect(string soundName, NetworkIdentity id, bool autoParent)
    {
        if(!sounds.ContainsKey(soundName))
        {
            return;
        }
        Transform parent = id.transform;
        if(!autoParent)
        {
            parent = null;
        }
        GameObject soundSource = Instantiate(soundSourcePrefab, id.transform.position, Quaternion.identity,parent) as GameObject;
        AudioSource audio = soundSource.GetComponent<AudioSource>();
        audio.clip = sounds[soundName];
        audio.Play();
        Destroy(soundSource, audio.clip.length);
    }
}
