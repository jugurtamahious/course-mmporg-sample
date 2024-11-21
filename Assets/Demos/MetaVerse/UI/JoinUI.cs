using UnityEngine;

public class JoinUI : MonoBehaviour
{
    public TCPClient Client;
    public TMPro.TMP_InputField InpIp;
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

    public void OnConnect()
    {
        // Vérification si le port est un int
        if (!int.TryParse(InpPort.text, out this.port))
        {
            Debug.LogWarning("Invalid port: " + InpPort.text);
            return;
        }


        string ip = InpIp.text;
        int port = int.Parse(InpPort.text);
        Debug.Log(port);

        Client.Connect(ip, port);

        GameManager.HostIP = ip;
        GameManager.HostPort = port;

        UI.SetActive(false);
    }
}
