using UnityEngine;

public class ReceiveTcpMessages : MonoBehaviour
{
    public TCPServer Serveur;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        string str = Serveur.ReceiveTCP();

        if (str != null){
              Serveur.BroadcastTCPMessage(str);
        }
       
    }
}
