//using UnityEngine;

//public class Seek : ISteering
//{
//    private Transform entity;           // AI agent's transform
//    private Transform target;           // Target's transform

//    public Seek(Transform entity, Transform target)     // Initializes entity and target fields and gives them values
//    {
//        this.entity = entity;
//        this.target = target;
//    }

//    public Vector3 GetDir()     // Calculates the direction in which the AI should move
//    {
//        // 1st Subtracts the AI's position from the target's position to get a vector pointing from the AI agent to the target
//        // 2nd Normalizes this vector to ensure it has a magnitude of 1, representing only the direction
//        // 3rd Returns this normalized direction vector, indicating the direction in which the AI agent should move to reach the target
//        return (target.position - entity.position).normalized;
//    }
//}
