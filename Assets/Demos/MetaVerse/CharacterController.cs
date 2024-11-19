using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;

public enum CharacterPlayer {
    Player1,
    Player2
}

public class CharacterController : MonoBehaviour
{
    public CharacterPlayer Player = CharacterPlayer.Player1;
    public float WalkSpeed = 3;
    public float RotateSpeed = 250;
    private Animator Anim;
    private MetaverseInput inputs;
    private InputAction PlayerAction;
    private Rigidbody rb;

    // Variables pour stocker les données
    private Vector3 currentPosition;
    private string currentAnimation;
    private float currentRotation;

    // Historique des données
    private List<Vector3> positionHistory = new List<Vector3>();
    private List<string> animationHistory = new List<string>();
    private List<float> rotationHistory = new List<float>();
    private List<float> walkHistory = new List<float>();

    private string filePath;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Anim = GetComponent<Animator>();
        inputs = new MetaverseInput();
        switch (Player) {
            case CharacterPlayer.Player1:
                PlayerAction = inputs.Player1.Move;
                break;
            case CharacterPlayer.Player2:
                PlayerAction = inputs.Player2.Move;
                break;
        }

        PlayerAction.Enable();

        rb = GetComponent<Rigidbody>();

        // Chemin du fichier pour sauvegarder les données
        filePath = Path.Combine(Application.dataPath, "PlayerDataHistory.txt");
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
        currentPosition = position;
        currentAnimation = walkValue > 0 ? "Walking" : "Idle";
        currentRotation = rotateValue;

        // Ajouter à l'historique
        positionHistory.Add(currentPosition);
        animationHistory.Add(currentAnimation);
        rotationHistory.Add(currentRotation);
        walkHistory.Add(walkValue);

        // Afficher les données dans la console
        Debug.Log(walkValue);
        Debug.Log($"Player: {Player}, Position: {currentPosition}, Animation: {currentAnimation}, Rotation: {currentRotation}");
    }

    private void SavePositionHistoryToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < positionHistory.Count; i++)
                {
                    string line = $"WalkingValue: {walkHistory[i]}, Position: {positionHistory[i]}, Animation: {animationHistory[i]}, Rotation: {rotationHistory[i]}";
                    writer.WriteLine(line);
                }
            }
            Debug.Log("Position history saved successfully!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save position history: {ex.Message}");
        }
    }

    void OnDisable()
    {
        PlayerAction.Disable();
        SavePositionHistoryToFile(); // Sauvegarde les données lors de la désactivation de l'objet
    }
}
