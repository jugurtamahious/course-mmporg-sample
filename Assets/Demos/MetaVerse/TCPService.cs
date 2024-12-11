using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class TCPService : MonoBehaviour
{
    /** 
    * Gestion des variables privées
    */
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

    /**
    * Fonctions
    */

    // Vérification de la présence des joueurs
    public void Update()
    {
    }

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

    public void StopService()
    {
        if (Globals.IsServer)
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

    // Méthode pour afficher les clients connectés en temps réel (debug)
    public void DisplayConnectedClients()
    {
        StringBuilder clientList = new StringBuilder("Clients connectés: ");

        foreach (var client in clients)
        {
            string clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            clientList.Append(clientAddress + " ");
        }
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

                    // Notifier les abonnés de l'événement
                    OnClientRemoved?.Invoke(ip);

                    // Supprimer le client de la liste
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