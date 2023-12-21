using UnityEngine.Events;

[CreateNodeMenu("Event")]
[NodeTint("#ebb734")]
[NodeWidth(300)]
public class Event : BaseNode
{
    [Input(backingValue = ShowBackingValue.Never)] public BaseNode input;
    [Output(backingValue = ShowBackingValue.Never)] public BaseNode output;
    
    public SerializableEvent[] events;
    
    public void Trigger()
    {
        foreach (var e in events)
        {
            e.Invoke();
        }
    }

    public override string GetString()
    {
        return "Event";
    }
}
