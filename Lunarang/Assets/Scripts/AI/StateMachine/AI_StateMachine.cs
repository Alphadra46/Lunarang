using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_StateMachine : StateManager<AI_StateMachine.EnemyState>
{
    
    public enum EnemyState
    {
        
        Idle,
        Patrol,
        Chase,
        Attack,
        Riposte
        
    }
    

    private void Awake()
    {
        CurrentState = States[EnemyState.Idle];
    }
    
}
