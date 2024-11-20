using UnityEngine;
using UnityEngine.InputSystem;

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
  private Vector3 networkedPosition; // Position reçue du serveur pour interpolation



  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    Anim = GetComponent<Animator>();
    inputs = new MetaverseInput();

    PlayerAction = inputs.Player1.Move;


    PlayerAction.Enable();

    rb = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    Vector2 vec = PlayerAction.ReadValue<Vector2>();
    Anim.SetFloat("Walk", vec.y);

    rb.MovePosition(rb.position + transform.forward * WalkSpeed * Time.fixedDeltaTime * vec.y);

    rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime * vec.x, Vector3.up));
  }

  void OnDisable()
  {
    PlayerAction.Disable();
  }

  private void SendPositionToServer()
  {
    Vector3 position = transform.position;
    string message = JsonUtility.ToJson(new { x = position.x, y = position.y, z = position.z });
    // Envoyer la position au GameState
    // Appeler un script UDP pour envoyer la position (ex : UDPServer.Instance.Send(message));
  }

  public void UpdatePositionFromServer(Vector3 newPos)
  {
    networkedPosition = newPos;
  }

  private void InterpolatePosition()
  {
    // Interpolation douce vers la position réseau
    transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * 10);
  }

  private bool IsLocalPlayer()
  {
    // Logique pour déterminer si c'est le joueur contrôlé localement
    return true; // Remplacez par votre logique de contrôle réseau
  }
}
