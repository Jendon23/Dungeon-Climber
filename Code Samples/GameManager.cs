using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkManager {
    
    public List<PlayerProfile> players = new List<PlayerProfile>();
    [SerializeField]
    int currentLevel = 0;
    private string currentSpawn = "OriginSpawn";
    public Level[] levels;
    public GameObject imPrefab;
    public GameObject smPrefab;
    public InventoryManager im;
    int[] savedInv;
    public SoundManager sm;

    [System.Serializable]
    public class PlayerProfile
    {
        public NetworkConnection conn;
        public int playerType = 0; //0 = vrPlayer, 1 = Desktop Fairy, 2 = external desktop fairy, 3 = spectator
        public string name;
    }


	// Use this for initialization
	void Awake ()
    {
        //DontDestroyOnLoad(im.gameObject);
	}
	void Start()
    {

    }
	// Update is called once per frame
	void Update () {
		
	}
    public void LoadLevel(string sceneName = "Lobby", string spawnTag = "")
    {
        for(int l = 0; l < levels.Length; ++l)
        {
            if(levels[l].sceneName == sceneName)
            {
                currentLevel = l;
                currentSpawn = spawnTag;
                savedInv = im.inventory;
                base.ServerChangeScene(sceneName);
                return;
            }
        }
    }
    public int GetNumVrPlayers()//this should eventually reflect a VR player list
    {
        int vrPlayersCount = 0;
        foreach(PlayerProfile p in players)
        {
            if(p.playerType == 0)
            {
                vrPlayersCount++;
            }
        }
        return vrPlayersCount;
    }
    public void Disconnect()
    {
        StopClient();
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        Debug.Log(conn.address + " has connected");
        PlayerProfile newPlayer = new PlayerProfile();
        newPlayer.conn = conn;
        newPlayer.playerType = 0;
        newPlayer.name = Random.Range(0, 9).ToString();
        players.Add(newPlayer);
        im.playerCount++;
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        for(int p = 0; p < players.Count; ++p)
        {
            if (players[p].conn == conn)
            {
                //save player info?
                Debug.Log(conn.address + " has disconnected");
                players.RemoveAt(p);
                im.playerCount--;
                break;
            }
        }
    }
    public override void OnServerSceneChanged(string sceneName)
    {

        base.OnServerSceneChanged(sceneName);

        GameObject newInventoryManager = Instantiate(imPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        if (savedInv != null)
            newInventoryManager.GetComponent<InventoryManager>().LoadInventory(savedInv);
        im = newInventoryManager.GetComponent<InventoryManager>();
        NetworkServer.Spawn(newInventoryManager);

        GameObject newSoundManager = Instantiate(smPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(newSoundManager);

        levels[currentLevel].LoadLevel();
    }
    public void AcquireKeyAtActiveLevel(int keyIndex)
    {
        if (levels[currentLevel].AcquireKey(keyIndex))
        {
            im.ModifyInventory(InventoryManager.InventoryItem.Key, 1);
        }
    }
    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Vector3 finalSpawn = FindApplicableRandomSpawnPoint(currentSpawn);
        GameObject player = (GameObject)Instantiate(playerPrefab, finalSpawn, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
    Vector3 FindApplicableRandomSpawnPoint(string tag)
    {
        //TaggedStartPosition[] startPositions = GameObject.FindObjectsOfType<TaggedStartPosition>();
        List<TaggedStartPosition> applicableStartPositions = new List<TaggedStartPosition>();
        Vector3 final = Vector3.zero;
        for(int i = 0; i < startPositions.Count; ++i)
        {
            Debug.Log(startPositions[i].GetComponent<TaggedStartPosition>());
            TaggedStartPosition start = startPositions[i].GetComponent<TaggedStartPosition>();
            if (start)
            {
                Debug.Log(tag +", "+start.connectedGate);
                if (start.connectedGate == tag)
                {
                    applicableStartPositions.Add(start);
                }
            }
        }

        if(applicableStartPositions.Count > 0)
        {
            final = applicableStartPositions[Random.Range(0, applicableStartPositions.Count)].transform.position;
        }

        return final;
    }
    public void ResetKeys()
    {
        for(int i = 0; i < levels.Length; ++i)
        {
            levels[i].ResetKeys();
        }
    }
    public void NetworkSpawnObject(GameObject go)
    {
        NetworkServer.Spawn(go);
    }
}
