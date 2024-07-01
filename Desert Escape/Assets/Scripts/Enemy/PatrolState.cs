using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class PatrolState : MonoBehaviour
{
    public Transform[] waypoints;                   // Array to hold the patrol waypoints
    private int currentWaypointIndex = 0;           // Index of the current waypoint
    private NavMeshAgent agent;                     // Reference to the NavMeshAgent component

    [HideInInspector]
    public bool isWaiting = false;                  // Flag to indicate if the enemy is currently waiting
    private float waitDuration = 3f;                // Duration of wait time in seconds

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();       // Get the NavMeshAgent component
        MoveToWaypoint(currentWaypointIndex);       // Start patrolling from the first waypoint
    }

    private void Update()
    {
        // Check if the agent has reached the current waypoint
        if (!isWaiting && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Start waiting
            isWaiting = true;
            Invoke(nameof(ContinuePatrol), waitDuration);   // Wait for the specified duration before continuing patrol
        }
    }
private void MoveToWaypoint(int index)
    {
        
        agent.SetDestination(waypoints[index].position);    // Set the destination to the position of the current waypoint
    }

    private void ContinuePatrol()
    {
        isWaiting = false; // End the waiting period

        // Move to the next waypoint
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        MoveToWaypoint(currentWaypointIndex);

        // Roulette to go to a random waypoint inside the array
        if (currentWaypointIndex == 0)
        {
            CalculateProbabilities();
            RouletteMoveToNextWaypoint();
        }
    }

    private void RouletteMoveToNextWaypoint()
    {
        List <float> probabilities = CalculateProbabilities();
        int selectedIndex = RouletteWheelSelection(probabilities);
        agent.SetDestination(waypoints[selectedIndex].position);
    }

    private List<float> CalculateProbabilities()
    {
        List<float> distances = new List<float>();

        // Calculates distances from the agent to each waypoint
        foreach (Transform waypoint in waypoints)
        {
            float distance = Vector3.Distance(transform.position, waypoint.position);
            distances.Add(distance);
        }
        List<float> probabilities = new List<float>();
        float totalDistance = distances.Sum();

        foreach (float distance in distances)
        {
            float probability = 1f - (distance / totalDistance);
            probabilities.Add(probability);
        }
        return probabilities;
    }
    private int RouletteWheelSelection(List<float> probabilities)
    {
        float randomValue = Random.value;  // Generate a random number between 0 and i

        //  Make selection based on probabilities
        float cumulativeProbability = 0;
        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return i;
            }
        }

        // If no waypoint can be selected 
        Debug.LogError("No waypoints availiable");
        return -1;
    }

    
}