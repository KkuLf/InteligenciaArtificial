using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState<T> : State<T>
{
    private PlayerModel _model;

    public PlayerDeathState(PlayerModel model)
    {
        _model = model;
    }

    public override void Enter()
    {
        base.Enter();

        _model.RIP();
    }
}
