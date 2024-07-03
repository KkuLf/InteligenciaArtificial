using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseState<T> : State<T>
{
    ISteering _pursuit;
    EnemyModel _model;
    ObstacleAvoidanceV2 _obs;
    Transform _target;
    public EnemyChaseState(EnemyModel model, Transform target, ISteering pursuit, ObstacleAvoidanceV2 obs)
    {
        _pursuit = pursuit;
        _model = model;
        _obs = obs;
        _target = target;
    }
    public override void Execute()
    {
        var dir = _obs.GetDir(_pursuit.GetDir(), false);
        _model.Move(_model.transform.forward);
        _model.LookDir(dir);
    }
}
