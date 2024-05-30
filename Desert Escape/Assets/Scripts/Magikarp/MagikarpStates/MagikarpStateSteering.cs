using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagikarpStateSteering<T> : State<T>
{
    ISteering _steering;
    Magikarp _magikarp;
    ObstacleAvoidanceV2 _obs;
    public MagikarpStateSteering(Magikarp magikarp, ISteering steering, ObstacleAvoidanceV2 obs)
    {
        _steering = steering;
        _magikarp = magikarp;
        _obs = obs;
    }
    public override void Execute()
    {
        var dir = _obs.GetDir(_steering.GetDir(), false);
        _magikarp.Move(dir);
        _magikarp.LookDir(dir);
    }
}
