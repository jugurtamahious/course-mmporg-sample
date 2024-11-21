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

  Animator Anim;
  MetaverseInput inputs;
  InputAction PlayerAction;
  Rigidbody rb;

  private Vector3 networkedPosition; // Position reçue du serveur pour interpolation
  private float lastRecordedTime = 0f; // Dernier temps enregistré

  private string filePath;

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

    RetrievePlayerData(newPosition, newRotation);

    // Envoie de la position au serveur
    SendPositionToServer();
  }

  void OnDisable()
  {
    PlayerAction.Disable();
  }

  private void SendPositionToServer()
  {
    Vector3 position = transform.position;


    float[] message = new float[] { position.x, position.y, position.z };

    // Convertir le tableau en une chaîne lisible
    string messageString = string.Join("/ ", message);
    // Debug.Log($"Position envoyée : {messageString}");

    // TODO : Remplacer par les données du serveur
    udpServer.SendData(messageString, "127.0.0.1", 25004);

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

  void RetrievePlayerData(Vector3 position, Quaternion rotation)
  {
    float currentTime = Time.time; // Temps actuel dans Unity
    float deltaTime = currentTime - lastRecordedTime; // Temps écoulé depuis le dernier enregistrement
    lastRecordedTime = currentTime; // Mettre à jour le dernier temps enregistré

    PlayerData data = new PlayerData
    {
      PlayerID = Player.ToString(),
      Position = position,
      Rotation = rotation,
      DeltaTime = deltaTime
    };
  
  }

}

// Classe pour structurer les données du joueur
[System.Serializable]
public class PlayerData
{
  public string PlayerID;
  public Vector3 Position;
  public Quaternion Rotation;
  public float DeltaTime;

}

// Classe pour contenir une liste de données
[System.Serializable]
public class PlayerDataList
{
  public List<PlayerData> Data = new List<PlayerData>();
}