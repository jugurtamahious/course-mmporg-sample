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
        udpClient = new UdpClient(port);
        Invoke("ReceiveMessage", 0.1f);
    }

    void ReceiveMessage()
    {
        try 
        {
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpClient.Receive(ref remoteIP);
            string message = Encoding.UTF8.GetString(data);
            
            Debug.Log($"Message reçu : {message}");
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
        udpClient.Close();
    }
}