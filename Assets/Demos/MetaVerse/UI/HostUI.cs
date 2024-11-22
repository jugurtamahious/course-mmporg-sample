using Unity.VisualScripting;
using UnityEngine;

public class HostUI : MonoBehaviour
{
    public TCPServer Server;
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

    public void Connexion()
    {
        // Vérification si le port est un int
        if (!int.TryParse(InpPort.text, out port))
        {
            Debug.LogWarning("Invalid port: " + InpPort.text);
            return;
        }


        // Changement du port 
        Server.port = port;

        // Démarrage du server
        Debug.Log("Connexion");
        Server.StartServer();
        UI.SetActive(false);

        


    }
}
