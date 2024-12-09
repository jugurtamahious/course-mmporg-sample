using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class TCPService : MonoBehaviour
{
    private bool isServer = Globals.IsServer;
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private List<TcpClient> clients = new List<TcpClient>();

    /**
    * Gestion des variables publiques
    */
    public GameManager GameManager;

    /**
    * Définition des événements
    */

    public delegate void ClientConnectedHandler(string clientAddress);
    public event ClientConnectedHandler OnClientConnected;

    public delegate void TCPMessageReceived(string message, TcpClient sender);
    public event TCPMessageReceived OnMessageReceived;

    public delegate void RemoveClient(string ip);
    public event RemoveClient OnClientRemoved;

    public void Update()
    {
        RemoveDisconnectedClients();
    }

    // Démarrer le serveur
    public bool StartServer(int port)
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            isServer = true;
            Debug.Log("TCP Server started on port: " + port);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed to start server: " + ex.Message);
            return false;
        }
    }

    // Arrêter le serveur ou le client
    public void StopService()
    {
        if (isServer)
        {
            foreach (var client in clients)
            {
                client.Close();
            }
            tcpListener?.Stop();
            clients.Clear();
            Debug.Log("Server stopped.");
        }
        else
        {
            networkStream?.Close();
            tcpClient?.Close();
            tcpClient = null;
            Debug.Log("Client disconnected.");
        }
    }

    // Connexion à un serveur en tant que client
    public bool ConnectToServer(string ip, int port)
    {
        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ip, port);
            networkStream = tcpClient.GetStream();
            isServer = false;
            Debug.Log("Connected to server at " + ip + ":" + port);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Failed to connect to server: " + ex.Message);
            return false;
        }
    }

    // Envoi de message (client ou serveur)
    public void SendTCPMessage(string message, TcpClient specificClient = null)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            if (isServer)
            {
                // Envoi à un client spécifique ou à tous les clients
                if (specificClient != null)
                {
                    specificClient.GetStream().Write(data, 0, data.Length);
                }
                else
                {
                    foreach (var client in clients)
                    {
                        client.GetStream().Write(data, 0, data.Length);
                    }
                }
            }
            else
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    networkStream.Write(data, 0, data.Length);
                }
                else
                {
                    Debug.LogWarning("Client not connected to any server.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error sending message: " + ex.Message);
        }
    }

    // Acceptation des nouveaux clients (serveur uniquement)
    public void AcceptClients()
    {
        if (tcpListener == null || !tcpListener.Pending()) return;

        try
        {
            TcpClient newClient = tcpListener.AcceptTcpClient();
            clients.Add(newClient);
            string clientAddress = ((IPEndPoint)newClient.Client.RemoteEndPoint).Address.ToString();

            Debug.Log("New client connected: " + clientAddress);

            // Notifier les abonnés de l'événement
            OnClientConnected?.Invoke(clientAddress);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error accepting client: " + ex.Message);
        }
    }

    // Réception des messages (serveur ou client)
    public void ReceiveTCPMessages()
    {
        try
        {
            if (isServer)
            {
                // Serveur : écouter chaque client
                foreach (var client in new List<TcpClient>(clients))
                {
                    if (client.Available > 0)
                    {
                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        OnMessageReceived?.Invoke(message, client);
                    }
                }
            }
            else
            {
                // Client : écouter uniquement le serveur
                if (tcpClient != null && networkStream != null && networkStream.DataAvailable)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    OnMessageReceived?.Invoke(message, tcpClient);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error receiving message: " + ex.Message);
        }
    }

    // Méthode pour afficher les clients connectés en temps réel
    public void DisplayConnectedClients()
    {
        StringBuilder clientList = new StringBuilder("Clients connectés: ");

        foreach (var client in clients)
        {
            string clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            clientList.Append(clientAddress + " ");
        }
        // Debug.Log(GetClients());
        // Debug.Log(clients.ToString() + " | Nombre de clients: " + clients.Count);
    }

    // Méthode pour supprimer les clients déconnectés de la liste
    // Vérifier l'état des clients et supprimer ceux qui sont déconnectés
    public void RemoveDisconnectedClients()
    {
        for (int i = clients.Count - 1; i >= 0; i--)
        {
            TcpClient client = clients[i];

            try
            {
                // Vérifier si le client est toujours connecté
                if (client.Client.Poll(0, SelectMode.SelectRead) && client.Available == 0)
                {
                    // Le client est déconnecté
                    string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Debug.Log("Client déconnecté : " + ip);

                    // Notifier les abonnés de l'événement
                    OnClientRemoved?.Invoke(ip);

                    Debug.Log("Client déconnecté 3 : " + ip);

                    clients.RemoveAt(i);
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Erreur lors de la vérification d'un client : {ex.Message}");
                clients.RemoveAt(i);
            }
        }
    }

    public string GetClients() {
        if (clients.Count == 0) {
            return "Aucun client connecté";
        }
        string str = "";

        foreach (var client in clients) {
            string clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            str += clientAddress + ", ";
        }

        str += "HostIP : " + Globals.HostIP;

        return str;
    }
}