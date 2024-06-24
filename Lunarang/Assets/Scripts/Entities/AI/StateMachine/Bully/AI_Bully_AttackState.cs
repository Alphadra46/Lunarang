using System.Collections;
using System.Collections.Generic;
using Enum;
using UnityEngine;
using UnityEngine.AI;

public class AI_Bully_AttackState : BaseState<AI_Bully_StateMachine.EnemyState>
{
    public AI_Bully_AttackState(AI_Bully_StateMachine.EnemyState key, AI_Bully_StateMachine manager) : base(key,
        manager)
    {
        _aiStateMachine = manager;
    }

    private readonly AI_Bully_StateMachine _aiStateMachine;

    /// <summary>
    /// Call Attack function and launch the coroutine End Attack.
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.StartCoroutine(Attack());
    }

    public override void ExitState()
    {
        _aiStateMachine.StartCoroutine(_aiStateMachine.AttackCD());
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
       
        // Basic Attack
        _aiStateMachine.agent.isStopped = true;
        _aiStateMachine.agent.velocity = Vector3.zero;
        _aiStateMachine.agent.SetDestination(_aiStateMachine.transform.position);

        _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");
        _aiStateMachine.canRotate = false;

        yield return new WaitForSeconds(_aiStateMachine.atkDuration);
        
        if(_aiStateMachine._debuffsBuffsComponent.CheckHasDebuff(Enum_Debuff.Freeze)) yield break;

        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
        _aiStateMachine.agent.enabled = true;
        _aiStateMachine.canRotate = true;
        
        // _aiStateMachine.canCharge = true;
        _aiStateMachine.StartCoroutine(_aiStateMachine.AttackCD());

        if (!_aiStateMachine.canShield) yield break;
        
        _aiStateMachine._stats.CreateShield(_aiStateMachine._stats.shieldValue);
        _aiStateMachine.StartCoroutine(_aiStateMachine.ShieldCD());
        
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Attack;
    }
}