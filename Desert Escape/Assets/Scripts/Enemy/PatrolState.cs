using UnityEngine;
using UnityEngine.AI;

public class PatrolState : MonoBehaviour
{
    public Transform[] waypoints; // Array to hold the patrol waypoints
    private int currentWaypointIndex = 0; // Index of the current waypoint
    private NavMeshAgent agent; // Reference to the NavMeshAgent component

    private bool isWaiting = false; // Flag to indicate if the enemy is currently waiting
    private float waitDuration = 3f; // Duration of wait time in seconds

    private Animator animator; // Reference to the Animator component

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component
        animator = GetComponent<Animator>(); // Get the Animator component

        // Start patrolling from the first waypoint
        MoveToWaypoint(currentWaypointIndex);
    }

    private void Update()
    {
        // Check if the agent has reached the current waypoint
        if (!isWaiting && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Start waiting
            isWaiting = true;
            Invoke(nameof(ContinuePatrol), waitDuration); // Wait for the specified duration before continuing patrol
            PlayIdleAnimation(); // Play idle animation while waiting
        }
        else
        {
            // Play running animation while moving to the next waypoint
            PlayRunningAnimation();
        }
    }

    private void ContinuePatrol()
    {
        isWaiting = false; // End the waiting period

        // Move to the next waypoint
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        MoveToWaypoint(currentWaypointIndex);

        // Reverse patrol path if the last waypoint is reached
        if (currentWaypointIndex == 0)
        {
            System.Array.Reverse(waypoints);
        }
    }

    private void MoveToWaypoint(int index)
    {
        // Set the destination to the position of the current waypoint
        agent.SetDestination(waypoints[index].position);
    }

    private void PlayRunningAnimation()
    {
        // Set the "IsIdle" parameter to false to play the running animation
        if (animator != null)
        {
            animator.SetBool("IsIdle", false);
        }
    }

    private void PlayIdleAnimation()
    {
        // Set the "IsIdle" parameter to true to play the idle animation
        if (animator != null)
        {
            animator.SetBool("IsIdle", true);
        }
    }
}
