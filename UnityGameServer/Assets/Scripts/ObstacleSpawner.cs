using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    private bool isColliding = false;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            if (isColliding) return;
            Debug.Log("HIT!");
            isColliding = true;
            StartCoroutine(Reset());
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(2);
        isColliding = false;
    }

    private void PlayerCollided(Player _player)
    {
        Player player = _player;
        player.collisions += 1;
        Debug.Log($"COLISIONES: {player.username}");
        Debug.Log($"COLISIONES: {player.collisions}");
        ServerSend.PlayerCollided(player);
    }
}
