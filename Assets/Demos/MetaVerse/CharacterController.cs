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
  private Quaternion networkedRotation;
  private float networkedAnimation;
  private float lastRecordedTime = 0f; // Dernier temps enregistré

  // Historique des données
  private List<PlayerData> dataHistory = new List<PlayerData>();

  private string filePath;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    
    Anim = GetComponent<Animator>();
    inputs = new MetaverseInput();
    PlayerAction = inputs.Player1.Move;
    

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
    // RetrievePlayerData(newPosition, newRotation);

    // Envoie des données
    // Debug.Log("Envoie données");
    SendPositionToServer();


    RetrievePlayerData(newPosition, newRotation);
    
    // Envoie de la position au serveur
    SendPositionToServer();
    
    // Debug.Log(vec.y);

    //Debug.Log($"Player {Player} position: {newPosition}, walk: {vec.y}, rotate: {vec.x}");
  }

  void OnDisable()
  {
    PlayerAction.Disable();
  }

  private void SendPositionToServer()
  {
    float animationValue = Anim.GetFloat("Walk");
    Vector3 position = transform.position;
    Quaternion rotation = transform.rotation;

    //Debug.Log(animationValue);
    
    float[] message = new float[] { position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, animationValue };

    

    // Convertir le tableau en une chaîne lisible
    string messageString = string.Join("/ ", message);
    // Debug.Log($"Position envoyée : {messageString}");
    udpServer.SendData(messageString, "127.0.0.1", 25000);

    Debug.Log(messageString);
  }

  private byte[] MessageToByte(string message) {
    return System.Text.Encoding.UTF8.GetBytes(message);

  }

  public void UpdatePositionFromServer(Vector3 newPos, Quaternion newRot, float newAnim)
  {
    networkedPosition = newPos;
    networkedRotation = newRot;
    networkedAnimation = newAnim;
  }

  private void InterpolatePosition()
  {
    // Interpolation douce vers la position réseau
    Anim.SetFloat("Walk", networkedAnimation);
    transform.position = Vector3.Lerp(transform.position, networkedPosition, Time.deltaTime * 10);
    transform.rotation = Quaternion.Lerp(transform.rotation, networkedRotation, Time.deltaTime * 10);
    
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
    float animationValue = Anim.GetFloat("Walk");

    PlayerData data = new PlayerData
    {
      PlayerID = Player.ToString(),
      Position = position,
      Rotation = rotation,
      AnimationValue = animationValue > 0 ? 1 : animationValue < 0 ? -1 : 0,
      DeltaTime = deltaTime
    };

    // Ajouter à l'historique
    dataHistory.Add(data);
    //Debug.Log(rotation);
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


}

// Classe pour structurer les données du joueur
[System.Serializable]
public class PlayerData
{
  public string PlayerID;
  public Vector3 Position;
  public Quaternion Rotation;
  public float AnimationValue;
  public float DeltaTime;

}

// Classe pour contenir une liste de données
[System.Serializable]
public class PlayerDataList
{
  public List<PlayerData> Data = new List<PlayerData>();
}