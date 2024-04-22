using UnityEngine;

public class IdleState : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component
    private bool isPlayingAnimation = false; // Flag to track if animation is playing

    private void Start()
    {
        animator = GetComponent<Animator>(); // Get the Animator component
        // Ensure animation is initially stopped
        if (animator != null)
        {
            animator.SetBool("IsIdle", false);
        }
    }

    private void Update()
    {
        // Check if animation is not playing and the enemy is idle
        if (!isPlayingAnimation) // Add && IdleCondition
        {
            PlayIdleAnimation();
        }
    }

    private void PlayIdleAnimation()
    {
        // Set the "IsIdle" parameter to true to start the idle animation
        if (animator != null)
        {
            animator.SetBool("IsIdle", true);
            isPlayingAnimation = true;
        }
    }

    // Method to stop the idle animation
    public void StopIdleAnimation()
    {
        // Set the "IsIdle" parameter to false to stop the idle animation
        if (animator != null)
        {
            animator.SetBool("IsIdle", false);
            isPlayingAnimation = false;
        }
    }
}
