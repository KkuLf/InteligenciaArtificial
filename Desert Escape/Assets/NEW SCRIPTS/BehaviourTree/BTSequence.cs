// Composite "AND" node: runs children in order and aborts at the first one that
// fails. Reads as "if condition holds, do step 1, then step 2, ...".
public class BTSequence : IBTNode
{
    readonly IBTNode[] _children;

    public BTSequence(params IBTNode[] children)
    {
        _children = children;
    }

    public BTStatus Tick()
    {
        for (int i = 0; i < _children.Length; i++)
        {
            var status = _children[i].Tick();
            if (status != BTStatus.Success) return status;
        }
        return BTStatus.Success;
    }
}
