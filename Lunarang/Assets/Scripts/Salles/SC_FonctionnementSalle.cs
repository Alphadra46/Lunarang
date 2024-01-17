using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SC_FonctionnementSalle : MonoBehaviour
{
    public bool IsRoomCleared = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        IsRoomCleared = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Will setup the room according to the type and parameters of this room
    /// </summary>
    protected abstract void SetupRoom();
}
