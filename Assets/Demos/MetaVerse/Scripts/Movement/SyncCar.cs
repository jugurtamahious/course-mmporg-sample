using UnityEngine;

public class SyncCar : MonoBehaviour
{
    private Animation animationComponent;
    private float syncThreshold = 1f;
    public UDPServer udpServer; // Référence au serveur UDP
    public UDPClient udpClient; // Référence au client UDP
    private float timeSinceLastUpdate = 0f;
    private const float updateInterval = 1f; // Intervalle de mise à jour en secondes

    private string carID;

    void Start()
    {
        carID = gameObject.name;

        // Récupérer le composant Animation
        animationComponent = GetComponent<Animation>();

        if (animationComponent == null)
        {
            Debug.LogError("Aucun composant Animation trouvé sur l'objet : " + carID);
            return;
        }

        // Souscription à l'evennement
        udpClient.OnCarUpdatePos += UpdateAnimation;

        
    }

    void Update()
    {
        if (Globals.IsServer)
        {
            timeSinceLastUpdate += Time.deltaTime;

            if (timeSinceLastUpdate >= updateInterval)
            {
                SendAnimationTime();
                timeSinceLastUpdate = 0f; // Réinitialiser le compteur de temps
            }
        }
    }

    public void UpdateAnimation(string otherCarID, float newAnimationTime)
    {
        if (animationComponent && carID == otherCarID)
        {
            // Obtenez l'animation actuelle
            AnimationState currentState = animationComponent[animationComponent.clip.name];
            if (currentState != null)
            {
                float currentAnimationTime = currentState.time % currentState.length;
                if (Mathf.Abs(currentAnimationTime - newAnimationTime) > syncThreshold)
                {
                    currentState.time = newAnimationTime * currentState.length;
                    animationComponent.Play();
                }
            }
        }
    }

    public float GetAnimationTime()
    {
        if (animationComponent)
        {
            AnimationState currentState = animationComponent[animationComponent.clip.name];
            if (currentState != null)
            {
                return (currentState.time % currentState.length) / currentState.length;
            }
        }
        return 0f;
    }

    private void SendAnimationTime()
    {

        Debug.Log("Démarrage du script pour la voiture " + carID);

        if (Globals.IsServer && udpServer != null)
        {
            float animationTime = GetAnimationTime();

            CarSyncUpdate update = new CarSyncUpdate
            {
                messageType = MessageType.CarPositionUpdate,
                carID = carID,
                animationTime = animationTime
            };

            string message = JsonUtility.ToJson(update);
            udpServer.BroadcastCarPositions(message); // Appel à la méthode de diffusion du serveur
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
    public class CarSyncUpdate : BaseMessage
    {
        public string carID;
        public float animationTime;
    }
}
