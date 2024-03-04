using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AI_FreezeState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_FreezeState(AI_StateMachine.EnemyState key, AI_StateMachine manager) : base(key, manager)
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
        _aiStateMachine.agent.isStopped = true;
    }

    public override void ExitState()
    {
        _aiStateMachine.agent.isStopped = false;
    }

    public override void UpdateState()
    {
        
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Idle;
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