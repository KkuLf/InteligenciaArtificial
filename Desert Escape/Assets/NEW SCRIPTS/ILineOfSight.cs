using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILineOfSight
{
    bool CheckRange(Transform target);
    bool CheckRange(Transform target, float range);
    bool CheckAngle(Transform target);
    bool CheckAngle(Transform target, float angle);
    bool CheckView(Transform target);
    bool CheckView(Transform target, LayerMask maskObs);
}
