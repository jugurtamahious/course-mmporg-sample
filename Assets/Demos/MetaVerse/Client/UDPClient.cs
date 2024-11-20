using System;
using System.Net;
using System.Net.Sockets;

public class UDPClient
{
    private UdpClient _udpClient;
    private IPEndPoint _serverEndPoint;

    public void Start(string serverIp, int serverPort)
    {
        _udpClient = new UdpClient();
        _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
    }

    public void SendPosition(float x, float y)
    {
        string message = $"{x},{y}";
        byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
        _udpClient.Send(data, data.Length, _serverEndPoint);
    }

    public void ReceivePositions()
    {
        _udpClient.BeginReceive(OnReceive, null);
    }

    private void OnReceive(IAsyncResult result)
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = _udpClient.EndReceive(result, ref serverEndPoint);

        // Process received positions
        _udpClient.BeginReceive(OnReceive, null);
    }
}
