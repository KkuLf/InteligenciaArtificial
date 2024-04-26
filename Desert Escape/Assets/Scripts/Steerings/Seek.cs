using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : ISteering
{

    Transform _entity;
    Transform _target;

    public Seek(Transform entity, Transform target)
    {
        _entity = entity;
        _target = target;
    }

    public Vector3 GetDir()
    {
        //b: target
        //a: entity

        Vector3 dir = (_target.position - _entity.position).normalized;
        return dir;

    }



}
