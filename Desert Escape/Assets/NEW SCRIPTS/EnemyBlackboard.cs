using UnityEngine;

// Centralizes the data the Enemy's FSM states and decision tree need, instead of each
// state/question receiving its own hand-picked subset of references as constructor
// params. EnemyController builds one of these and hands the same instance to everyone.
public class EnemyBlackboard
{
    public EnemyModel Model;
    public Transform Target;
    public ILineOfSight LineOfSight;
    public ISteering Pursuit;
    public ObstacleAvoidanceV2 ObstacleAvoidance;

    // Written by the tree's LOS question, read by states that care whether the player
    // is currently visible without each one re-running the sensor checks itself.
    public bool HasLineOfSight;
}
