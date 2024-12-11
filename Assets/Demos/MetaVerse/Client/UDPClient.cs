using System.Net;
using UnityEngine;
using System.Collections.Generic;

public class UDPClient : MonoBehaviour
{


    /* Serializable */

    [System.Serializable]
    public class BaseMessage
    {
        public MessageType messageType;
    }

    [System.Serializable]
    public class CharacterUpdate
    {
        public string playerID;
        public Vector3 position;
        public Quaternion rotation;
        public string animation;
    }

    [System.Serializable]
    public class CarSyncUpdate
    {
        public string carID;
        public float animationTime;
    }

    /* Variables Publiques */

    public string ServerIP = "127.0.0.1";
    public int ServerPort = 25000;

    public GameObject CharacterPrefab;
    public Transform SpawnArea;
    public GameManager gameManager;
    public ScoreManager scoreManager;


    /* Variables Privées */

    private UDPService UDP;

    private IPEndPoint ServerEndpoint;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    /* Events */

    public delegate void CarUpdatePos(string carID, float time);
    public event CarUpdatePos OnCarUpdatePos;

    /* Méthodes Unity */

    // On détruit l'objet si ce n'est pas le serveur
    void Awake()
    {
        if (Globals.IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    // Ajout du UDPService, initialisation du client
    // Définition de l'adresse IP et du port du serveur
    // Ajout de l'événement OnMessageReceived
    void Start()
    {
        UDP = gameObject.AddComponent<UDPService>();

        UDP.InitClient();

        GameObject newPlayer = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);
        CharacterController c = newPlayer.GetComponent<CharacterController>();
        c.enabled = true;

        Globals.playerID = "Player" + UnityEngine.Random.Range(1000, 9999).ToString();
        players.Add(Globals.playerID, newPlayer);

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(Globals.HostIP), Globals.HostPort);

        UDP.OnMessageReceived += OnMessageReceived;


    }

    /* Méthodes */

    // Traitement du message quand le client le recoit du serveur
    private void OnMessageReceived(string message, IPEndPoint sender)
    {
        // Debug 
        Debug.Log("[CLIENT] Message received from " +
            sender.Address.ToString() + ":" + sender.Port
            + " =>" + message);

        // Désérialisation du message
        try
        {

            // Lecture du type de message
            BaseMessage baseMessage = JsonUtility.FromJson<BaseMessage>(message);

            switch (baseMessage.messageType)
            {
                // Traitement pour les déplacements de personnage
                case MessageType.CharacterUpdate:
                    CharacterUpdate updatePlayer = JsonUtility.FromJson<CharacterUpdate>(message);
                    MovePlayer(updatePlayer, updatePlayer.playerID);
                    break;

                // Traitement pour les mises à jour des positions des voitures
                case MessageType.CarPositionUpdate:
                    CarSyncUpdate updateCar = JsonUtility.FromJson<CarSyncUpdate>(message);
                    UpdateCarPositions(updateCar);
                    break;
            }


        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing message: {ex.Message}");
        }

    }

    // Met à jour la position des voitures avec un event
    public void UpdateCarPositions(CarSyncUpdate update)
    {
        OnCarUpdatePos?.Invoke(update.carID, update.animationTime);
    }

    // Déplacement du joueur
    public void MovePlayer(CharacterUpdate positionData, string playerID)
    {
        // Vérifie si le joueur local est celui reçu
        if (Globals.playerID == playerID)
        {
            return;
        }

        // Si le joueur existe déjà, mettez à jour sa position et animation
        if (players.ContainsKey(playerID))
        {
            players[playerID].transform.position = positionData.position;
            players[playerID].transform.rotation = positionData.rotation;

            Animator animator = players[playerID].GetComponent<Animator>();
            if (animator)
            {
                string animationToPlay = positionData.animation;
                if (animationToPlay == "Other")
                {
                    animationToPlay = "Walk";
                }

                if (animationToPlay == "Walk")
                {
                    animator.SetFloat("Walk", 1.0f);
                }
                else
                {
                    animator.SetFloat("Walk", 0.0f);
                    animator.Play(animationToPlay);
                }
            }
        }
        else
        {
            // Création d'un nouveau joueur
            GameObject newPlayer = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);
            newPlayer.transform.position = positionData.position;
            newPlayer.transform.rotation = positionData.rotation;
            players.Add(playerID, newPlayer);
        }
    }


    // Envoi d'un message au serveur via UDP Service
    public void sendMesageToServer(string message)
    {
        UDP.SendUDPMessage(message, ServerEndpoint);

        // Find the ScoreManager instance
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.UpdatePlayerList();
        }
    }
}


