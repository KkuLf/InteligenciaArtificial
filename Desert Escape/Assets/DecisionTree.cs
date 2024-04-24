using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    public float sightRange = 10f; // Range of enemy's line of sight
    public float coneAngle = 45f; // Angle of enemy's cone of vision

    private Transform player; // Reference to the player's transform
    private StateMachine stateMachine; // Reference to the state machine

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player
        stateMachine = GetComponent<StateMachine>(); // Get the state machine component
    }

    private void Update()
    {
        // Check if the player is within the enemy's line of sight
        if (IsPlayerInSight())
        {
            Debug.Log("Player in sight!");
            // Transition to shoot state
            stateMachine.SetState(StateMachine.EnemyState.Shoot);
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
                        // Transition to shoot state
                       
                        stateMachine.SetState(StateMachine.EnemyState.Shoot);
                        Debug.Log("shoot");
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

