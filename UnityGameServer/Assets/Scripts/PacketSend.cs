using System.Collections;
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

    public static void EnterLobby(int _toClient, string _msg) //send packet to a specific client
    {
        using (Packet _packet = new Packet((int)ServerPackets.enterLobby))
        {
            _packet.Write(_msg); //write the message to the packet
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void AssignMiddlewareToUser(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.assignMiddlewareToUser))
        {
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void StartMiddleware(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.startMiddleware))
        {
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void SendToLobby(int _clientId, string _playerName, string _league)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendToLobby))
        {
            _packet.Write(_clientId);
            _packet.Write(_playerName);
            _packet.Write(_league);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SendReadyState(int _clientId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendReadyState))
        {
            _packet.Write(_clientId);
            SendTCPDataToAll(_packet);
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

            if (Server.clients[_player.id].hasMiddleware)
            {
                SendUDPDataToAll(_packet);
            }
            else
            {
                SendUDPDataToAll(_player.id, _packet);
            }
        }
    }
    public static void PlayerHandleRotation(int playerId, Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHandleRotation))
        {
            //Debug.Log(_rotation);
            _packet.Write(_rotation);
            SendUDPData(playerId, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        if (Server.clients[_playerId].username != "Middleware")
        {
            using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
            {
                _packet.Write(_playerId);

                SendTCPDataToAll(_packet);
            }
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

    public static void SpeedUp(int playerId, bool speedUp)
    {
        using (Packet _packet = new Packet((int)ServerPackets.speedUp))
        {
            _packet.Write(playerId);
            _packet.Write(speedUp);
            SendTCPDataToAll(_packet);
        }
    }

    public static void UpdatePlayerSteps(int playerId, int steps)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updatePlayerSteps))
        {
            _packet.Write(playerId);
            _packet.Write(steps);
            SendTCPDataToAll(_packet);
        }
    }

    public static void UpdatePlayerLaps(int playerId, int laps)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updatePlayerLaps))
        {
            _packet.Write(playerId);
            _packet.Write(laps);
            SendTCPDataToAll(_packet);
        }
    }

    public static void UpdatePlayerPoints(Player player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updatePlayerPoints))
        {
            _packet.Write(player.id);
            _packet.Write(player.points);
            SendTCPDataToAll(_packet);
        }
    }

    public static void SendPlayerStatisticsToAll(int playerId, float burned_calories, float traveled_meters, int points, float finalTime, int placement)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendPlayerStatisticsToAll))
        {
            _packet.Write(playerId);
            _packet.Write(burned_calories);
            _packet.Write(traveled_meters);
            _packet.Write(points);
            _packet.Write(finalTime);
            _packet.Write(placement);
            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerCollidedWithOtherPlayer(float _speed, bool _collision)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerCollidedWithOtherPlayer))
        {
            _packet.Write(_speed);
            _packet.Write(_collision);
            SendTCPDataToAll(_packet);
        }
    }

    public static void ElementCollision(string _elementTag, Player _player, ElementCollision _element)
    {
        using (Packet _packet = new Packet((int)ServerPackets.elementCollision))
        {
            _packet.Write(_player.id);
            _packet.Write(_elementTag);
            _packet.Write(_player.collisions);
            //get players position for other players to listen to a cow muuu depending on the distance
            if (NetworkManager.instance.sceneName == "Vaquita")
            {
                _packet.Write(_player.controller.transform.position);
            }
            else
            {
                _packet.Write(_player.transform.position);
                _packet.Write(_element.transform.position);
            }
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

    public static void ObstacleSpawned2(int gameObjectId, Vector3 _position, Quaternion _rotation, string obstacleType, int playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.obstacleSpawned2))
        {
            _packet.Write(gameObjectId);
            _packet.Write(_position);
            _packet.Write(_rotation);
            _packet.Write(obstacleType);

            SendTCPData(playerId, _packet);
        }
    }

    public static void ObstacleMovement(int gameObjectId, Vector3 _position, Quaternion _rotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.obstacleMovement))
        {
            _packet.Write(gameObjectId);
            _packet.Write(_position);
            _packet.Write(_rotation);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerFinishedGame(int _playerId, float _speed)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerFinishedGame))
        {
            _packet.Write(_playerId);
            _packet.Write(_speed);
            SendTCPDataToAll(_packet);
        }
    }

    public static void ActivatePoitingArrowAndSendMessage(int _playerId, Vector3 lastGlassPosition, Quaternion lastGlassQuaternion, string message)
    {
        using (Packet _packet = new Packet((int)ServerPackets.activatePointingArrowAndSendMessage))
        {
            _packet.Write(_playerId);
            _packet.Write(lastGlassPosition);
            _packet.Write(lastGlassQuaternion);
            _packet.Write(message);
            SendTCPData(_playerId, _packet);
        }
    }

    public static void ShowAlertWithMessage(int _playerId, string message, bool state)
    {
        using (Packet _packet = new Packet((int)ServerPackets.showAlertWithMessage))
        {
            _packet.Write(_playerId);
            _packet.Write(message);
            _packet.Write(state);
            SendTCPData(_playerId, _packet);
        }
    }

    public static void DeleteArrow(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.deletePointingArrow))
        {
            _packet.Write(_playerId);
            SendTCPData(_playerId, _packet);
        }
    }

    public static void ActivateAlert(int _playerId, bool state)
    {
        using (Packet _packet = new Packet((int)ServerPackets.activateAlert))
        {
            _packet.Write(_playerId);
            _packet.Write(state);
            SendTCPData(_playerId, _packet);
        }
    }

    #endregion
}
