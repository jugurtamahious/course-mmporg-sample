using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    /* Variables Publiques */
    public GameObject CharacterPrefab;  // Le Prefab du personnage
    public Transform SpawnArea;         // Le point de spawn

    public UDPServer udpServer;

    /* Variables Privées */
    private TCPService tcpService;      // Service TCP pour gérer les connexions
    private ScoreManager scoreManager;



    /* Méthodes Unity */

    void Start()
    {
        // Démarrage du serveur TCP
        tcpService = gameObject.AddComponent<TCPService>();

        // Abonnement aux évenements
        tcpService.OnClientConnected += OnNewClientConnected;
        tcpService.OnClientRemoved += OnRemoveClient;

        Debug.Log("GameManager démarré en mode serveur");

        // Find the ScoreManager instance
        // scoreManager = FindObjectOfType<ScoreManager>();
        // if (scoreManager != null)
        // {
        //     scoreManager.UpdatePlayerList();
        // }
    }


    void Update()
    {

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
        OnRemoveClient(clientAddress);
        // SpawnClient(clientAddress);
    }
    

    // Gestion de la création d'un nouveau client
    public void SpawnClient(string clientAddress)
    {
        // Instancier un nouveau personnage à l'endroit défini par SpawnArea
        if (CharacterPrefab != null && SpawnArea != null)
        {
            GameObject newCharacter = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);

            // Ajouter le personnage à la liste des personnages clients
            newCharacter.name = $"Character_{clientAddress}";
            Debug.Log($"Personnage créé pour le client {clientAddress}");

            // Update the ScoreManager with the new list of players
            if (scoreManager != null)
            {
                scoreManager.UpdatePlayerList();
            }
        }
        else
        {
            Debug.LogWarning("CharacterPrefab ou SpawnArea n'est pas défini.");
        }
    }

    // Gestion de la suppression d'un client
    public void OnRemoveClient(string clientAddress)
    {

        GameObject prefab = udpServer.getPrefab(clientAddress);

        Debug.Log("Removing client: " + clientAddress);

        // Regarde si l'instance du joueur existe
        if (prefab != null)
        {
            udpServer.RemoveClient(clientAddress);
            Debug.Log($"Personnage de {clientAddress} supprimé");

            // Update the ScoreManager with the new list of players
            if (scoreManager != null)
            {
                scoreManager.UpdatePlayerList();
            }
        }
        else
        {
            Debug.LogWarning($"Aucun personnage trouvé pour {clientAddress}");
        }
    }

}
