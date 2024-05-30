using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWalk<T> : State<T>
{
    IPlayerModel _player;
    IPlayerView _view;
    T _idleInput;
    public PlayerStateWalk(IPlayerModel player, IPlayerView view, T idleInput)
    {
        _player = player;
        _view = view;
        _idleInput = idleInput;
    }
    public override void Execute()
    {
        base.Execute();
        Debug.Log("Walk");
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(x, 0, z).normalized;
        _player.Move(dir);
        _player.LookDir(dir);

        if (x == 0 && z == 0)
        {
            //Transition
            _fsm.Transition(_idleInput);
        }
    }
}
