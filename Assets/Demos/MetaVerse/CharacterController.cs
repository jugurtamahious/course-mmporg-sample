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

  // Variables pour stocker les données
  private Vector3 currentPosition;
  private float currentAnimation;
  private float currentRotation;

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
    currentAnimation = walkValue;
    currentRotation = rotateValue;

    Debug.Log(walkValue);

    // Afficher les données dans la console
    Debug.Log($"Player: {Player}, Position: {currentPosition}, Animation: {currentAnimation}, Rotation: {currentRotation}");
  }

  void OnDisable()
  {
    PlayerAction.Disable();
  }
}
