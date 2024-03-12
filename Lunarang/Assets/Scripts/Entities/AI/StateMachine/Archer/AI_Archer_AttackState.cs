using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_AttackState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_Archer_AttackState(AI_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key, manager)
    {
        _aiArcherStateMachine = manager;
    }
    
    private readonly AI_Archer_StateMachine _aiArcherStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Spawn a projectile and directly back to Chase State.
    /// </summary>
    public override void EnterState()
    {
        _aiArcherStateMachine.SpawnProjectile();
        _aiArcherStateMachine.TransitionToState(AI_StateMachine.EnemyState.Chase);
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
        _aiArcherStateMachine.TransitionToState(AI_Archer_StateMachine.EnemyState.Chase);
        
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Chase;
    }
    
    
}
