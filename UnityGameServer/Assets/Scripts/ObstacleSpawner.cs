using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public static Dictionary<int, ObstacleSpawner> spawners = new Dictionary<int, ObstacleSpawner>();
    private static int nextSpawnerId = 1;

    public int spawnerId;
    public bool hasItem = false;

    private void Start()
    {
        hasItem = false;
        spawnerId = nextSpawnerId;
        nextSpawnerId++;
        spawners.Add(spawnerId, this);

        StartCoroutine(SpawnObstacle());
    }

    private IEnumerator SpawnObstacle()
    {
        yield return new WaitForSeconds(10f);

        hasItem = true;
    }
}
