using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_IdleState : BaseState<AI_Archer_StateMachine.EnemyState>
{
    public AI_Archer_IdleState(AI_Archer_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key, manager)
    {
        _aiArcherStateMachine = manager;
    }
    
    private readonly AI_Archer_StateMachine _aiArcherStateMachine;
    private NavMeshAgent _agent;

    /// <summary>
    /// Switch to Patrol State.
    /// </summary>
    public override void EnterState()
    {
        _aiArcherStateMachine.StartCoroutine(TransitionPatrol());
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }

    public override AI_Archer_StateMachine.EnemyState GetNextState()
    {
        return AI_Archer_StateMachine.EnemyState.Patrol;
    }

    /// <summary>
    /// Delay before switching to Patrol State.
    /// </summary>
    IEnumerator TransitionPatrol()
    {
        yield return new WaitForSeconds(_aiArcherStateMachine.idleDelay);
        _aiArcherStateMachine.TransitionToState(AI_Archer_StateMachine.EnemyState.Patrol);
    }
    
}
