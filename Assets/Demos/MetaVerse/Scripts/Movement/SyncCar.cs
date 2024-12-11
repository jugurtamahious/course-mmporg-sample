using UnityEngine;

public class SyncCar : MonoBehaviour
{
    private Animation animationComponent;
    private float syncThreshold = 1f;
    public GameManager gameManager;
    private string carID;
    public UDPServer udpServer; // Référence au serveur UDP

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

        // Lancer une boucle périodique pour envoyer les durées d'animation
        InvokeRepeating(nameof(SendAnimationTime), 1f, 1f);
    }

    public void UpdateAnimation(float newAnimationTime)
    {
        if (animationComponent)
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
        if (Globals.IsServer && udpServer != null)
        {
            float animationTime = GetAnimationTime();
            UDPServer.CarSyncUpdate syncUpdate = new UDPServer.CarSyncUpdate
            {
                carID = carID,
                animationTime = animationTime
            };

            string message = JsonUtility.ToJson(syncUpdate);
            udpServer.BroadcastMessage(message); // Appel à la méthode de diffusion du serveur
        }
    }
}
