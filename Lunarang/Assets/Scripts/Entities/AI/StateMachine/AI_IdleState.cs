using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_IdleState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_IdleState(AI_StateMachine.EnemyState key, AI_StateMachine manager) : base(key, manager)
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
        _aiStateMachine.agent.enabled = true;
        _aiStateMachine._collider.enabled = true;
        _aiStateMachine.StartCoroutine(TransitionPatrol());

        _aiStateMachine.canAttack = true;
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
        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Patrol);
    }
    
}
