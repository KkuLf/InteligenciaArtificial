using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evade : ISteering
{
    Pursuit _pursuit;

    public Evade(Transform entity, Rigidbody target, float timePrediction)
    {
        _pursuit = new Pursuit(entity, target, timePrediction);
    }
    public Vector3 GetDir()
    {
        return -_pursuit.GetDir();
    }
}

