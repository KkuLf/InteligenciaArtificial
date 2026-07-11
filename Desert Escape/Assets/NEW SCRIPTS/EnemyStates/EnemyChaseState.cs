using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState<T> : State<T>
{
    EnemyBlackboard _bb;

    public EnemyChaseState(EnemyBlackboard blackboard)
    {
        _bb = blackboard;
    }
    public override void Execute()
    {
        // Movement leads the target (Pursuit predicts ahead using its velocity), but
        // facing/LOS tracks the target's actual current position - otherwise the vision
        // cone swings with Pursuit's prediction and loses the player just from strafing.
        var moveDir = _bb.ObstacleAvoidance.GetDir(_bb.Pursuit.GetDir(), false);
        _bb.Model.Move(moveDir);
        _bb.Model.LookDir(_bb.Target.position - _bb.Model.transform.position);
    }
}
