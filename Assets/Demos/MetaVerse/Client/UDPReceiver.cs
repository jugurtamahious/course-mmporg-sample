using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UDPReceiver : MonoBehaviour
{
    public int port = 25000;
    private UdpClient udpClient;

    void Start()
    {
        StartServer();
    }

    public void StartServer()
    {
        udpClient = new UdpClient(port);
        Invoke("ReceiveMessage", 0.1f);
    }

    public void ReceiveMessage()
    {
        try 
        {
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 25004);
            byte[] data = udpClient.Receive(ref remoteIP);
            string message = Encoding.UTF8.GetString(data);
            
            Debug.Log($"Message reçu : {message}");
            Debug.Log($"Données reçues depuis : {remoteIP.Address}:{remoteIP.Port}");

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur UDP : {e.Message}");
        }

        // Continue à écouter
        Invoke("ReceiveMessage", 0.1f);
    }

    void OnApplicationQuit()
    {
        // udpClient.Close();
    }
}