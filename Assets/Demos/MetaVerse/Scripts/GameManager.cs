using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public TCPServer Server; // Référence au serveur
    public GameObject CharacterPrefab; // Le Prefab du personnage
    public Transform SpawnArea; // Le point de spawn
    public UDPReceiver Receiver; // Référence au récepteur UDP
    public static string HostIP;
    public static int HostPort;

    private Dictionary<string, GameObject> clientCharacters = new Dictionary<string, GameObject>();

    void Update()
    {
        // Récupérer les données UDP reçues

        // Mise à jour du state

        // Debug.Log(clientCharacters.Keys.ToString());

        // Envoie du state aux clients

    }

    // Gestion de l'événement lorsque le serveur démarre
    public void OnServerStarted()
    {
        Debug.Log("GameManager démarré");

        // Mettre l'instance du joueur existant dans le serveur
        GameObject engineer = GameObject.FindWithTag("hostPlayer");

        if (engineer != null)
        {
            clientCharacters[GameManager.HostIP] = engineer;
        }
        else
        {
            Debug.LogError("Aucun GameObject avec le tag 'Engineer' trouvé !");
        }

    }


    // Gestion de l'événement lorsqu'un nouveau client se connecte
    public void OnNewClientConnected(string clientAddress)
    {
        RemoveClient(clientAddress);
        Debug.Log($"Nouveau client connecté : {clientAddress}");

        // Instancier un nouveau personnage à l'endroit défini par SpawnArea
        if (CharacterPrefab != null && SpawnArea != null)
        {
            // Instancier le personnage
            GameObject newCharacter = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);

            // Ajouter le personnage à la liste des personnages clients
            clientCharacters[clientAddress] = newCharacter;
            newCharacter.name = $"Character_{clientAddress}";

        }
    }

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
}
