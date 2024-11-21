using UnityEngine;

public class ReceiveTcpMessages : MonoBehaviour
{
    public TCPServer tCPServer ;
    public GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        string str = tCPServer.ReceiveTCP();

        if (str != null){
              Debug.Log("Je créé un player");
              gameManager.OnNewClientConnected(str);
        }
       
    }
    
}
