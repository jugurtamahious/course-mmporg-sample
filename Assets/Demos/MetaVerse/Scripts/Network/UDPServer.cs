using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private UDPService UDPService;
    public GameManager GameManager;
    public int ListenPort = 25000;

    void Awake()
    {
        if (!Globals.IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // UDPService.Listen(ListenPort);
        // UDPService.OnMessageReceived += OnMessageReceived;
    }

    void Update()
    {

    }

    /**
    * Méthodes d'écoutes
    */

    // Système "Protocol" de gestion d'actions
    private void OnMessageReceived(string message, IPEndPoint sender)
    {
        // DEBUG
        Debug.Log("[SERVER] Message received from " +
                            sender.Address.ToString() + ":" + sender.Port
                            + " =>" + message);

        // Traitement du message ( décodage )

        switch (message)
        {
            case "message":
                // Traitement de la demande suivante 
                // Réception du state du joueur X

                break;
        }

    }

    /**
    * Méthodes d'envoi
    */

    // Envoyer d'un message UDP a tout les joueurs
    public void SendUDPMessage(string message, IPEndPoint sender)
    {
        switch (message)
        {
            case "message":
                // GameState global
                // GameState players
            break;
        }
        UDPService.SendUDPMessage(message, sender );

    }
}