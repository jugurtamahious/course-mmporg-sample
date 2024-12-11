 using UnityEngine;

    public class TCPServer : MonoBehaviour
    {
        private TCPService tcpService;
        public GameManager GameManager;

        void Awake(){
            if (!Globals.IsServer) {
               gameObject.SetActive(false);
            }
        }
        private void Start()
        {
            tcpService = gameObject.AddComponent<TCPService>();

            // Gestion des events
            tcpService.OnClientConnected += OnClientConnected;
            tcpService.OnClientRemoved += OnClientRemoved;

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
        }

        private void OnClientConnected(string client)
        {
            // Création du personnage du client
            GameManager.OnNewClientConnected(client);
        }

        private void OnClientRemoved(string ip) {
            GameManager.OnRemoveClient(ip);
        }

        private void OnApplicationQuit()
        {
            tcpService.StopService();
        }
    }