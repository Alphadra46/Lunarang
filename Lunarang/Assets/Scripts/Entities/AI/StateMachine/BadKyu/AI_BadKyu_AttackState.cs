using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_BadKyu_AttackState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_BadKyu_AttackState(AI_StateMachine.EnemyState key, AI_BadKyu_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_BadKyu_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Spawn a projectile and directly back to Chase State.
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.SpawnProjectile();
        if(_aiStateMachine.currentProjectiles == 3) _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Attack;
    }
    
    
}
