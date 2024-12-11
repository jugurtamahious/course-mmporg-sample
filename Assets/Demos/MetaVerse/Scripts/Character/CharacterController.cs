using UnityEngine;
using UnityEngine.InputSystem;

public enum MessageType
{
    CharacterUpdate,
    CarPositionUpdate
}
[System.Serializable]
public class CarSyncUpdate : BaseMessage
{
    public string carID;
    public float animationTime;
}
[System.Serializable]
public class BaseMessage
{
    public MessageType messageType;
}

[System.Serializable]
public class CharacterUpdate : BaseMessage
{
    public string playerID;
    public Vector3 position;
    public Quaternion rotation;
    public string animation;
}

public class CharacterController : MonoBehaviour
{

    /* Variables Publiques */
    public float WalkSpeed = 3f;
    public float RotateSpeed = 250f;
    public UDPClient udpClient;
    public UDPServer udpServer;


    /* Variables Privées */
    private Animator Anim;
    private MetaverseInput inputs;
    private InputAction PlayerAction;
    private Rigidbody rb;

    private bool isLocalPlayer = true;

    private string playerID;

    /* Méthodes Unity */

    // On détruit l'objet si ce n'est pas le serveur
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
        // Ne continue pas l'initialisation si je ne suis pas le joueur local
        if (!isLocalPlayer) return;

        // Charger les composants du joueur
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

    /* Méthodes */

    // Application des mouvements du joueur
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

    // Envoi de la position du joueur au serveur
    private void SendPositionToServer()
    {
        AnimatorStateInfo currentState = Anim.GetCurrentAnimatorStateInfo(0);
        string animationName = currentState.IsName("Idle") ? "Idle" :
                               currentState.IsName("Walk") ? "Walk" : "Other";

        CharacterUpdate update = new CharacterUpdate
        {
            messageType = MessageType.CharacterUpdate,
            playerID = Globals.playerID,
            position = transform.position,
            rotation = transform.rotation,
            animation = animationName
        };

        string message = JsonUtility.ToJson(update);
        udpClient.sendMesageToServer(message);
    }

    // Définit si le joueur est local ou non
    public void SetLocalPlayer(bool isLocal)
    {
        isLocalPlayer = isLocal;
    }

}
