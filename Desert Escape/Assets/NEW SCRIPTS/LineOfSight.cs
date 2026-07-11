using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour, ILineOfSight
{
    public float range;
    [Range(1, 360)]
    public float angle;
    public LayerMask maskObs;

    // Large enough to not get flooded/truncated by all the pathfinding Node colliders
    // packed into the map before ever reaching the actual target's collider.
    readonly Collider[] _overlapBuffer = new Collider[64];

    public float Range => range;
    public float Angle => angle;

    public bool CheckRange(Transform target)
    {
        return CheckRange(target, range);
    }
    public bool CheckRange(Transform target, float range)
    {
        // Overlap sensor: is the target's collider inside a sphere around Origin,
        // instead of a raw distance comparison.
        int count = Physics.OverlapSphereNonAlloc(Origin, range, _overlapBuffer);
        for (int i = 0; i < count; i++)
        {
            // Compare by hierarchy root, not exact transform - the collider that
            // actually overlaps might sit on a child of the target (e.g. a hitbox),
            // not necessarily on the target Transform itself.
            if (_overlapBuffer[i] != null && _overlapBuffer[i].transform.root == target.root) return true;
        }
        return false;
    }
    public bool CheckAngle(Transform target)
    {
        return CheckAngle(target, angle);
    }
    public bool CheckAngle(Transform target, float angle)
    {
        // Dot-product sensor: the cosine of the angle between Forward and the direction
        // to the target is exactly their dot product (both normalized).
        Vector3 dirToTarget = (target.position - Origin).normalized;
        float dot = Vector3.Dot(Forward.normalized, dirToTarget);
        float cosHalfAngle = Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad);
        return dot >= cosHalfAngle;
    }
    public bool CheckView(Transform target)
    {
        return CheckView(target, maskObs);
    }
    public bool CheckView(Transform target, LayerMask maskObs)
    {
        Vector3 dirToTarget = target.position - Origin;
        float distance = dirToTarget.magnitude;
        return !Physics.Raycast(Origin, dirToTarget, distance, maskObs);
    }
    Vector3 Origin => transform.position;
    Vector3 Forward => transform.forward;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Origin, range);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Origin, Quaternion.Euler(0, angle / 2, 0) * Forward * range);
        Gizmos.DrawRay(Origin, Quaternion.Euler(0, -(angle / 2), 0) * Forward * range);
    }
}
