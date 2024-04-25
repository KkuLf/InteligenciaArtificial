using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Idle,
        Shoot
    }

    private EnemyState currentState;
    private float idleTimer;
    private float idleDuration = 6f; // Duration of idle state

    private void Start()
    {
        SetState(EnemyState.Patrol);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                // You only need to transition to the Shoot state if the player is in sight.
                // No need to check for Idle state here.
                break;

            case EnemyState.Idle:
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleDuration)
                {
                    // Transition back to patrol state after idle duration
                    SetState(EnemyState.Patrol);
                }
                break;

            case EnemyState.Shoot:
                // Implement logic for shooting state
                break;
        }
    }

    public void SetState(EnemyState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case EnemyState.Patrol:
                // Activate PatrolState script
                GetComponent<PatrolState>().enabled = true;
                break;

            case EnemyState.Idle:
                // Deactivate PatrolState script when transitioning to Idle
                GetComponent<PatrolState>().enabled = false;
                idleTimer = 0f; // Reset idle timer
                break;

            case EnemyState.Shoot:
                Debug.Log("statemachineshoot");
                break;
        }
    }
}
