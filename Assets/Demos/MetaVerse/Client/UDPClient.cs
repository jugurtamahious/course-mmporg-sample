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

        PlayerMessage playerMessage = JsonUtility.FromJson<PlayerMessage>(message);
        // Passer le playerID à la méthode MovePlayer
        MovePlayer(message, playerMessage.playerID);
        //  gameManager.OnNewClientConnected(ServerEndpoint.ToString());


    }

    public void MovePlayer(string message, string playerID)
    {

        try
        {
            CharacterUpdate positionData = JsonUtility.FromJson<CharacterUpdate>(message);

            if (players.ContainsKey(playerID))
            {
                if (Globals.playerID != playerID)
                {
                    players[playerID].transform.position = positionData.position;
                }
            }
            else
            {
                GameObject newPlayer = Instantiate(CharacterPrefab, SpawnArea.position, SpawnArea.rotation);
                newPlayer.transform.position = positionData.position;
                players.Add(playerID, newPlayer);
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing JSON message: {message}. Exception: {ex.Message}");
        }
    }

    public void sendMesageToServer(string message)
    {
        UDP.SendUDPMessage(message, ServerEndpoint);
    }


    [System.Serializable]
    public class PlayerMessage
    {
        public string playerID;
        public Position position;
    }

    [System.Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }
}