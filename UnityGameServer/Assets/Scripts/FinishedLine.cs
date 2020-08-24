using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishedLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            Player player = other.GetComponent<Player>();
            if (!player.steps.Contains(tag))
            {
                player.steps.Add(tag);
            }
            if (player.steps.Count == 4 && tag == "FinishLine")
            {
                player.reachedFinishLine = true;
                PacketSend.PlayerFinishedGame(player.id, player.speed);
            }
                
        }
    }
}
