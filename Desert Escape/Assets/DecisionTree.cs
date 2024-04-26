// DecisionTree.cs
using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    public float sightRange = 10f;
    public float coneAngle = 45f;
    private Transform player;
    private StateMachine stateMachine; // Add a reference to the StateMachine

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        stateMachine = GetComponent<StateMachine>(); // Get reference to StateMachine
    }

    private void Update()
    {
        if (IsPlayerInSight())
        {
            Debug.Log("Player in sight!");
            stateMachine.SetState(StateMachine.EnemyState.Shoot); // Transition to Shoot state
        }
    }

    public bool IsPlayerInSight()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, coneAngle / 2, 0) * transform.forward * sightRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -coneAngle / 2, 0) * transform.forward * sightRange);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * sightRange);
    }
}
