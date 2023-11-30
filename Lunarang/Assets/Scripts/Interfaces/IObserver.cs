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
    /// <param name="newCurrentHP">The current hp of the player</param>
    /// <param name="newMaxHP">The max hp of the player</param>
    public void OnNotify(float newCurrentHP, float newMaxHP);
}
