using UnityEngine;

// The enemy just lost sight of the player: it stands still and "thinks" for a
// while instead of immediately giving up, then falls back to Patrol if the
// player isn't reacquired in time.
public class EnemySearchState<T> : State<T>
{
    readonly EnemyBlackboard _bb;
    readonly float _duration;
    float _timer;

    public EnemySearchState(EnemyBlackboard blackboard, float duration)
    {
        _bb = blackboard;
        _duration = duration;
    }

    public override void Enter()
    {
        base.Enter();
        _timer = 0f;
        _bb.Model.Move(Vector3.zero); // stop dead instead of coasting on whatever velocity Chase left behind
    }

    public override void Execute()
    {
        base.Execute();
        _timer += Time.deltaTime;
    }

    public bool TimedOut => _timer >= _duration;
}
