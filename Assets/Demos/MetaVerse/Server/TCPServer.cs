using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class TCPServer : MonoBehaviour
{

    private TcpListener _tcpListener;
    private List<TcpClient> Clients = new List<TcpClient>();
    public GameManager gm;
    public int port = 25000;


    public void Start()
    {

    }

    public void StartServer()
    {
        _tcpListener = new TcpListener(IPAddress.Any, port);
        _tcpListener.Start();
        Debug.Log("TCP Server started on port " + port);

        // Start accepting clients
        // _tcpListener.BeginAcceptTcpClient(OnClientConnect, null);
    }

    public void Update()
    {
        string str = ReceiveTCP();

        if (str != null)
        {
            Debug.Log("Je créé un player");
            gm.OnNewClientConnected(str);
        }
    }

    private void OnApplicationQuit()
    {
        foreach (var client in Clients)
        {
            client.Close();
        }
        _tcpListener?.Stop();
    }


    // private void OnClientConnect(IAsyncResult result)
    // {
    //     TcpClient client = _tcpListener.EndAcceptTcpClient(result);
    //     int clientId = Clients.Count;
    //     Clients.Add(client);
    //     Debug.Log($"Client {clientId} connected.");

    //     // Await next connection
    //     _tcpListener.BeginAcceptTcpClient(OnClientConnect, null);
    // }

    public string ReceiveTCP()
    {
        if (_tcpListener == null)
        {
            return null;
        }

        while (_tcpListener.Pending())
        {
            TcpClient tcpClient = _tcpListener.AcceptTcpClient();
            string clientAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();
            Debug.Log("New connection received from: " + clientAddress);
            Clients.Add(tcpClient);
            return $"New connection received from: {clientAddress}";
        }

        foreach (TcpClient client in Clients)
        {

            if (!client.Connected)
            {
                Debug.Log("Client disconnected");
                Clients.Remove(client);
                continue;
            }

            // while (client.Available > 0) {   
            //     byte[] data = new byte[client.Available];
            //     client.GetStream().Read(data, 0, client.Available);

            //     try {
            //         return ParseString(data); 
            //     } catch (System.Exception ex) {
            //         Debug.LogWarning("Error receiving TCP message: " + ex.Message);
            //         return "false"; 
            //     }
            // }
        }

        return null;
    }

    public string ParseString(byte[] bytes)
    {
        string message = System.Text.Encoding.UTF8.GetString(bytes);
        // OnMessageReceive.Invoke(message);
        return message;
    }

}
