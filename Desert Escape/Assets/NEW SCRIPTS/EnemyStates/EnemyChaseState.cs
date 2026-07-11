using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState<T> : State<T>
{
    ISteering _pursuit;
    EnemyModel _model;
    ObstacleAvoidanceV2 _obs;
    Transform _target;

    public EnemyChaseState(EnemyModel model, ISteering pursuit, ObstacleAvoidanceV2 obs, Transform target)
    {
        _pursuit = pursuit;
        _model = model;
        _obs = obs;
        _target = target;
    }
    public override void Execute()
    {
        // Movement leads the target (Pursuit predicts ahead using its velocity), but
        // facing/LOS tracks the target's actual current position - otherwise the vision
        // cone swings with Pursuit's prediction and loses the player just from strafing.
        var moveDir = _obs.GetDir(_pursuit.GetDir(), false);
        _model.Move(moveDir);
        _model.LookDir(_target.position - _model.transform.position);
    }
}
