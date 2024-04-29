using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    public float sightRange = 10f;                  // Distance to which the enemy
    public float coneAngle = 45f;                   // Angle of vision
    private Transform player;                       // Player's transform
    private StateMachine stateMachine;              // Add a reference to the StateMachine

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;  
        stateMachine = GetComponent<StateMachine>();    // Get reference to StateMachine
    }

    private void Update()
    {
        if (IsPlayerInSight())      // We check if the player is spotted
        {
            Debug.Log("Player in sight!");
            stateMachine.SetState(StateMachine.EnemyState.Shoot); 
            // DT processes it and calls the State machine to transition to to Shoot state and execute it
        }
    }

    public bool IsPlayerInSight()       // LOS checking 
    {
        Vector3 directionToPlayer = player.position - transform.position;

        if (directionToPlayer.magnitude <= sightRange)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) <= coneAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, sightRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()     // Gizmos to visualize LOS in the editor and adjust accordingly :)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, coneAngle / 2, 0) * transform.forward * sightRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -coneAngle / 2, 0) * transform.forward * sightRange);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * sightRange);
    }
}
