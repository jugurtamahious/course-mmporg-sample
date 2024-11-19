using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPServer: MonoBehaviour
{
    private UdpClient _udpServer;
    public int port = 25000;

    public void Start()
    {
        _udpServer = new UdpClient(port);
        _udpServer.BeginReceive(OnReceive, null);
        Debug.Log("UDP Server listening on port " + port);
    }

    private void OnReceive(IAsyncResult result)
    {
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = _udpServer.EndReceive(result, ref clientEndPoint);
        
        // Process data (e.g., update player position)
        _udpServer.BeginReceive(OnReceive, null);
    }

    public void SendData(byte[] data, IPEndPoint clientEndPoint)
    {
        _udpServer.Send(data, data.Length, clientEndPoint);
    }
}
