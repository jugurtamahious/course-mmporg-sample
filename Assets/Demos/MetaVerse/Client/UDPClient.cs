using System.Net;
using UnityEngine;
using System.Collections.Generic;

public class UDPClient : MonoBehaviour
{
    private UDPService UDP;
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 25000;

    public GameObject CharacterPrefab;
    public Transform SpawnArea;

    public GameManager gameManager;

    private float NextCoucouTimeout = -1;
    private IPEndPoint ServerEndpoint;

    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Globals.IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {

        UDP = gameObject.AddComponent<UDPService>();

        UDP.InitClient();

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(Globals.HostIP), Globals.HostPort);

        UDP.OnMessageReceived += OnMessageReceived;

    }

    private void OnMessageReceived(string message, IPEndPoint sender)
    {
        Debug.Log("[CLIENT] Message received from " +
            sender.Address.ToString() + ":" + sender.Port
            + " =>" + message);

        try
        {

            BaseMessage baseMessage = JsonUtility.FromJson<BaseMessage>(message);

            switch (baseMessage.messageType)
            {
                case MessageType.CharacterUpdate:
                    CharacterUpdate updatePlayer = JsonUtility.FromJson<CharacterUpdate>(message);
                    MovePlayer(updatePlayer, updatePlayer.playerID);
                    break;
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

    public void UpdateCarPositions(CarSyncUpdate update)
    {
        GameObject car = GameObject.Find(update.carID);
        Animation carAnimation = car.GetComponent<Animation>();

        if (carAnimation != null)
        {
            // Assurez-vous que l'animation est en cours de lecture
            if (!carAnimation.isPlaying)
            {
                carAnimation.Play();
            }


            // Définir la durée de l'animation
            foreach (AnimationState state in carAnimation)
            {
                state.time = update.animationTime;
                carAnimation.Sample();
            }
        }
        else
        {
            Debug.LogWarning($"Animation component not found on car with ID {update.carID}.");
        }

    }

    public void MovePlayer(CharacterUpdate positionData, string playerID)
    {
        if (players.ContainsKey(playerID))
        {
            if (Globals.playerID != playerID)
            {
                players[playerID].transform.position = positionData.position;
                players[playerID].transform.rotation = positionData.rotation;

                Animator animator = players[playerID].GetComponent<Animator>();

                if (animator)
                {
                    string animationToPlay = positionData.animation;

                    // Map "Other" to "Walk"
                    if (animationToPlay == "Other")
                    {
                        animationToPlay = "Walk";
                    }

                    if (animationToPlay == "Walk")
                    {
                        animator.SetFloat("Walk", 1.0f); // Déclenche l'animation de marche
                    }
                    else
                    {
                        animator.SetFloat("Walk", 0.0f);
                        animator.Play(animationToPlay);
                    }
                }
            }
        }
        else
        {
            GameObject newPlayer = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);
            newPlayer.transform.position = positionData.position;
            newPlayer.transform.rotation = positionData.rotation;
            players.Add(playerID, newPlayer);
        }
    }


    public void sendMesageToServer(string message)
    {
        UDP.SendUDPMessage(message, ServerEndpoint);
    }

    public enum MessageType
    {
        CharacterUpdate,
        CarPositionUpdate
    }

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
}