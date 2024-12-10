using System.Net;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
    public UDPService UDPService;
    public int ServerPort = 25000;

    public GameManager gameManager;
    private IPEndPoint ServerEndpoint;

    void Awake()
    {
        // Desactiver mon objet si je ne suis pas le client
        if (Globals.IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UDPService.InitClient();

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(Globals.HostIP), Globals.HostPort);

        UDPService.OnMessageReceived += OnMessageReceived;

    }

    private void OnMessageReceived(string message, IPEndPoint sender)
    {
        Debug.Log("[CLIENT] Message received from " +
                        sender.Address.ToString() + ":" + sender.Port
                        + " =>" + message);
    }

    public void sendMesageToServer(string message)
    {
        // Envoie des positions au serveur
        // UDPService.SendUDPMessage(message, ServerEndpoint);
    }
}