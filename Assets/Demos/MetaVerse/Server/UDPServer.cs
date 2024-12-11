using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private UDPService UDP;
    public int ListenPort = 25000;
    public GameObject CharacterPrefab;
    public Transform SpawnArea;

    public Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>();
    private Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (!Globals.IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        UDP = gameObject.AddComponent<UDPService>();

        UDP.Listen(ListenPort);

        UDP.OnMessageReceived += (string message, IPEndPoint sender) =>
        {
            string playerID = sender.Address.ToString() + ":" + sender.Port;

            if (!Clients.ContainsKey(playerID))
            {
                Clients.Add(playerID, sender);
            }

            MovePlayer(message, playerID);
            BroadcastPlayerPositions(message);
        };
    }

    /// <summary>
    /// Met à jour ou crée un joueur, et diffuse les positions à tous les clients.
    /// </summary>
    /// <param name="message">Message JSON contenant la position.</param>
    /// <param name="playerID">ID unique du joueur.</param>
    public void MovePlayer(string message, string playerID)
    {

        CharacterUpdate positionData = JsonUtility.FromJson<CharacterUpdate>(message);

        if (players.ContainsKey(playerID))
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
                    animator.SetFloat("Walk", 1.0f);
                }
                else
                {
                    animator.SetFloat("Walk", 0.0f);
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
    /// <summary>
    /// Envoie les positions de tous les joueurs à tous les clients connectés.
    /// </summary>
    public void BroadcastPlayerPositions(string message)
    {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients)
        {
            UDP.SendUDPMessage(message, client.Value);
        }
    }

    public void BroadcastCarPositions(string message)
    {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients)
        {
            UDP.SendUDPMessage(message, client.Value);
        }
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
    public class CharacterUpdate : BaseMessage
    {
        public string playerID;
        public Vector3 position;
        public Quaternion rotation;
        public string animation;
    }

    [System.Serializable]
    public class CarSyncUpdate : BaseMessage
    {
        public string carID;
        public float animationTime;
    }


}
