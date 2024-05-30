using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    IPlayerModel _player;
    IPlayerView _view;

    FSM<StatesEnum> _fsm;
    private void Awake()
    {
        _player = GetComponent<IPlayerModel>();
        _view = GetComponent<IPlayerView>();
        InitializeFSM();
    }

    void InitializeFSM()
    {
        _fsm = new FSM<StatesEnum>();

        var idle = new PlayerStateIdle<StatesEnum>(StatesEnum.Walk);
        var walk = new PlayerStateWalk<StatesEnum>(_player, _view, StatesEnum.Idle);

        idle.AddTransition(StatesEnum.Walk, walk);
        walk.AddTransition(StatesEnum.Idle, idle);

        _fsm.SetInit(idle);
    }
    void Update()
    {
        _fsm.OnUpdate();
    }
    public void ChangeModel(IPlayerModel model)
    {
        _player = model;
    }
}
