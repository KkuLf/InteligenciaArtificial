using UnityEngine;

public class IdleState : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private PatrolState patrolState; // Reference to the PatrolState script
    private bool isPlayingAnimation = false; // Flag to track if animation is playing

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        patrolState = GetComponent<PatrolState>(); // Get the PatrolState component
    }

    private void Update()
    {
        // Check if the enemy is idle already
        if (patrolState != null && patrolState.isWaiting)
        {
            PlayIdleAnimation();
        }
        else
        {
            StopIdleAnimation();
        }
    }

    private void PlayIdleAnimation()
    {
        // Set the "IsIdle" parameter to true to start the idle animation
        if (animator != null && !isPlayingAnimation)
        {
            animator.SetBool("IsIdle", true);
            isPlayingAnimation = true;
        }
    }

    private void StopIdleAnimation()
    {
        // Set the "IsIdle" parameter to false to stop the idle animation
        if (animator != null && isPlayingAnimation)
        {
            animator.SetBool("IsIdle", false);
            isPlayingAnimation = false;
        }
    }
}
