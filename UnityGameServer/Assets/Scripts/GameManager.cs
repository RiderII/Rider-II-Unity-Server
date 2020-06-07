using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public float spawnDistanceFromPlayer = 20f;
    private float newObstacleSpawnedTime = 2f;

    private void Start()
    {
        Debug.Log("Started!");
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

            ServerSend.ObstacleSpawned(obstacleObject.transform.position);
            newObstacleSpawnedTime = Random.Range(2f, 4f);
        }
        else {
            newObstacleSpawnedTime -= Time.deltaTime;
        }
    }
}
