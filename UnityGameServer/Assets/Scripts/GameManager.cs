using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public float spawnDistanceFromPlayer = 20f;
    public float spawnDistanceFromObstacles = 15f;
    public float closestPosition;
    public Player player;

    private float obstaclePointer;

    private void Start()
    {
        Debug.Log("Started!");
    }

    void FixedUpdate()
    {
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                if (closestPosition < _client.player.controller.center.z)
                {
                    player = _client.player; //Get The closest Player
                }
            }
        }

        if (player) {
            if (obstaclePointer < player.controller.bounds.center.z)
            {
                obstaclePointer += spawnDistanceFromObstacles;

                GameObject obstacleObject = Instantiate(obstaclePrefab);
                obstacleObject.transform.position = new Vector3(
                    Random.Range(-3f, 3f),
                    1.5f,
                    player.controller.bounds.center.z + spawnDistanceFromPlayer
                );
            }
        }
    }
}
