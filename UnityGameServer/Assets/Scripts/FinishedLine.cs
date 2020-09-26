using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishedLine : MonoBehaviour
{
    public Player player;
    private int laps = 1;
    private int lapsDone = 0;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "4.6 kilómetros")
        {
            switch (NetworkManager.instance.sceneName)
            {
                case "200 metros": laps = Constants.twoHundredmeterLaps; break;
                case "500 metros": laps = Constants.fiveHundredmeterLaps; break;
            }
        }   
    }

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
                if (other.name == "86")
                {
                    lapsDone++;
                    if (SceneManager.GetActiveScene().name != "4.6 kilómetros")
                    {
                        PacketSend.UpdatePlayerLaps(player.id, lapsDone + 1);
                    }
                    if (lapsDone == laps)
                    {
                        player.reachedFinishLine = true;
                        PacketSend.PlayerFinishedGame(player.id, player.speed);
                    }
                    player.steps.Clear();
                }
            }
            if (player.steps.Count == 146 && SceneManager.GetActiveScene().name == "500 metros")
            {
                if (other.name == "146")
                {
                    lapsDone++;
                    if (SceneManager.GetActiveScene().name != "4.6 kilómetros")
                    {
                        PacketSend.UpdatePlayerLaps(player.id, lapsDone + 1);
                    }
                    if (lapsDone == laps)
                    {
                        player.reachedFinishLine = true;
                        PacketSend.PlayerFinishedGame(player.id, player.speed);
                    }
                    player.steps.Clear();
                }
            }
        }
    }
}
