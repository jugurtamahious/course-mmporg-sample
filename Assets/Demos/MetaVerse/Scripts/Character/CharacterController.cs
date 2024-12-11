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
            isLocalPlayer = false;
            enabled = false; 
        }

        udpServer = GameObject.FindFirstObjectByType<UDPServer>();
        udpClient = GameObject.FindFirstObjectByType<UDPClient>();
    }

    private void Start()
    {
        if (!isLocalPlayer) return; 

        Anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Configurer les contrôles pour un joueur local
        inputs = new MetaverseInput();
        PlayerAction = inputs.Player1.Move;
        PlayerAction.Enable();

        // Générer un identifiant unique pour chaque joueur
        playerID = "Player" + UnityEngine.Random.Range(1000, 9999).ToString();
        Globals.playerID = playerID;
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return; 
 
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
        AnimatorStateInfo currentState = Anim.GetCurrentAnimatorStateInfo(0);
            string animationName = currentState.IsName("Idle") ? "Idle" :
                                   currentState.IsName("Walk") ? "Walk" : "Other";

            CharacterUpdate update = new CharacterUpdate
            {
                playerID = Globals.playerID,
                position = transform.position,
                rotation = transform.rotation,
                animation = animationName
            };

            string message = JsonUtility.ToJson(update);
            udpClient.sendMesageToServer(message);
    }

    public void SetLocalPlayer(bool isLocal)
    {
        isLocalPlayer = isLocal;
    }

     [System.Serializable]
    public class CharacterUpdate
    {
        public string playerID;
        public Vector3 position;
        public Quaternion rotation;
        public string animation;
    }
}
