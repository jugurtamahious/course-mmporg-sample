using UnityEngine;
using System.Net.Sockets;

public class TCPServer : MonoBehaviour
{

    private TcpListener _tcpListener;
    private List<TcpClient> Clients = new List<TcpClient>();
    public GameManager gm;
    public int port = GameManager.HostPort;
    
    void Awake() {
        // Desactiver mon objet si je ne suis pas le serveur
        if (!Globals.IsServer) {
            gameObject.SetActive(false);
        }
    }
    
    public void Start()
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
