using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class TCPServer : MonoBehaviour
{

    private TcpListener _tcpListener;
    private List<TcpClient> Clients = new List<TcpClient>();
    public int port = 25000;
    public string ip = "192.168.0.103";


    public void Start()
    {
        Debug.Log("Server started");
        _tcpListener = new TcpListener(IPAddress.Any, port);
        _tcpListener.Start();
        Debug.Log("Server started on port " + port);

        // Start accepting clients
        _tcpListener.BeginAcceptTcpClient(OnClientConnect, null);
    }


    public void Update()
    {
        // Debug.Log("Server updated");
        ReceiveTCPConnexion();
    }

    private void OnApplicationQuit()
    {
        foreach (var client in Clients)
        {
            client.Close();
        }
        _tcpListener?.Stop();
    }


    private void OnClientConnect(IAsyncResult result)
    {
        TcpClient client = _tcpListener.EndAcceptTcpClient(result);
        int clientId = Clients.Count;
        Clients.Add(client);
        Debug.Log($"Client {clientId} connected.");

        // Await next connection
        _tcpListener.BeginAcceptTcpClient(OnClientConnect, null);
    }

    private void ReceiveTCPConnexion()
    {
        if (_tcpListener == null) { return; }

        while (_tcpListener.Pending())
        {
            TcpClient tcpClient = _tcpListener.AcceptTcpClient();
            Debug.Log("New connection received from: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address);
            Clients.Add(tcpClient);
        }

        foreach (TcpClient client in Clients)
        {
            if (!client.Connected)
            {
                Debug.Log("Client disconnected");
                Clients.Remove(client);
                return;
            }

            while (client.Available > 0)
            {
                byte[] data = new byte[client.Available];
                client.GetStream().Read(data, 0, client.Available);

                try
                {
                    Debug.Log("Client message: " + System.Text.Encoding.UTF8.GetString(data));
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning("Error receiving TCP message: " + ex.Message);
                }
            }
        }
    }
}
