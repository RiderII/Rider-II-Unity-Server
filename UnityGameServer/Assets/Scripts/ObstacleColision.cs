using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleColision : MonoBehaviour
{
    private bool isColliding = false;
    private Player player;

    private void Start()
    {
        player = transform.gameObject.GetComponent<Player>();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            if (isColliding) return;
            Debug.Log("HITT!");
            isColliding = true;
            StartCoroutine(Reset());

            Player player = hit.controller.gameObject.GetComponent<Player>();
            player.speed *= player.obstacleSlowDown;
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

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RampUp")
        {
            StartCoroutine(SlowDown());
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.tag == "RampUp")
        {
            player.speed += 12;
            player.surpassSpeed = true;
            PacketSend.SpeedUp(player.id, true);
        }
    }

    IEnumerator SlowDown()
    {
        yield return new WaitForSeconds(2);
        player.surpassSpeed = false;
        PacketSend.SpeedUp(player.id, false);
    }
}
