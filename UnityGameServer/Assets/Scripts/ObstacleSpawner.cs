using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner instance;
    public GameObject coneStack;
    public GameObject tiresStack;
    public GameObject rockStack;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnObstacles(int playerId)
    {
        foreach (Transform child in coneStack.transform)
        {
            PacketSend.ObstacleSpawned2(int.Parse(child.name), child.transform.position, child.transform.rotation, "cone", playerId);
        }

        foreach (Transform child in tiresStack.transform)
        {
            foreach (Transform childofchild in child)
            {
                PacketSend.ObstacleSpawned2(int.Parse(childofchild.name), childofchild.transform.position, childofchild.transform.rotation, "tire", playerId);
            }
        }

        if (rockStack)
        {
            foreach (Transform child in rockStack.transform)
            {
                PacketSend.ObstacleSpawned2(int.Parse(child.name), child.transform.position, child.transform.rotation, "rock", playerId);
            }
        }
    }
}
