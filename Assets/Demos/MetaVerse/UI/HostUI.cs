using Unity.VisualScripting;
using UnityEngine;
using System.Net;

public class HostUI : MonoBehaviour
{
    public TCPServer Server;
    // public UDPSender UDPSender;
    public TMPro.TMP_InputField InpPort;
    public GameObject BtnConnect;
    public GameObject UI;
    public GameManager GameManager;
    public int port = 25000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Connexion()
    {
        // Vérification si le port est un int
        if (!int.TryParse(InpPort.text, out port))
        {
            Debug.LogWarning("Invalid port: " + InpPort.text);
            return;
        }


        // Changement du port du serveur
        Server.port = port;

        string localIP = GetLocalIPAddress();


        GameManager.HostPort = port;
        GameManager.HostIP = localIP;

        // Démarrage du server
        Server.StartServer();
        UI.SetActive(false);
    }

    private string GetLocalIPAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "Aucune adresse IPv4 trouvée.";
        }
        catch (System.Exception ex)
        {
            return $"Erreur : {ex.Message}";
        }
    }
}
