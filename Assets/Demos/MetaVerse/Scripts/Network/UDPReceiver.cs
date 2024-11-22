using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class UDPReceiver : MonoBehaviour
{
    // public int port = 25004;
    private UdpClient udpClient;

    void Start()
    {
        StartServer();
    }

    public void StartServer()
    {
        // udpClient = new UdpClient(port);
        // Debug.Log($"Serveur UDP démarré sur le port {port}");
        // StartListening();
    }

    public void StartListening()
    {
        udpClient.BeginReceive(OnReceive, null);
    }

    private void OnReceive(IAsyncResult result)
    {
        try
        {
            IPEndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpClient.EndReceive(result, ref remoteIP);
            string message = Encoding.UTF8.GetString(data);

            Debug.Log($"Message reçu : {message}");
            Debug.Log($"Données reçues depuis : {remoteIP.Address}:{remoteIP.Port}");

            // Recommence à écouter
            StartListening();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur UDP : {e.Message}");
        }
    }

    void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}
