using System.Collections.Generic;
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
            stateMachine.SetState(StateMachine.EnemyState.Shoot);
            // DT processes it and calls the State machine to transition to to Shoot state and execute it

            List<float> alertSpeeds = new List<float> { 1.0f, 1.5f, 2.0f, 2.5f };
            List<float> probabilities = new List<float> { 0.1f, 0.2f, 0.4f, 0.3f }; // Adjust these probabilities as needed
            float selectedSpeed = RouletteWheelSelection(alertSpeeds, probabilities);

            stateMachine.SetAlertSpeed(selectedSpeed);
        }
    }

    public bool IsPlayerInSight() => IsPlayerInSight(1f, 1f);

    // rangeMultiplier/angleMultiplier let callers widen the check (e.g. while already
    // chasing) so jitter right at the edge of sightRange/coneAngle doesn't flip the
    // result every frame.
    public bool IsPlayerInSight(float rangeMultiplier, float angleMultiplier)
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float effectiveRange = sightRange * rangeMultiplier;
        float effectiveAngle = coneAngle * angleMultiplier;

        if (directionToPlayer.magnitude <= effectiveRange)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) <= effectiveAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, effectiveRange))
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

    private float RouletteWheelSelection(List<float> values, List<float> probabilities)
    {
        float randomValue = Random.value;
        float cumulativeProbability = 0;

        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                return values[i];
            }
        }

        // Fallback in case no value is selected (shouldn't happen)
        Debug.LogError("No value selected in RouletteWheelSelection.");
        return values[0];
    }
}
