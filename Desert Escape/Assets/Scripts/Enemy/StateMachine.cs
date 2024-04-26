using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{
    public int x = 1;
    private DecisionTree decisionTree;
    private float speed = 1.5f;

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
        SetState(EnemyState.Patrol);
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (decisionTree.IsPlayerInSight()) 
                {
                    SetState(EnemyState.Shoot);
                }
                break;
            case EnemyState.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleDuration)
                {
                    SetState(EnemyState.Patrol);
                }
                break;
            case EnemyState.Shoot:
                // Disable other state scripts
                GetComponent<PatrolState>().enabled = false;

                // Apply the pursuit behavior to follow the player
                Vector3 pursueDir = new Pursuit(transform, player.GetComponent<Rigidbody>(), 1f).GetDir();
                // Apply the direction to the enemy's movement
                // For now, let's just log the direction
                Debug.Log("Pursuit Direction: " + pursueDir);

                // Move the enemy in the direction of pursueDir (you need to implement this part)
                // For example, you can use a NavMeshAgent to move the enemy towards the player
                GetComponent <NavMeshAgent>().destination = transform.position + pursueDir * speed;
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
}

