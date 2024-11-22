using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinUI : MonoBehaviour
{
    public TMPro.TMP_InputField InpIp;
    public TMPro.TMP_InputField InpPort;
    public GameObject BtnConnect;
    public GameObject UI;
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

        Globals.HostIP = ip;
        Globals.HostPort = port;
        SetRole(false);
        StartGame();

    }

    public void SetRole(bool isServer)
    {
        Globals.IsServer = isServer;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MetaVerse");
    }
}
