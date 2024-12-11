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

    public ScoreManager scoreManager;

    private float NextCoucouTimeout = -1;
    private IPEndPoint ServerEndpoint;

    public Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    public Vector3 targetPosition;
    public Quaternion targetRotation;

    // Method to get the players dictionary
    public Dictionary<string, GameObject> GetPlayers()
    {
        return players;
    }

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
            CharacterUpdate update = JsonUtility.FromJson<CharacterUpdate>(message);
            MovePlayer(update, update.playerID);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error parsing message: {ex.Message}");
        }

    }

    public void MovePlayer(CharacterUpdate positionData, string playerID)
    {
        if (players.ContainsKey(playerID))
        {
            if (Globals.playerID != playerID)
            {
                // Met à jour les cibles
                targetPosition = positionData.position;
                targetRotation = positionData.rotation;

                // Interpolation
                players[playerID].transform.position = Vector3.Lerp(players[playerID].transform.position, targetPosition, Time.deltaTime * 30f);
                players[playerID].transform.rotation = Quaternion.Lerp(players[playerID].transform.rotation, targetRotation, Time.deltaTime * 30f);

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

        // Find the ScoreManager instance
        scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.UpdatePlayerList();
        }
    }

    [System.Serializable]
    public class CharacterUpdate
    {
        public string playerID;
        public Vector3 position;
        public Quaternion rotation;
        public string animation;
    }
}
