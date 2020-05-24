using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet) //read data from packet in the same order it was sent.
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

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
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.readQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
    }

    public static void RestartScene(int _fromClient, Packet _packet)
    {
        Server.clients[_fromClient].player.controller.enabled = false;
        Server.clients[_fromClient].player.transform.position = new Vector3(Vector2.zero.x - 1.0f, Vector2.zero.y, 2f);
        ServerSend.RestartPlayerPosition(Server.clients[_fromClient].player);
        Debug.Log($"Player: { Server.clients[_fromClient].username} has been sent to starting position: " +
            $"{Server.clients[_fromClient].player.transform.position}");
        Server.clients[_fromClient].player.controller.enabled = true;
    }
}
