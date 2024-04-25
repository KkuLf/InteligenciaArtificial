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

    public AudioClip shootSound; // Assign the audio clip for shooting in the inspector

    private EnemyState currentState;
    private float idleTimer;
    private float idleDuration = 6f; // Duration of idle state
    private bool playerInLineOfSight = false;
    private bool hasPlayedShootSound = false; // Track if shoot sound has been played

    private void Start()
    {
        // Set the initial state to Patrol
        SetState(EnemyState.Patrol);
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                // You only need to transition to the Shoot state if the player is in sight.
                // No need to check for Idle state here.
                if (playerInLineOfSight)
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
                if (!hasPlayedShootSound)
                {
                    PlayShootSound();
                    hasPlayedShootSound = true; // Set to true to indicate the sound has been played
                }
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

    private void PlayShootSound()
    {
        // Play the shoot sound at the position of the enemy
        AudioSource.PlayClipAtPoint(shootSound, transform.position);
    }

    // Example function to check if player is in line of sight, replace with your own logic
    private void OnTriggerStay(Collider other)
    {
        // Replace this with your actual line of sight checking logic
        if (other.CompareTag("Player"))
        {
            playerInLineOfSight = true;
            if (currentState != EnemyState.Shoot)
            {
                SetState(EnemyState.Shoot);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInLineOfSight = false;
            hasPlayedShootSound = false; // Reset so that sound can be played again if the player re-enters
            if (currentState == EnemyState.Shoot)
            {
                SetState(EnemyState.Patrol);
            }
        }
    }
}

