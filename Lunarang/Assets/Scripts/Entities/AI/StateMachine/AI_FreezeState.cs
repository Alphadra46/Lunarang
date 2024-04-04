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
        _aiStateMachine.agent.SetDestination(_aiStateMachine.transform.position);
        _aiStateMachine.agent.velocity = Vector3.zero;
        
        _aiStateMachine._renderer.PauseAnimator(true);
    }

    public override void ExitState()
    {
        _aiStateMachine.agent.isStopped = false;
        _aiStateMachine._renderer.PauseAnimator(false);
    }

    public override void UpdateState()
    {
        
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Idle;
    }
    
}