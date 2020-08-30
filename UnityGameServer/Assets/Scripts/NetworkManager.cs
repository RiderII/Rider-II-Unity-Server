using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour //works like the program class
{
    public static NetworkManager instance;
    private bool destroyGameManager = false;
    public bool verifyDisconnection = false;

    public GameObject playerPrefab;
    public GameObject playerPrefabRigid;
    public GameObject gameManager;
    public GameObject gameManagerClone;
    public string sceneName;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
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
        Server.Start(4, 26950);
        //#endif
    }

    private void FixedUpdate()
    {
        if (sceneName == "Vaquita")
        {
            if (verifyDisconnection)
            {
                destroyGameManager = true;

                for (int i = 1; i <= Server.MaxPLayers; i++)
                {
                    if (Server.clients[i].tcp.socket != null)
                    {
                        destroyGameManager = false;
                        verifyDisconnection = false;
                        return;
                    }
                }

                if (destroyGameManager)
                {
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        Destroy(gameManagerClone.gameObject); //has to be destroyed in the main thread
                    });
                    verifyDisconnection = false;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        if (sceneName == "Vaquita")
        {
            return Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>(); //returns a reference of the player
        }
        else
        {
            return Instantiate(playerPrefabRigid, new Vector3(-25.706f, 0.606f, -21.200f), Quaternion.Euler(-0.807f, 111.458f, 0.017f)).GetComponent<Player>(); //returns a reference of the player
        }
    }

    public GameManager StartGameManager()
    {
        if (sceneName == "Vaquita")
        {
            if (gameManagerClone == null)
            {
                gameManagerClone = Instantiate(gameManager).GetComponent<GameManager>().gameObject;
            }
        }
        return null;
    }
}
