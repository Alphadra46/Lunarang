using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "SO/Events", fileName = "SO_Event_Parameter")]
public class SO_Event_Paramater<T> : ScriptableObject
{
    
    public UnityAction<T> OnEventRaised;
    
    public void RaiseEvent(T parameter)
    {
        OnEventRaised?.Invoke(parameter);
    }
    
}
