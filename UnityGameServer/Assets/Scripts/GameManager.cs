using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public static float spawnDistanceFromPlayer = 20f;
    public static float newObstacleSpawnedTime = 2f;

    private void Start()
    {
        Debug.Log("Started!");
        spawnDistanceFromPlayer = 20f;
    }

    void FixedUpdate()
    {
        if (newObstacleSpawnedTime <= 0f)
        {
            spawnDistanceFromPlayer += Random.Range(5f, 15f);

            GameObject obstacleObject = Instantiate(obstaclePrefab);
            obstacleObject.transform.position = new Vector3(
                Random.Range(-3f, 3f),
                1f,
                spawnDistanceFromPlayer
            );

            PacketSend.ObstacleSpawned(obstacleObject.transform.position);
            newObstacleSpawnedTime = Random.Range(2f, 4f);
        }
        else
        {
            newObstacleSpawnedTime -= Time.deltaTime;
        }
    }
}
