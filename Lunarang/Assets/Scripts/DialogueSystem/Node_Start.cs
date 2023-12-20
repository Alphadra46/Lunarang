[CreateNodeMenu("Start")]
[NodeTint("#6e34eb")]
[NodeWidth(100)]
public class Node_Start : Node_Base
{
    [Output] public int exit;

    public override string GetString()
    {
        return "Start";
    }
    
}
