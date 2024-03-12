using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Bully_IdleState : BaseState<AI_Bully_StateMachine.EnemyState>
{
    public AI_Bully_IdleState(AI_Bully_StateMachine.EnemyState key, AI_Bully_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_Bully_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Switch to Patrol State
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.StartCoroutine(TransitionPatrol());
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }

    public override AI_Bully_StateMachine.EnemyState GetNextState()
    {
        return AI_Bully_StateMachine.EnemyState.Patrol;
    }

    /// <summary>
    /// Switch to Patrol State after a certain delay
    /// </summary>
    /// <returns></returns>
    IEnumerator TransitionPatrol()
    {
        yield return new WaitForSeconds(_aiStateMachine.idleDelay);
        _aiStateMachine.TransitionToState(AI_Bully_StateMachine.EnemyState.Patrol);
    }
    
}
