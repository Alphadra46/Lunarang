using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SC_Command
{
    public abstract string descText { get; }
    
    public abstract void Execute(string[] args);
    
}
