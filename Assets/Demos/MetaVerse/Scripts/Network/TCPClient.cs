using UnityEngine;
using System.Net.Sockets;

public class TCPClient : MonoBehaviour
{

    public GameManager GameManager;
    private TCPService tcpService;

    private void Start()
    {
        tcpService = gameObject.AddComponent<TCPService>();
        tcpService.OnMessageReceived += OnMessageReceived;

        if (tcpService.ConnectToServer(Globals.HostIP, Globals.HostPort))
        {
            // Debug.Log("Connected to server.");
        }
        else
        {
            Debug.LogError("Failed to connect to server.");
        }
    }

    public void SendData(string message)
    {
        tcpService.SendTCPMessage(message);
    }

    private void OnMessageReceived(string message, TcpClient sender)
    {
        Debug.Log("Message received from server: " + message);
    }

    private void OnApplicationQuit()
    {
        tcpService.StopService();
    }
}
