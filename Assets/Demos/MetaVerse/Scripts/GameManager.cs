using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;

public class GameManager : MonoBehaviour
{
    public GameObject CharacterPrefab; // Le Prefab du personnage
    public Transform SpawnArea; // Le point de spawn
    public TCPService tcpService; // Service TCP pour gérer les connexions

    private Dictionary<string, GameObject> clientCharacters = new Dictionary<string, GameObject>();

    void Start()
    {
        // Démarrage du serveur TCP
        tcpService = gameObject.AddComponent<TCPService>();

        // Abonnement aux évenements
        tcpService.OnMessageReceived += OnMessageReceived;
        tcpService.OnClientConnected += OnNewClientConnected;

        tcpService.StartServer(Globals.HostPort);

        Debug.Log("GameManager démarré en mode serveur");

        // Mettre l'instance du joueur existant (serveur) dans le dictionnaire
        GameObject hostPlayer = GameObject.FindWithTag("localPlayer");

        if (hostPlayer != null)
        {
            clientCharacters[Globals.HostIP] = hostPlayer;
        }
        else
        {
            Debug.LogError("Aucun GameObject avec le tag 'hostPlayer' trouvé !");
        }
    }

    void Update()
    {
        // Envoie périodique des états aux clients
        BroadcastStateToClients();
    }

    private void BroadcastStateToClients()
    {
        // Exemple de données envoyées aux clients (à adapter selon le jeu)
        foreach (var client in clientCharacters)
        {
            string clientAddress = client.Key;
            GameObject character = client.Value;

            string stateMessage = $"{clientAddress}:{character.transform.position.x},{character.transform.position.y},{character.transform.position.z}";
            // tcpService.SendTCPMessage(stateMessage, null);
            // Debug.Log(stateMessage);
        }
    }

    // Gestion de l'événement lorsqu'un message est reçu via TCP
    private void OnMessageReceived(string message, TcpClient sender)
    {
        string clientAddress = ((System.Net.IPEndPoint)sender.Client.RemoteEndPoint).Address.ToString();

        if (message == "connect")
        {
            OnNewClientConnected(clientAddress);
        }
        else if (message == "disconnect")
        {
            RemoveClient(clientAddress);
        }
        else
        {
            Debug.Log($"Message reçu de {clientAddress} : {message}");
        }
    }

    // Gestion de l'événement lorsqu'un nouveau client se connecte
    public void OnNewClientConnected(string clientAddress)
    {
        if (clientAddress == Globals.HostIP)
        {
            Debug.LogWarning("Impossible de supprimer le joueur hôte");
            return;
        }
        Debug.Log($"Nouveau client connecté : {clientAddress}");

        // Supprimer toute instance existante pour ce client
        RemoveClient(clientAddress);

        // Instancier un nouveau personnage à l'endroit défini par SpawnArea
        if (CharacterPrefab != null && SpawnArea != null)
        {
            GameObject newCharacter = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);

            // Ajouter le personnage à la liste des personnages clients
            clientCharacters[clientAddress] = newCharacter;
            newCharacter.name = $"Character_{clientAddress}";

            Debug.Log($"Personnage créé pour le client {clientAddress}");
        }
        else
        {
            Debug.LogWarning("CharacterPrefab ou SpawnArea n'est pas défini.");
        }
    }


    // Gestion de la suppression d'un client
    public void RemoveClient(string clientAddress)
    {

        // Regarde si l'instance du joueur existe
        if (clientCharacters.TryGetValue(clientAddress, out GameObject character))
        {
            Destroy(character); // Supprimer l'instance du personnage
            clientCharacters.Remove(clientAddress);
            Debug.Log($"Personnage de {clientAddress} supprimé");
        }
        else
        {
            Debug.LogWarning($"Aucun personnage trouvé pour {clientAddress}");
        }
    }

    private void OnApplicationQuit()
    {
        // Arrêter le service TCP
        tcpService.StopService();
        tcpService.OnClientConnected -= OnNewClientConnected;

    }
}
