using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_AttackState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_Archer_AttackState(AI_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_Archer_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Spawn a projectile and directly back to Chase State.
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.StartCoroutine(Attack());
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
    private IEnumerator Attack()
    {

        _aiStateMachine.agent.isStopped = true;
        _aiStateMachine.agent.SetDestination(_aiStateMachine.transform.position);
        
        _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");

        yield return new WaitForSeconds(_aiStateMachine.atkBlockRotationDelay);
        
        _aiStateMachine.canRotate = false;
        
        yield return new WaitForSeconds(_aiStateMachine.atkDuration);
        
        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
        _aiStateMachine.canRotate = true;
        
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Chase;
    }
    
    
}
