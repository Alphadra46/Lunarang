using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : System.Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    [ShowInInspector, PropertySpace(SpaceAfter = 10)] protected BaseState<EState> CurrentState;
    [ShowInInspector, PropertySpace(SpaceAfter = 10)] public EState NextState;
    
    protected bool IsTransitionState = false;
    

    /// <summary>
    /// Call current state start function.
    /// </summary>
    private void Start()
    {
        CurrentState.EnterState();
    }

    /// <summary>
    /// Call current state update function.
    /// </summary>
    private void Update()
    {
        CurrentState.UpdateState();
    }

    /// <summary>
    /// Switch to another state.
    /// </summary>
    /// <param name="stateKey">Next State</param>
    public void TransitionToState(EState stateKey)
    {
        NextState = stateKey;
        
        IsTransitionState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitionState = false;
        
    }
}
