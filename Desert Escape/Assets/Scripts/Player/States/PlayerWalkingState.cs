using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkingState<T> : State<T>
{
    private PlayerModel _model;
    private PlayerView _view;
    private PlayerController _controller;

    float x;
    float z;

    public PlayerWalkingState(PlayerModel model, PlayerView view, PlayerController controller)
    {
        _model = model;
        _view = view;
        _controller = controller;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");

        _model.Move(x, z);

    }
}
