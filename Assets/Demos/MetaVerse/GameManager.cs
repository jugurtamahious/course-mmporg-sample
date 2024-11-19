using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TCPServer Server; // Référence au serveur
    public GameObject CharacterPrefab; // Le Prefab du personnage
    public Transform SpawnArea; // Le point de spawn

    void Start()
    {
        // S'abonner à l'événement de connexion d'un nouveau client
        Server.OnClientConnected += OnNewClientConnected;
    }

    void OnDestroy()
    {
        // Désabonner pour éviter les erreurs
        if (Server != null)
            Server.OnClientConnected -= OnNewClientConnected;
    }

    // Gestion de l'événement lorsqu'un nouveau client se connecte
    private void OnNewClientConnected(string clientAddress)
    {
        Debug.Log($"Nouveau client connecté : {clientAddress}");

        // Instancier un nouveau personnage à l'endroit défini par SpawnArea
        if (CharacterPrefab != null && SpawnArea != null)
        {
            GameObject newCharacter = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);
            newCharacter.name = $"Character_{clientAddress}";
            
            Debug.Log("Je suis crée");
        }
    }
}
