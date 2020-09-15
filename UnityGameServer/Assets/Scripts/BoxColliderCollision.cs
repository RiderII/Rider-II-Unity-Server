using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderCollision : MonoBehaviour
{
    public Player player;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            PacketSend.PlayerCollidedWithOtherPlayer(player.speed, true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            PacketSend.PlayerCollidedWithOtherPlayer(player.speed, false);
        }
    }
}
