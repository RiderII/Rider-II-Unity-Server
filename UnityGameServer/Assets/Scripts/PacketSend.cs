﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketSend
{
    private static void SendTCPData(int _toClient, Packet _packet) //will prepare the packet to be sent
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet) //send packet to all clients
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPLayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet) //send packet to all clients except one
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPLayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    private static void SendUDPDataToAll(Packet _packet) //send packet to all clients
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPLayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet) //send packet to all clients except one
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPLayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets

    public static void Welcome(int _toClient, string _msg) //send packet to a specific client
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg); //write the message to the packet
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void RestartPlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.restartPlayerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerCollided(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerCollided))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.collisions);
            //get players position for other players to listen to a cow muuu depending on the distance
            _packet.Write(_player.controller.transform.position); 

            SendTCPDataToAll(_packet);
        }
    }

    public static void ObstacleSpawned(Vector3 _position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.obstacleSpawned))
        {
            _packet.Write(_position);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerFinishedGame(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerFinishedGame))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    #endregion
}