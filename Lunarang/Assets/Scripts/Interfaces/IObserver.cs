using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    /// <summary>
    /// Used by subject to communicate with the observer
    /// </summary>
    public void OnNotify();

    /// <summary>
    /// Used by subject to communicate with the observer
    /// </summary>
    /// <param name="context">A message to help the observer find which type of subject send this notification</param>
    /// <param name="subjectReference">Reference to the subject for additional context</param>
    public void OnNotify(string context, SC_Subject subjectReference);
    
    /// <summary>
    /// Used by subject to communicate with the observer
    /// </summary>
    /// <param name="newCurrentHP">The current hp of the player</param>
    /// <param name="newMaxHP">The max hp of the player</param>
    public void OnNotify(float newCurrentHP, float newMaxHP);
}
