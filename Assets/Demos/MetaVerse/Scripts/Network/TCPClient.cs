using UnityEngine;
using System.Net.Sockets;

public class TCPClient : MonoBehaviour
{

    private TCPService tcpService;

    void Awake() {
        if (Globals.IsServer) {
            Destroy(this);
        }
    }

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

    void Update() { 
        // Supprimer les clients déconnectés
        tcpService.RemoveDisconnectedClients();

        // Réception des messages
        tcpService.ReceiveTCPMessages();

    }

    public void SendData(string message)
    {
        tcpService.SendTCPMessage(message);
    }

    private void OnMessageReceived(string message, TcpClient sender)
    {
        Debug.Log("Message received from server: " + message);
    }
}
