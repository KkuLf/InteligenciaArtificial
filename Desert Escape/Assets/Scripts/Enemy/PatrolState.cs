using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

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
        MoveToNextWaypoint();
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
        isWaiting = false;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        MoveToNextWaypoint();
        if (currentWaypointIndex == 0)
        {
            System.Array.Reverse(waypoints);
        }
    }

    private void MoveToNextWaypoint()
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
        float randomValue = Random.value;  // Generate a random number between 0 and 1

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

        // If not select any waypoint 
        Debug.LogError("No se pudo seleccionar un waypoint.");
        return -1;
    }
}