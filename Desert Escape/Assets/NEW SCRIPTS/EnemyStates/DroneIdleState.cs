using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneIdleState<T>: State<T>
{
    DroneModel _model;

    public DroneIdleState(DroneModel model)
    {
        _model = model;
    }

    public override void Execute()
    {
        _model.Move(Vector3.zero);
    }
}
