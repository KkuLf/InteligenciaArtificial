using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState<T> : State<T>
{
    EnemyModel _model;
    Transform _target;

    public EnemyAttackState(EnemyModel model, Transform target)
    {
        _model = model;
        _target = target;
    }

    public override void Execute()
    {
        base.Execute();
        _model.Attack(_target.position);
    }
}
