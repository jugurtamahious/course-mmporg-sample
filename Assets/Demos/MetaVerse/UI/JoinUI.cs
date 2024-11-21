using UnityEngine;

public class JoinUI : MonoBehaviour
{
    public TCPClient Client;
    public TMPro.TMP_InputField InpIp;
    public TMPro.TMP_InputField InpPort;
    public GameObject BtnConnect;
    public GameObject UI;
    public int port = 25000;
    public UDPSender udpSender;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConnect()
    {
        // Vérification si le port est un int
        if (!int.TryParse(InpPort.text, out this.port))
        {
            Debug.LogWarning("Invalid port: " + InpPort.text);
            return;
        }

        // Paramêtre pour la connexion TCP
        string ip = InpIp.text;
        int port = int.Parse(InpPort.text);

        // Connexion UDP
        udpSender.serverIP = ip;
        udpSender.serverPort = port + 1;


        Client.Connect(ip, port);
        UI.SetActive(false);
    }
}
