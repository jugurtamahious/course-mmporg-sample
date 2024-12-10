using UnityEngine;
using System.Collections.Generic;
using System.Net.Sockets;

public class GameManager : MonoBehaviour
{
    public GameObject CharacterPrefab; // Le Prefab du personnage
    public Transform SpawnArea; // Le point de spawn
    private Dictionary<string, GameObject> clientCharacters = new Dictionary<string, GameObject>();

    void Start()
    {
        Debug.Log("GameManager est instanciée");

        // Mettre l'instance du joueur existant (local) dans le dictionnaire
        GameObject localPlayer = GameObject.FindWithTag("localPlayer");


        if (localPlayer != null)
        {   
            // Server
            clientCharacters[Globals.GetLocalIPAddress()] = localPlayer;

            Debug.Log("IP DU JOUEUR : " + Globals.GetLocalIPAddress());
        }
        else
        {
            Debug.LogError("Aucun GameObject avec le tag 'hostPlayer' trouvé !");
        }
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
        SpawnClient(clientAddress);
       
    }

    public void SpawnClient(string clientAddress){
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
    public void OnRemoveClient(string clientAddress)
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
}