using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleColision: MonoBehaviour
{
    private bool isColliding = false;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            if (isColliding) return;
            Debug.Log("HITT!");
            isColliding = true;
            StartCoroutine(Reset());

            Player player = hit.controller.gameObject.GetComponent<Player>();
            PlayerCollided(player);
        }
    }


    IEnumerator Reset()
    {
        yield return new WaitForSeconds(2);
        isColliding = false;
    }

    private void PlayerCollided(Player _player)
    {
        _player.collisions += 1;
        Debug.Log($"COLISIONES: {_player.username}");
        Debug.Log($"COLISIONES: {_player.collisions}");
        PacketSend.PlayerCollided(_player);
    }
}
