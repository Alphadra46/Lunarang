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
        Debug.Log("TA MERE");
        _aiStateMachine.Attack();
        _aiStateMachine.TransitionToState(AI_StateMachine.EnemyState.Chase);
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
    
    
    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Attack;
    }
    
    
}
