using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public int x = 1;
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
                // Check for transition to idle state
                if ( x >=1 )
                {
                    SetState(EnemyState.Idle);
                }
                // Check for transition to shoot state
                else if (x >= 1)
                {
                    SetState(EnemyState.Shoot);
                }
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

            //case EnemyState.Idle:
            //    // Deactivate other state scripts and start idle timer
            //    GetComponent<PatrolState>().enabled = false;
            //    idleTimer = 0f;
            //    GetComponent<IdleState>().enabled = true;
            //    break;

                //case EnemyState.Shoot:
                //    // Deactivate other state scripts and start shooting
                //    GetComponent<IdleState>().enabled = false;
                //    GetComponent<ShootState>().enabled = true;
                //    break;
        }
    }
}
