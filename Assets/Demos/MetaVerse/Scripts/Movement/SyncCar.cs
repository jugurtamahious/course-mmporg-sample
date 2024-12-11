using UnityEngine;

public class SyncCar : MonoBehaviour
{
    /* Variables Publiques */
    public UDPServer udpServer; // Référence au serveur UDP
    public UDPClient udpClient; // Référence au client UDP

    /* Variables Privées */
    private Animation animationComponent;
    private float syncThreshold = 1f;
    private float timeSinceLastUpdate = 0f;
    private const float updateInterval = 1f; // Intervalle de mise à jour en secondes
    private string carID;

    /* Méthodes Unity */

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
        // Mise à jour des positions de voitures
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

    /* Méthodes */

    // Mise à jour de l'animation
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

    // Obtenir le temps d'animation
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

    // Envoi du temps d'animation
    private void SendAnimationTime()
    {

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

}
