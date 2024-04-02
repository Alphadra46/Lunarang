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
        _aiStateMachine.StartCoroutine(Attack(_aiStateMachine.atkDuration));
    }

    public override void ExitState()
    {
        // _aiStateMachine.StopCoroutine(Attack(_aiStateMachine.atkDuration));
    }

    public override void UpdateState()
    {
    }

    /// <summary>
    /// After a certain delay, deactivate the hurtbox and switch to Chase State.
    /// </summary>
    /// <param name="delay">Delay in seconds before switching state.</param>
    private IEnumerator Attack(float delay)
    {

        _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");
        _aiStateMachine.canRotate = false;

        yield return new WaitForEndOfFrame();
        _aiStateMachine.CanAttack(false);
        
        yield return new WaitForSeconds(delay);
        
        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
        _aiStateMachine.canRotate = true;
        _aiStateMachine.CanAttack(true);
        
    }
    
    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Attack;
    }
    
    
}
