using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Bully_StunState : BaseState<AI_Bully_StateMachine.EnemyState>
{
    public AI_Bully_StunState(AI_Bully_StateMachine.EnemyState key, AI_Bully_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_Bully_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;
    
    /// <summary>
    /// Start a coroutine to stun the entity for a certain duration.
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.StartCoroutine(StunTimer(_aiStateMachine.stunDuration));
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }

    /// <summary>
    /// Stun the entity for a certain duration, and after switch to Chase State.
    /// </summary>
    /// <param name="duration">Duration of the stun.</param>
    public IEnumerator StunTimer(float duration)
    {

        yield return new WaitForSeconds(duration);

        _aiStateMachine.TransitionToState(AI_Bully_StateMachine.EnemyState.Chase);
        
    }

    public override AI_Bully_StateMachine.EnemyState GetNextState()
    {
        return AI_Bully_StateMachine.EnemyState.Chase;
    }

}
