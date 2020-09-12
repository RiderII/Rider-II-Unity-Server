using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedLine : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine"))
        {
            if (!player.steps.Contains(other.name) && (player.steps.Count + 1).ToString() == other.name)
            {
                player.steps.Add(other.name);
                PacketSend.UpdatePlayerSteps(player.id, player.steps.Count);
            }
            if (player.steps.Count == 86 && SceneManager.GetActiveScene().name == "200 metros")
            {
                player.reachedFinishLine = true;
                PacketSend.PlayerFinishedGame(player.id, player.speed);
            }
            if (player.steps.Count == 146 && SceneManager.GetActiveScene().name == "500 metros")
            {
                player.reachedFinishLine = true;
                PacketSend.PlayerFinishedGame(player.id, player.speed);
            }
        }
    }
}
