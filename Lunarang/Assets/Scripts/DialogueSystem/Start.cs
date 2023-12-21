using XNode;

[CreateNodeMenu("Start")]
[NodeTint("#6e34eb")]
[NodeWidth(100)]
public class Start : BaseNode
{
    [Output(backingValue = ShowBackingValue.Never)] public BaseNode output;
    
    public override string GetString()
    {
        return "Start";
    }
    
}
