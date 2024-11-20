using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private TcpClient _tcpClient;

    private void Start()
    {

    }

    public void Connect(string ip, int port)
    {
        Debug.Log("Connecting to " + ip + ":" + port);
        _tcpClient = new TcpClient();
        _tcpClient.Connect(ip, port);
        Debug.Log("Connected to " + ip + ":" + port);
    }

    private void SendData(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        _tcpClient.GetStream().Write(data, 0, data.Length);
    }
}
