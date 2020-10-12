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
        if (SceneManager.GetActiveScene().name != "4.6 kilometros")
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

            if (player.steps.Contains(other.name))
            {
                EraseArrou(other);
                if (int.Parse(player.lastGlassRef.name) > int.Parse(other.name))
                {
                    player.lastGlassRef = other.gameObject;
                    PacketSend.ActivateAlert(player.id, true);
                }
                else
                {
                    player.lastGlassRef = other.gameObject;
                    PacketSend.ActivateAlert(player.id, false);
                }
            }
            if (!player.steps.Contains(other.name))
            {
                EraseArrou(other);
                player.lastGlassRef = other.gameObject;
                PacketSend.ActivateAlert(player.id, false);
            }

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
                    if (SceneManager.GetActiveScene().name != "4.6 kilometros")
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
                    if (SceneManager.GetActiveScene().name != "4.6 kilometros")
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

    private void EraseArrou(Collider other)
    {
        if (player.ptArrow && player.ptArrow.transform.position.x == other.gameObject.transform.position.x &&
                    player.ptArrow.transform.position.z == other.gameObject.transform.position.z)
        {
            player.arrowActive = false;
            PacketSend.DeleteArrow(player.id);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                Destroy(player.ptArrow);
            });
        }
    }
}
