using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState<T> : State<T>
{
    EnemyBlackboard _bb;

    public EnemyAttackState(EnemyBlackboard blackboard)
    {
        _bb = blackboard;
    }

    public override void Execute()
    {
        base.Execute();
        _bb.Model.Attack(_bb.Target.position);
    }
}
