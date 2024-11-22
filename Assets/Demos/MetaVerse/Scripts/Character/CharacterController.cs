using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;


public enum CharacterPlayer
{
  Player1,
}

public class CharacterController : MonoBehaviour
{
  public CharacterPlayer Player = CharacterPlayer.Player1;
  public float WalkSpeed = 3;
  public float RotateSpeed = 250;
  public UDPSender udpServer;
  public CharacterScore characterScore;
  


  public PlayerState CurrentPlayerState
      {
          get
          {
            
              return new PlayerState
              {
                  PlayerID = Player.ToString(),
                  Position = transform.position,
                  Rotation = transform.rotation,
                  AnimationValue = Anim.GetFloat("Walk"),
                  Score = characterScore != null ? characterScore.Score : 0,
                  DeltaTime = Time.time - lastRecordedTime
              };
          }
      }
  

  Animator Anim;
  MetaverseInput inputs;
  InputAction PlayerAction;
  Rigidbody rb;

  

  private Vector3 networkedPosition; // Position reçue du serveur pour interpolation
  private Quaternion networkedRotation;
  private float networkedAnimation;
  private float lastRecordedTime = 0f; // Dernier temps enregistré




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

    // Mise à jour de la position
    Vector3 newPosition = rb.position + transform.forward * WalkSpeed * Time.fixedDeltaTime * vec.y;
    rb.MovePosition(newPosition);

    // Mise à jour de la rotation
    Quaternion newRotation = rb.rotation * Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime * vec.x, Vector3.up);
    rb.MoveRotation(newRotation);

    PlayerState currentState = CurrentPlayerState;
    Debug.Log(currentState);
    //Debug.Log($"Player State: {currentState.PlayerID},Rotation: {currentState.Rotation}, Position: {currentState.Position}, Score: {currentState.Score}, DeltaTime: {currentState.DeltaTime}, Animation: {currentState.AnimationValue}");

    // Envoie des données
    
    SendStateToServer();
    
    // Envoie de la position au serveur
    SendStateToServer(); 
    
    // Debug.Log(vec.y);

    //Debug.Log($"Player {Player} position: {newPosition}, walk: {vec.y}, rotate: {vec.x}");
  }

  void OnDisable()
  {
    PlayerAction.Disable();
  }

  private void SendStateToServer()
  { 
    // ajouter les state du joueur ici

    characterScore = FindObjectOfType<CharacterScore>();
    //Debug.Log(CurrentPlayerState.PlayerID);

    if (CurrentPlayerState == null)
    {
        Debug.LogError("CurrentPlayerState is null!");
        return;
    }
    
    string message = JsonUtility.ToJson(CurrentPlayerState);

    //Debug.Log("Serialized Player State: " + message);
    //Debug.Log(message); 


    // Convertir le tableau en une chaîne lisible
    udpServer.SendData(message, "127.0.0.1", 25000);
    
  }

  private byte[] MessageToByte(string message) {
    return System.Text.Encoding.UTF8.GetBytes(message);

  }


  private bool IsLocalPlayer()
  {
    // Logique pour déterminer si c'est le joueur contrôlé localement
    return true; // Remplacez par votre logique de contrôle réseau
  }
  
  

}

[System.Serializable] // Important pour la sérialisation Unity
public class PlayerState
{
    public string PlayerID;
    public Vector3 Position;
    public Quaternion Rotation;
    public float AnimationValue;
    public int Score;
    public float DeltaTime;
}
