using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class StateMachine : MonoBehaviour
{
    private DecisionTree decisionTree;
    private float speed = 1.5f;
    private float alertSpeed = 1.5f;

    public enum EnemyState
    {
        Patrol,
        Idle,
        Shoot
    }

    private EnemyState currentState;
    private float idleTimer;
    private float idleDuration = 6f; // Duration of idle state
    private Transform player; // Reference to the player's transform

    private void Start()
    {
        decisionTree = GetComponent<DecisionTree>();
        SetState(EnemyState.Patrol);
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:                         // Patrol State from when moving between waypoints
                if (decisionTree.IsPlayerInSight())         // If the player is spotted
                {
                    SetState(EnemyState.Shoot);             // We transition to the shoot state
                }
                break;
            case EnemyState.Idle:                           // Idle state from waiting at waypoints before patroling to the next
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleDuration)              // We set a timer for the wait duration
                {
                    SetState(EnemyState.Patrol);            // And we go back into the patrol
                }
                break;
            case EnemyState.Shoot:                          // Shoot state for when the player has been spotted

                // Disable other state scripts
                GetComponent<PatrolState>().enabled = false;

                // Apply the pursuit behavior to follow the player
                Vector3 pursueDir = new Pursuit(transform, player.GetComponent<Rigidbody>(), 1f).GetDir();

                // Apply the direction to the enemy's movement
                //Debug.Log("Pursuit Direction: " + pursueDir);

                // We use a NavMeshAgent to move the enemy towards the player in the direction of pursueDir
                GetComponent<NavMeshAgent>().speed = alertSpeed;
                GetComponent<NavMeshAgent>().destination = transform.position + pursueDir * speed;
                break;
        }
    }

    public void SetState(EnemyState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case EnemyState.Patrol:
                GetComponent<PatrolState>().enabled = true;
                break;
            case EnemyState.Idle:
                GetComponent<IdleState>().enabled = true;
                break;
            case EnemyState.Shoot:
                // Deactivate other state scripts and start shooting
                GetComponent<PatrolState>().enabled = false;
                GetComponent<ShootState>().enabled = true;
                break;
        }
    }

    public void SetAlertSpeed(float speed)
    {
        alertSpeed = speed;
    }
}
