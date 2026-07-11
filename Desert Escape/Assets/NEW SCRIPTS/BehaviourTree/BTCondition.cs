using System;

// Leaf node wrapping a yes/no check: Success if the predicate holds, Failure if not.
// Typically the first child of a BTSequence, gating whether its actions run.
public class BTCondition : IBTNode
{
    readonly Func<bool> _predicate;

    public BTCondition(Func<bool> predicate)
    {
        _predicate = predicate;
    }

    public BTStatus Tick()
    {
        return _predicate() ? BTStatus.Success : BTStatus.Failure;
    }
}
