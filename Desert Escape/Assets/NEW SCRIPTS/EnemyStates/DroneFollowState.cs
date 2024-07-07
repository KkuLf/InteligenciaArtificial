using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneFollowState<T> : State<T>
{
    ISteering _steering;
    DroneModel _model;
    ObstacleAvoidanceV2 _obs;

    public DroneFollowState(DroneModel model, ISteering steering, ObstacleAvoidanceV2 obs)
    {
        _steering = steering;
        _model = model;
        _obs = obs;
    }
    public override void Execute()
    {
        Debug.Log("ENTERED FOLLOW");
        var dir = _obs.GetDir(_steering.GetDir(), false);
        _model.Move(dir);
        _model.LookDir(dir);
    }
}
