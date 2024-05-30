//using UnityEngine;

//public class Pursuit : ISteering
//{
//    private Transform entity;           // Represents the AI agent's transform
//    private Rigidbody target;           // Represents the target's Rigidbody component
//    private float timePrediction;       // Time in the future the AI agent predicts the target's position


//    public Pursuit(Transform entity, Rigidbody target, float timePrediction)    // Constructor that initializes it all
//    {
//        this.entity = entity;
//        this.target = target;
//        this.timePrediction = timePrediction;
//    }

//    public Vector3 GetDir()     // Calculates the direction in which the AI agent should move
//    {
//        Vector3 point = target.position + target.transform.forward * target.velocity.magnitude * timePrediction;

//        // Direction from the AI agent's position to the predicted future position of the target
//        Vector3 dirToPoint = (point - entity.position).normalized;

//        // Direction from the AI agent's position to the current position of the target
//        Vector3 dirToTarget = (target.position - entity.position).normalized;

//        if (Vector3.Dot(dirToPoint, dirToTarget) < 0)
//        {
//            dirToPoint = dirToTarget;
//        }

//        return dirToPoint;
//    }
//}
