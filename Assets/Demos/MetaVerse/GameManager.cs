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

    void Update() {
        // Récupérer les données UDP reçues
        
    }

    // Gestion de l'événement lorsqu'un nouveau client se connecte
    public void OnNewClientConnected(string clientAddress)
    {
        string newCharacterName = $"Character_{clientAddress}";
        Debug.Log($"Nouveau client connecté : {clientAddress}");

        // Instancier un nouveau personnage à l'endroit défini par SpawnArea
        if (CharacterPrefab != null && SpawnArea != null)
        {
            GameObject newCharacter = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);
            clientCharacters[clientAddress] = newCharacter;

            newCharacter.name = $"Character_{clientAddress}";

        }
    }

    public void RemoveClient(string clientAddress)
    {
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
