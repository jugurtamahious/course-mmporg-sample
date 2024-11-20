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

    void OnDisable()
    {
        PlayerAction.Disable();
    }
}
