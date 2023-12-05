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
    
    /// <summary>
    /// All actions performed by executing the command.
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public abstract void Execute(string[] args);
    
}
