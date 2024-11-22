using UnityEngine;
using System.Net.Sockets;

public class TCPServer : MonoBehaviour
{
    private TCPService tcpService;

    private void Start()
    {
        tcpService = gameObject.AddComponent<TCPService>();
        tcpService.OnMessageReceived += OnMessageReceived;

        if (tcpService.StartServer(Globals.HostPort))
        {
            Debug.Log("Server started.");
        }
        else
        {
            Debug.LogError("Failed to start server.");
        }
    }

    private void OnMessageReceived(string message, TcpClient sender)
    {
        Debug.Log("Received message from client: " + message);
    }

    private void OnApplicationQuit()
    {
        tcpService.StopService();
    }
}
