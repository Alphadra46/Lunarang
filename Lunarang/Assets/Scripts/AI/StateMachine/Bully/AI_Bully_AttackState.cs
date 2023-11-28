using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Bully_AttackState : BaseState<AI_Bully_StateMachine.EnemyState>
{
    public AI_Bully_AttackState(AI_Bully_StateMachine.EnemyState key, AI_Bully_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_Bully_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Call Attack function and launch the coroutine End Attack.
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.Attack();
        _aiStateMachine.StartCoroutine(EndAttack(_aiStateMachine.atkDuration));
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
    }

    /// <summary>
    /// After a certain delay, deactivate the hurtbox and switch to Chase State.
    /// </summary>
    /// <param name="delay">Delay in seconds before switching state.</param>
    public IEnumerator EndAttack(float delay)
    {

        yield return new WaitForSeconds(delay);
        _aiStateMachine.hurtBox.SetActive(false);
        _aiStateMachine.TransitionToState(AI_Bully_StateMachine.EnemyState.Chase);
        
    }
    
    public override AI_Bully_StateMachine.EnemyState GetNextState()
    {
        return AI_Bully_StateMachine.EnemyState.Attack;
    }
    
    
}
