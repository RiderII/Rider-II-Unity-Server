using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public static int dataBufferSize = 4096;

    public int id;
    public string username;
    public string lobbyState = "Pendiente";
    public Player player;
    public TCP tcp; // reference to its tcp class
    public UDP udp;

    public Client(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP //here we will store the instance that we get in the server connects callback
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;

        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _socket) //once the server gets an incoming connection it sends a welcome message
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream(); //returns a NetworkStream to send and receive data

            receivedData = new Packet();

            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            // send player to the lobby
            PacketSend.EnterLobby(id, "Joining the lobby!");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {id} via TCP {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result); //receive data (return the number of bytes we read from the stream)
                if (_byteLength <= 0)
                {
                    // Disconnect
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                // handle data
                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null); //continue to reading data from the stream
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                // Handle disconnect
                Server.clients[id].Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;

            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });

                _packetLength = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect() //properly disconnect clients
        {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;


        }
    }

    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }

        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endPoint, _packet);
        }

        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet);
                }
            });
        }

        public void Disconnect() //properly disconnect clients
        {
            endPoint = null;
        }
    }

    public void SendIntoLobby(string _league)
    {
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.tcp.socket != null && _client.username != "Middleware")
            {
                PacketSend.SendToLobby(_client.id, _client.username, _league); //Send new player entered in the lobby to all
            }
        }
    }

    public void SendIntoGame(string _playerName, int _playerId = 0)
    {
        int lastPlayerInserverIndex = 0;
        float farRight = NetworkManager.instance.sceneName == "Vaquita" ? -1.0f : 0f;
        int lasPlayerId = 0;
        player = NetworkManager.instance.InstantiatePlayer(); //assign a value to our player field

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                lastPlayerInserverIndex += 1;
                if ((_client.id != id && _client.player.controller  && (_client.player.controller.center.x >= farRight)) ||
                    (_client.id != id && (_client.player.transform.position.x <= farRight)))
                {
                    if (NetworkManager.instance.sceneName == "Vaquita")
                    {
                        farRight = _client.player.controller.center.x;
                    }
                    else
                    {
                        farRight = _client.player.transform.position.x;
                    }
                    
                    lasPlayerId = _client.id;
                }
            }
        }

        if (lastPlayerInserverIndex > 1)
        {
            float position;

            if (Server.clients[lasPlayerId].player.controller)
            {
                position = Server.clients[lasPlayerId].player.controller.center.x; //we can dinamically spwan player based on previous players
                player.Initialize(id, _playerName, position + 2.0f, player); //positions player = new Player(id, _playerName, new Vector3(position + 1.5f, 0, 0));
            }
            else
            {
                position = Server.clients[lasPlayerId].player.transform.position.x;
                player.Initialize(id, _playerName, position - 2.0f, player);
            }
        }
        else
        {
            player.Initialize(id, _playerName, player.transform.position.x, player);
            NetworkManager.instance.StartGameManager();
        }

        SendPlayerPosition();
    }

    private void SendPlayerPosition()
    {
        //send information of all other players already connected to this new player

        foreach (Client _client in Server.clients.Values) // Recibo la posicion de los jugadores ya conectados
        {
            if (_client.player != null)
            {
                if (_client.id != id)
                {
                    PacketSend.SpawnPlayer(id, _client.player);
                }
            }
        }

        //Send the new player information to all other players as well as to himself

        foreach (Client _client in Server.clients.Values) // A cada cliente le envio mi posicion
        {
            if (_client.player != null)
            {
                PacketSend.SpawnPlayer(_client.id, player);
            }
        }
    }

    private void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint + " " + username} has disconnected.");

        PacketSend.PlayerDisconnected(id);

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject); //has to be destroyed in the main thread
            player = null;
        });

        tcp.Disconnect();
        udp.Disconnect();

        NetworkManager.instance.sceneName = "";
        NetworkManager.instance.verifyDisconnection = true;
    }
}
