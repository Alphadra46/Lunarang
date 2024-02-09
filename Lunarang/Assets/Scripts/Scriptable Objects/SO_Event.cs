using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "SO/Events", fileName = "SO_Event")]
public class SO_Event : ScriptableObject
{
    
    public UnityAction OnEventRaised;
    
    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
    
}