// Composite "OR" node: tries children in order and stops at the first one that
// doesn't fail. Reads as "try plan A; if it can't run, try plan B; ...".
public class BTSelector : IBTNode
{
    readonly IBTNode[] _children;

    public BTSelector(params IBTNode[] children)
    {
        _children = children;
    }

    public BTStatus Tick()
    {
        for (int i = 0; i < _children.Length; i++)
        {
            var status = _children[i].Tick();
            if (status != BTStatus.Failure) return status;
        }
        return BTStatus.Failure;
    }
}
