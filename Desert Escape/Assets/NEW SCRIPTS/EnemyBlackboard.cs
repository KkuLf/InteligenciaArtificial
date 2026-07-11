using UnityEngine;

public class EnemyBlackboard
{
    public EnemyModel Model;
    public Transform Target;
    public ILineOfSight LineOfSight;
    public ISteering Pursuit;
    public ObstacleAvoidanceV2 ObstacleAvoidance;

    public bool HasLineOfSight;
}
