using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;

public class SC_Subject : MonoBehaviour
{
    [HideInInspector]
    public List<IObserver> observers = new List<IObserver>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
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
}
