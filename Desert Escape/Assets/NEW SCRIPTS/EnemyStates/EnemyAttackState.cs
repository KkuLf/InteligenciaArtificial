using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState<T> : State<T>
{
    EnemyModel _model;

    public EnemyAttackState(EnemyModel model)
    {
        _model = model;
    }

    public override void Execute()
    {
        base.Execute();
        _model.Attack();
    }
}
