using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState<T> : State<T>
{
    private PlayerModel _model;
    private PlayerView _view;

    float x;
    float z;

    public PlayerIdleState(PlayerModel model, PlayerView view)
    {
        _model = model;
        _view = view;
    }

    public override void Enter()
    {
        base.Enter();

    }


    // Update is called once per frame
    public override void Execute()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        _model.Move(x, z);
    }
}
