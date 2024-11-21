using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TCPServer Server; // Référence au serveur
    public GameObject CharacterPrefab; // Le Prefab du personnage
    public Transform SpawnArea; // Le point de spawn

    // Gestion de l'événement lorsqu'un nouveau client se connecte
    public void OnNewClientConnected(string clientAddress)
    {
        Debug.Log($"Nouveau client connecté : {clientAddress}");

        // Instancier un nouveau personnage à l'endroit défini par SpawnArea
        if (CharacterPrefab != null && SpawnArea != null)
        {
            GameObject newCharacter = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);
            newCharacter.name = $"Character_{clientAddress}";

            CharacterController c = newCharacter.GetComponent<CharacterController>();
            c.enabled = false;

            
            
            Debug.Log("Je suis crée");
        }
    }
}
