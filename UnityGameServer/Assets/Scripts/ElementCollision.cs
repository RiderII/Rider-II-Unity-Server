using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCollision : MonoBehaviour
{
    private bool isColliding = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isColliding) return;
            Debug.Log("HITT!");
            isColliding = true;
            StartCoroutine(Reset());
            Player player = other.transform.gameObject.GetComponent<Player>();
            player.speed *= 0.80f;
            player.collisions += 1;

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

        }
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        isColliding = false;
    }
}
