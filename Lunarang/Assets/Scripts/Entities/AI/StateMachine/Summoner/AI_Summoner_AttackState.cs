using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Summoner_AttackState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_Summoner_AttackState(AI_StateMachine.EnemyState key, AI_Summoner_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_Summoner_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Call Attack function and launch the coroutine End Attack.
    /// </summary>
    public override void EnterState()
    {
        if (_aiStateMachine.canSummon)
        {
            _aiStateMachine.StartCoroutine(Summon());
        }
        else
        {
            _aiStateMachine.StartCoroutine(Attack());
        }
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
    private IEnumerator Summon()
    {
        
        _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");
        _aiStateMachine.Attack();
        
        yield return new WaitForSeconds(_aiStateMachine.atkDuration);
        
        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
        
    }
    
    private IEnumerator Attack()
    {
        
        // _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");
        
        _aiStateMachine.Attack();
        yield return new WaitForEndOfFrame();
        
        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
        
    }
    
    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Attack;
    }
    
    
}
