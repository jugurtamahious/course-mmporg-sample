using UnityEngine;
using UnityEngine.InputSystem;
using System.Net;

[System.Serializable]
public class CharacterUpdate
{
    public string playerID; // Identifiant unique du joueur
    public Vector3 position; // Position du joueur
}

public class CharacterController : MonoBehaviour
{
    public float WalkSpeed = 3f;
    public float RotateSpeed = 250f;
    public UDPServer udpServer; // Référence au serveur UDP
    public UDPClient udpClient; // Référence au client UDP

    private Animator Anim;
    private MetaverseInput inputs;
    private InputAction PlayerAction;
    private Rigidbody rb;

    private bool isLocalPlayer = true;

    private string playerID;

    private void Awake()
    {
        if (Globals.IsServer)
        {
            isLocalPlayer = false; // Le serveur ne contrôle pas de personnage
            enabled = false; // Désactive ce script sur le serveur
        }
    }

    private void Start()
    {
        if (!isLocalPlayer) return; // Ne rien faire pour le serveur

        Anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Configurer les contrôles pour un joueur local
        inputs = new MetaverseInput();
        PlayerAction = inputs.Player1.Move;
        PlayerAction.Enable();

        // Générer un identifiant unique pour chaque joueur
        playerID = "Player" + UnityEngine.Random.Range(1000, 9999).ToString();
        Debug.Log("Player ID: " + playerID);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return; // Ne rien faire pour le serveur

        // Gestion des mouvements du joueur local
        HandleLocalMovement();
        SendPositionToServer();
    }

    private void HandleLocalMovement()
    {
        Vector2 movementInput = PlayerAction.ReadValue<Vector2>();
        Anim.SetFloat("Walk", movementInput.y);

        // Déplacement
        Vector3 newPosition = rb.position + transform.forward * WalkSpeed * Time.fixedDeltaTime * movementInput.y;
        rb.MovePosition(newPosition);

        // Rotation
        Quaternion newRotation = rb.rotation * Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime * movementInput.x, Vector3.up);
        rb.MoveRotation(newRotation);
    }

    private void SendPositionToServer()
    {
        CharacterUpdate update = new CharacterUpdate
        {
            playerID = playerID,
            position = transform.position
        };
        

        string message = JsonUtility.ToJson(update);
        udpClient.sendMesageToServer(message); // Envoyer au serveur uniquement si udpClient est configuré
    }

    public void SetLocalPlayer(bool isLocal)
    {
        isLocalPlayer = isLocal;
    }
}
