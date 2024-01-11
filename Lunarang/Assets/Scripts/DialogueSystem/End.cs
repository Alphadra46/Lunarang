using XNode;

[CreateNodeMenu("End")]
[NodeTint("#e63c3c")]
[NodeWidth(100)]
public class End : BaseNode
{
    [Input(backingValue = ShowBackingValue.Never)] public BaseNode input;

    public override string GetString()
    {
        return "End";
    }
}
