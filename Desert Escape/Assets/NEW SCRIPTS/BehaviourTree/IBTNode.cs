// Behaviour Tree core: unlike the binary decision tree (QuestionNode/ActionNode) used
// by the Enemy, every BT node reports back how it went, which is what lets composite
// nodes (Selector/Sequence) chain children with fallbacks.
public enum BTStatus
{
    Success,
    Failure,
    Running
}

public interface IBTNode
{
    BTStatus Tick();
}
