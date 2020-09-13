using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PacketHandle
{
    public static void RequestEnterLobby(int _fromClient, Packet _packet) //read data from packet in the same order it was sent.
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        if (_username != "Middleware")
        {
            string _league = _packet.ReadString();
            string _scene = _packet.ReadString();


            if (NetworkManager.instance.sceneName == "")
            {
                NetworkManager.instance.sceneName = _scene;
                SceneManager.LoadScene(_scene);
            }

            Server.clients[_fromClient].username = _username; //we want to set the client name when logged in
                                                              // being logged in is implies connecting to the server automatically, but not necessarily into the game
                                                              // clients names will appear in a list within the invite options perhaps, so we want the names of the connected clients
                                                              // to be public so that they can be invited to get into a game

            Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint + " " + Server.clients[_fromClient].username} " +
                $"connected successfully and is now a player with id: {_fromClient}");
            if (_fromClient != _clientIdCheck) //Check if the client claimed the correct id
            {
                Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }

            // Send  player into game (this is automatic when the users connects to a server we want to modify this behaviour)
            Server.clients[_fromClient].SendIntoLobby(_league);
        }
        else
        {
            Server.clients[_fromClient].username = _username;
            Debug.Log($"Rider II middleware sending data to player with id: {_clientIdCheck}");
        }
    }

    public static void SendReadyState(int _fromClient, Packet _packet)
    {
        int clientId = _packet.ReadInt();
        PacketSend.SendReadyState(_fromClient);
    }

    public static void SendToGame(int _fromClient, Packet _packet)
    {
        int clientId = _packet.ReadInt();
        string _username = _packet.ReadString();
        Server.clients[clientId].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }

        if (Server.clients[_fromClient].username != "Middleware")
        {
            Quaternion _rotation = _packet.readQuaternion();
            Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
        }
        else
        {
            int _toClient = _packet.ReadInt();
            Server.clients[_toClient].player.SetInput(_inputs);
        }
    }

    public static void RecievePlayerStatistics(int _fromCLient, Packet _packet)
    {
        int clientId = _packet.ReadInt();
        float burned_calories = _packet.ReadFloat();
        float traveled_meters = _packet.ReadFloat();
        int points = _packet.ReadInt();
        float finalTime = _packet.ReadFloat();
        int placement = _packet.ReadInt();
        PacketSend.SendPlayerStatisticsToAll(clientId, burned_calories, traveled_meters, points, finalTime, placement);
    }

    public static void RestartScene(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.controller.enabled = false;
        Server.clients[_fromClient].player.transform.position = new Vector3(Vector2.zero.x - 1.0f, Vector2.zero.y, 2f);
        PacketSend.RestartPlayerPosition(Server.clients[_fromClient].player);
        Debug.Log($"Player: { Server.clients[_fromClient].username} has been sent to starting position: " +
            $"{Server.clients[_fromClient].player.transform.position}");
        Server.clients[_fromClient].player.controller.enabled = true;
    }
}
