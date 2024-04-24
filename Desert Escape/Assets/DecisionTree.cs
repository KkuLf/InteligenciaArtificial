using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    public float sightRange = 10f; // Range of enemy's line of sight
    public float coneAngle = 45f; // Angle of enemy's cone of vision

    private Transform player; // Reference to the player's transform

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player
    }

    private void Update()
    {
        // Check if the player is within the enemy's line of sight
        if (IsPlayerInSight())
        {
            Debug.Log("Player in sight!");
            // Perform actions when player is spotted
        }
    }

    private bool IsPlayerInSight()
    {
        // Calculate direction to the player
        Vector3 directionToPlayer = player.position - transform.position;

        // Check if the player is within sight range
        if (directionToPlayer.magnitude <= sightRange)
        {
            // Check if player is within the cone of vision
            if (Vector3.Angle(transform.forward, directionToPlayer) <= coneAngle / 2)
            {
                // Perform raycast to check if there are obstacles between enemy and player
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        // LEFE ACA TENES QUE PONER QUE SE PASE AL STATE MACHINE DE SHOOTSTATE
                        // Y SOLO QUE APUNTE AL JUGADOR Y SE ESUCHE UN TIRO
                        return true; // Player is in sight
                    }
                }
            }
        }

        return false; // Player is not in sight
    }

    private void OnDrawGizmosSelected()
    {
        // Draw the line of sight cone in the scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, coneAngle / 2, 0) * transform.forward * sightRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -coneAngle / 2, 0) * transform.forward * sightRange);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * sightRange);
    }
}
