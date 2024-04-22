using UnityEngine;
using UnityEngine.AI;

public class PatrolState : MonoBehaviour
{
    public Transform[] waypoints; // Array to hold the patrol waypoints
    private int currentWaypointIndex = 0; // Index of the current waypoint
    private NavMeshAgent agent; // Reference to the NavMeshAgent component

    [HideInInspector]
    public bool isWaiting = false; // Flag to indicate if the enemy is currently waiting
    private float waitDuration = 3f; // Duration of wait time in seconds

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component

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
}
