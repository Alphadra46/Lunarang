using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class SC_Subject : MonoBehaviour
{
    [HideInInspector]
    public List<IObserver> observers = new List<IObserver>();
    
    /// <summary>
    /// Subscribe the subject to an observer
    /// </summary>
    /// <param name="observer">The observer to subscribe</param>
    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }
    
    /// <summary>
    /// Remove an observer from the list of subscribed observers
    /// </summary>
    /// <param name="observer">The observer to remove</param>
    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    /// <summary>
    /// Notify all observers the subject have subscribed to
    /// </summary>
    protected void NotifyObservers(float newCurrentHP, float newMaxHP)
    {
        observers.ForEach((observer) =>
        {
            observer.OnNotify(newCurrentHP,newMaxHP);
        });
    }
    
    
    /// <summary>
    /// Notify all observers the subject have subscribed to
    /// </summary>
    protected void NotifyObservers()
    {
        observers.ForEach((observer) =>
        {
            observer.OnNotify();
        });
    }
    
    /// <summary>
    /// Notify all observers the subject have subscribed to
    /// </summary>
    protected void NotifyObservers(string context)
    {
        observers.ForEach((observer) =>
        {
            observer.OnNotify(context, this);
        });
    }
}
