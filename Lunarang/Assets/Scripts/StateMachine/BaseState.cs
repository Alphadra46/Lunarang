using System;
using UnityEngine;

public abstract class BaseState<EState> where EState : System.Enum
{
   
   /// <summary>
   /// Initialize State
   /// </summary>
   /// <param name="key">Associate this state with a enum key.</param>
   /// <param name="manager">Manager of this state.</param>
   protected BaseState(EState key, StateManager<EState> manager)
   {
      StateKey = key;
      Manager = manager;
   }
    
   public EState StateKey { get; private set; }
   
   public StateManager<EState> Manager { get; private set; }
   
   /// <summary>
   /// Called when entering in this state.
   /// </summary>
   public abstract void EnterState();
   
   /// <summary>
   /// Called when exiting in this state.
   /// </summary>
   public abstract void ExitState();
   
   /// <summary>
   /// Called every tick when in this state.
   /// </summary>
   public abstract void UpdateState();
   
   /// <summary>
   /// Get the state that follows this one.
   /// </summary>
   /// <returns>Next State</returns>
   public abstract EState GetNextState();
   
}
