using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour, ILineOfSight
{
    public float range;
    [Range(1, 360)]
    public float angle;
    public LayerMask maskObs;

    public bool CheckRange(Transform target)
    {
        return CheckRange(target, range);
    }
    public bool CheckRange(Transform target, float range)
    {
        float distance = Vector3.Distance(target.position, Origin);
        return distance <= range;
    }
    public bool CheckAngle(Transform target)
    {
        return CheckAngle(target, angle);
    }
    public bool CheckAngle(Transform target, float angle)
    {
        Vector3 dirToTarget = target.position - Origin;
        float angleToTarget = Vector3.Angle(Forward, dirToTarget);
        return angleToTarget <= angle / 2;
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
