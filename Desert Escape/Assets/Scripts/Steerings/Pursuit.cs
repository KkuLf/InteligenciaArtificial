// Pursuit.cs
using UnityEngine;

public class Pursuit : ISteering
{
    private Transform entity;
    private Rigidbody target;
    private float timePrediction;

    public Pursuit(Transform entity, Rigidbody target, float timePrediction)
    {
        this.entity = entity;
        this.target = target;
        this.timePrediction = timePrediction;
    }

    public Vector3 GetDir()
    {
        Vector3 point = target.position + target.transform.forward * target.velocity.magnitude * timePrediction;
        Vector3 dirToPoint = (point - entity.position).normalized;
        Vector3 dirToTarget = (target.position - entity.position).normalized;

        if (Vector3.Dot(dirToPoint, dirToTarget) < 0)
        {
            dirToPoint = dirToTarget;
        }

        return dirToPoint;
    }
}
