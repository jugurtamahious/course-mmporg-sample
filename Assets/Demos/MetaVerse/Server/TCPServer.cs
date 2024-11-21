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
    public int port = GameManager.HostPort;


    public void Start()
    {

    }

    public void StartServer()
    {
        _tcpListener = new TcpListener(IPAddress.Any, port);
        _tcpListener.Start();
        Debug.Log("TCP Server started on port " + port);
    }

    public void Update()
    {
        string str = ReceiveTCP();

        if (str != null)
        {
            Debug.Log("Création du joueur : " + str);
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

            return clientAddress;
        }

        foreach (TcpClient client in Clients)
        {

            if (!client.Connected)
            {
                Debug.Log("Client disconnected");
                Clients.Remove(client);
                // Enlever son personnage
                continue;
            }
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
