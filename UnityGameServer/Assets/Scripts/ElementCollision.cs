using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollision : MonoBehaviour
{
    private bool isColliding = false;

    private void OnTriggerExit(Collider other)
    {
        Player player = other.transform.gameObject.GetComponent<Player>();
        if (tag == "RampUp")
        {
            StartCoroutine(SlowDown(player));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isColliding) return;
            Debug.Log("HITT!");
            isColliding = true;
            StartCoroutine(Reset());

            Player player = other.transform.gameObject.GetComponent<Player>();

            if (tag != "RampUp" && tag != "RampDown")
            {
                player.speed *= 0.80f;
                player.collisions += 1;
            }
                
            if (tag == "Tires")
            {
                PacketSend.ElementCollision("Tires", player, this);
            }
            else if (tag == "Rock")
            {
                PacketSend.ElementCollision("Rock", player, this);
            }
            else if (tag == "Tree")
            {
                PacketSend.ElementCollision("Tree", player, this);
            }

            else if (tag == "RampUp")
            {
                player.speed += 10;
                player.surpassSpeed = true;
                PacketSend.SpeedUp(player.id, true);
            }
        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        isColliding = false;
    }

    IEnumerator SlowDown(Player player)
    {
        yield return new WaitForSeconds(2);
        PacketSend.SpeedUp(player.id, false);
    }
}
