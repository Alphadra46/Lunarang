using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_DeathState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_DeathState(AI_StateMachine.EnemyState key, AI_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Switch to Patrol State.
    /// </summary>
    public override void EnterState()
    {
        //TODO Replace this by Death Animation
        
        _aiStateMachine._stats.Invoke(nameof(SC_AIStats.Death), 1f);
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Patrol;
    }

    /// <summary>
    /// Delay before switching to Patrol State.
    /// </summary>
    IEnumerator TransitionPatrol()
    {
        yield return new WaitForSeconds(_aiStateMachine.idleDelay);
        _aiStateMachine.TransitionToState(AI_StateMachine.EnemyState.Patrol);
    }
    
}
