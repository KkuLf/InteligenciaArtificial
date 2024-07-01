using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneStateSteering<T> : State<T>
{
    ISteering _steering;
    Drone _drone;
    ObstacleAvoidanceV2 _obs;
    public DroneStateSteering(Drone drone, ISteering steering, ObstacleAvoidanceV2 obs)
    {
        _steering = steering;
        _drone = drone;
        _obs = obs;
    }
    public override void Execute()
    {
        var dir = _obs.GetDir(_steering.GetDir(), false);
        _drone.Move(dir);
        _drone.LookDir(dir);
    }
}