using System;
using UnityEngine;

public abstract class BaseState<EState> where EState : System.Enum
{
   public BaseState(EState key, StateManager<EState> manager)
   {
      StateKey = key;
      Manager = manager;
   }
   
   public EState StateKey { get; private set; }
   
   public StateManager<EState> Manager { get; private set; }
   
   public abstract void EnterState();
   public abstract void ExitState();
   public abstract void UpdateState();
   public abstract EState GetNextState();
   
}
