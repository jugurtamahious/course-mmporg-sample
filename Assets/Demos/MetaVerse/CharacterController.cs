using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;

public enum CharacterPlayer
{
    Player1,
    Player2
}

public class CharacterController : MonoBehaviour
{
    public CharacterPlayer Player = CharacterPlayer.Player1;
    public float WalkSpeed = 3;
    public float RotateSpeed = 250;

    Animator Anim;
    MetaverseInput inputs;
    InputAction PlayerAction;
    Rigidbody rb;

    // Historique des données
    private List<PlayerData> dataHistory = new List<PlayerData>();

    private string filePath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Anim = GetComponent<Animator>();
        inputs = new MetaverseInput();
        switch (Player)
        {
            case CharacterPlayer.Player1:
                PlayerAction = inputs.Player1.Move;
                break;
            case CharacterPlayer.Player2:
                PlayerAction = inputs.Player2.Move;
                break;
        }

        PlayerAction.Enable();

        rb = GetComponent<Rigidbody>();

        // Chemin du fichier JSON pour sauvegarder les données
        filePath = Path.Combine(Application.dataPath, "PlayerDataHistory.json");
        Debug.Log($"Data will be saved to: {filePath}");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 vec = PlayerAction.ReadValue<Vector2>();
        Anim.SetFloat("Walk", vec.y);

        // Mise à jour de la position
        Vector3 newPosition = rb.position + transform.forward * WalkSpeed * Time.fixedDeltaTime * vec.y;
        rb.MovePosition(newPosition);

        // Mise à jour de la rotation
        Quaternion newRotation = rb.rotation * Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime * vec.x, Vector3.up);
        rb.MoveRotation(newRotation);

        // Récupérer les données du joueur
        RetrievePlayerData(newPosition, vec.y, vec.x);
    }

    void RetrievePlayerData(Vector3 position, float walkValue, float rotateValue)
    {
        // Stockage des données
        PlayerData data = new PlayerData
        {
            PlayerID = Player.ToString(),
            Position = position,
            Animation = walkValue > 0 ? "Walking" : "Idle",
            Rotation = rotateValue,
            WalkSpeed = walkValue
        };

        // Ajouter à l'historique
        dataHistory.Add(data);
    }

    private void SavePositionHistoryToJson()
    {
        try
        {
            // Convertir l'historique en JSON
            string json = JsonUtility.ToJson(new PlayerDataList { Data = dataHistory }, true);
            
            // Sauvegarder dans le fichier
            File.WriteAllText(filePath, json);

            Debug.Log("Position history saved successfully to JSON!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save position history: {ex.Message}");
        }
    }

    void OnDisable()
    {
        PlayerAction.Disable();
        SavePositionHistoryToJson(); // Sauvegarde les données lors de la désactivation de l'objet
    }
}

// Classe pour structurer les données du joueur
[System.Serializable]
public class PlayerData
{
    public string PlayerID;
    public Vector3 Position;
    public string Animation;
    public float Rotation;
    public float WalkSpeed;
}

// Classe pour contenir une liste de données
[System.Serializable]
public class PlayerDataList
{
    public List<PlayerData> Data = new List<PlayerData>();
}
