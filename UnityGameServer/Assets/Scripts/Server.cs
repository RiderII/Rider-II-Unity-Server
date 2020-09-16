using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int MaxPLayers { get; private set; }
    public static int Port { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    //the server needs a way to decide which packet method to call based on the packet id that it receives
    public delegate void Packethandler(int _fromClient, Packet _packet);
    public static Dictionary<int, Packethandler> packetHandlers;

    private static TcpListener tcpListener;
    private static UdpClient udpListener;

    public static void Start(int _maxPlayers, int _port)
    {
        MaxPLayers = _maxPlayers;
        Port = _port;

        Debug.Log("Starting Server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); // Returns and IAsuncResult (creates and connects socket)

        udpListener = new UdpClient(_port);
        udpListener.BeginReceive(UDPReceiveCallback, null);

        Debug.Log($"Server started on {Port}.");
    }

    private static void TCPConnectCallback(IAsyncResult _result)
    {
        // Ending async request
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result); // Asynchronously accepts an incoming connection attempt and creates a new TcpClient to handle remote host communication.
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); // keep listeting for new connections
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPLayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }

        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    private static void UDPReceiveCallback(IAsyncResult _result)
    {
        try
        {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //returns bytes we receive and set our IPEndpoint to endpoint where the data came from
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null); // don't miss any incoming data

            if (_data.Length < 4) //we might have lost a packet
            {
                return;
            }

            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }

                if (clients[_clientId].udp.endPoint == null) // this is a new connection and the packet received is a new one
                                                             //that opens the clients port
                {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }

                //check if the endPoint which we have store our client matches the endpoint where the packet comes from (security)
                // a hacker might send an id which is not theirs and try to have control over movement
                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    clients[_clientId].udp.HandleData(_packet);
                }
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error receiving UDP data: {_ex}");
        }
    }

    public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
    {
        try
        {
            if (_clientEndPoint != null)
            {
                udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
            }
        }
        catch (Exception _ex)
        {
            Debug.Log($"Error sending data to {_clientEndPoint} via UDP: {_ex}");
        }
    }

    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPLayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, Packethandler>()
        {
            { (int)ClientPackets.requestEnteredLobby, PacketHandle.RequestEnterLobby },
            { (int)ClientPackets.sendReadyState, PacketHandle.SendReadyState },
            { (int)ClientPackets.sendToGame, PacketHandle.SendToGame },
            { (int)ClientPackets.playerMovement, PacketHandle.PlayerMovement },
            { (int)ClientPackets.restartScene, PacketHandle.RestartScene },
            { (int)ClientPackets.sendPlayerStatistics, PacketHandle.RecievePlayerStatistics },
            { (int)ClientPackets.startMiddleware, PacketHandle.StartMiddleware },
        };
        Debug.Log("Packets initialized");
    }

    public static void Stop()
    {
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }

        if (udpListener != null)
        {
            udpListener.Close();
        }
    }
}
