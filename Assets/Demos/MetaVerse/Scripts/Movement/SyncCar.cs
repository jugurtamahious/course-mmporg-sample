using UnityEngine;

public class SyncCar : MonoBehaviour
{
    private Animator animator;
    private float syncThreshold = 0.1f;
    public GameManager gameManager;
    private int carID;

    void Start()
    {

        carID = GetInstanceID();

        animator = GetComponent<Animator>();

        // Lancer une boucle périodique pour envoyer les durées d'animation
        InvokeRepeating(nameof(SendAnimationTime), 1f, 1f); // Appelle toutes les secondes
    }

    public void UpdateAnimation(float newAnimationTime)
    {
        if (animator)
        {
            float currentAnimationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
            if (Mathf.Abs(currentAnimationTime - newAnimationTime) > syncThreshold)
            {
                animator.Play(0, 0, newAnimationTime);
            }
        }
    }

    public float GetAnimationTime()
    {
        if (animator)
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        }
        return 0f;
    }

    private void SendAnimationTime()
    {
        // if (gameManager.OnCarAnimationTimeUpdated != null)
        {
            float animationTime = GetAnimationTime();
            // gameManager.OnCarAnimationTimeUpdated.Invoke(carID, animationTime);
        }
    }
}
