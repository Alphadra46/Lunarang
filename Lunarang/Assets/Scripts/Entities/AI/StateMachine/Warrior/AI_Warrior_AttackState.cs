using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Warrior_AttackState : BaseState<AI_Warrior_StateMachine.EnemyState>
{
    public AI_Warrior_AttackState(AI_Warrior_StateMachine.EnemyState key, AI_Warrior_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_Warrior_StateMachine _aiStateMachine;

    /// <summary>
    /// Call Attack function and launch the coroutine End Attack.
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.StartCoroutine(Attack(_aiStateMachine.atkDuration));
    }

    public override void ExitState()
    {
        _aiStateMachine.StopCoroutine(Attack(_aiStateMachine.atkDuration));
    }

    public override void UpdateState()
    {
        // if (!_aiStateMachine._stats.isDead) return;
        //
        // _aiStateMachine.StopCoroutine(EndAttack(_aiStateMachine.atkDuration));
        // _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Death);
    }
    
    private IEnumerator Attack(float delay)
    {
        
        _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");
        _aiStateMachine.canRotate = false;
        
        yield return new WaitForSeconds(delay);
        
        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
        _aiStateMachine.canRotate = true;
        
    }
    
    public override AI_Warrior_StateMachine.EnemyState GetNextState()
    {
        return AI_Warrior_StateMachine.EnemyState.Attack;
    }
    
    
}
