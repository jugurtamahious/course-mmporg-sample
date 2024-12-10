using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    public UDPService UDP;
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
        try
        {
            CharacterUpdate positionData = JsonUtility.FromJson<CharacterUpdate>(message);

            if (players.ContainsKey(playerID))
            {
                players[playerID].transform.position = positionData.position;
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
   /// <summary>
    /// Envoie les positions de tous les joueurs à tous les clients connectés.
    /// </summary>
   public void BroadcastPlayerPositions(string message) {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients) {
            UDP.SendUDPMessage(message, client.Value);
        }
    }

}
