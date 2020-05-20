using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour //works like the program class
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject gameManager;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //ensure that only one instance of this class exists makes sense for every single client you only have one
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0; //improve perfomance
        Application.targetFrameRate = 30; //server is pumping frames very fast, but the actual logic is ticking 30 ticks per second

        // Unity editor doesn't shutdown sockets when exiting playmode until you enter playmode again, which means
        // that the port you use will be taken until you enter playmode a second time
        //Server.Start(50, 26950);
        //#if UNITY_EDITOR
        //Debug.Log("Build the project to start the server!");
        //#else
        Server.Start(50, 26950);
        //#endif
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>(); //returns a reference of the player
    }

    public GameManager StartGameManager()
    {
        return Instantiate(gameManager).GetComponent<GameManager>();
    }
}
