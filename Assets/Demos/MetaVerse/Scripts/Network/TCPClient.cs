using UnityEngine;

public class TCPClient : MonoBehaviour
{
    private TCPService tcpService;

    void Awake() {
        // Desactiver mon objet si je ne suis pas le serveur
        if (Globals.IsServer) {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        tcpService = gameObject.AddComponent<TCPService>();

        if (tcpService.ConnectToServer(Globals.HostIP, Globals.HostPort))
        {
            Debug.Log("Connected to server.");
        }
        else
        {
            Debug.LogError("Failed to connect to server.");
        }
    }
}