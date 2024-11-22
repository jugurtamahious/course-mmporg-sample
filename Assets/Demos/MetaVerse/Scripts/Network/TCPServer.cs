    using UnityEngine;
    using System.Net.Sockets;

    public class TCPServer : MonoBehaviour
    {
        private TCPService tcpService;
        public GameManager GameManager;

        void Awake(){
            if (!Globals.IsServer) {
                Destroy(this);
            }
        }
        private void Start()
        {
            tcpService = gameObject.AddComponent<TCPService>();
            tcpService.OnMessageReceived += OnMessageReceived;
            tcpService.OnClientConnected += OnClientConnected;

            if (tcpService.StartServer(Globals.HostPort))
            {
                Debug.Log("Server started.");
            }
            else
            {
                Debug.LogError("Failed to start server.");
            }

        }

        public void Update() {
            
            // Affichage de la liste des clients connectés
            tcpService.DisplayConnectedClients();

            // Acceptation des nouveaux clients
            tcpService.AcceptClients();

            // Supprimer les clients déconnectés
            tcpService.RemoveDisconnectedClients();

            // Réception des messages
            tcpService.ReceiveTCPMessages();
        }

        private void OnMessageReceived(string message, TcpClient sender)
        {
            Debug.Log("Received message from client: " + message);
        }

        private void OnClientConnected(string client)
        {
            // Création du personnage du client
            GameManager.OnNewClientConnected(client);
        }

        private void OnApplicationQuit()
        {
            tcpService.StopService();
        }
    }
