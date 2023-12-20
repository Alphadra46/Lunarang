using XNode;

[CreateNodeMenu("End")]
public class Node_End : Node_Base
{
    
    [Input] public int entry;

    public override string GetString()
    {
        return "End";
    }
}
