using System;

// Leaf node that does actual work and reports how it went - Running while still
// busy (e.g. moving somewhere), Success when done, Failure if it couldn't act.
public class BTAction : IBTNode
{
    readonly Func<BTStatus> _action;

    public BTAction(Func<BTStatus> action)
    {
        _action = action;
    }

    public BTStatus Tick()
    {
        return _action();
    }
}
