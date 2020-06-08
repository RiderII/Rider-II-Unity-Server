using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishGame : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            PacketSend.PlayerFinishedGame(player.id);
            GameManager.spawnDistanceFromPlayer = 20f;
            GameManager.newObstacleSpawnedTime = 5f;
        }
    }
}
