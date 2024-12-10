using System.Net;
using UnityEngine;
using System.Collections.Generic;

public class UDPClient : MonoBehaviour
{
    private UDPService UDP;
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 25000;

    public GameManager gameManager;

    private float NextCoucouTimeout = -1;
    private IPEndPoint ServerEndpoint;

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
        string PlayerID = sender.Address.ToString() + ":" + sender.Port.ToString();
        Debug.Log("[CLIENT] Message received from " +
            sender.Address.ToString() + ":" + sender.Port
            + " =>" + message);
        //  gameManager.OnNewClientConnected(ServerEndpoint.ToString());
        MovePlayer(message, PlayerID);


    }

    public void MovePlayer(string message, string playerID)
    {

        Dictionary<string, GameObject> players = gameManager.getClientCharacters();

        try
        {
            CharacterUpdate positionData = JsonUtility.FromJson<CharacterUpdate>(message);

            if (players.ContainsKey(playerID))
            {
                players[playerID].transform.position = positionData.position;
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
}