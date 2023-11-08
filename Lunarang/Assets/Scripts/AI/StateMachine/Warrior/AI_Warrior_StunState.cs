using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Warrior_StunState : BaseState<AI_Warrior_StateMachine.EnemyState>
{
    public AI_Warrior_StunState(AI_Warrior_StateMachine.EnemyState key, AI_Warrior_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_Warrior_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;
    

    public override void EnterState()
    {
        Debug.Log("DASH");
        // _aiArcherStateMachine._rb.AddForce(-_aiArcherStateMachine.centerPoint.transform.forward * _aiArcherStateMachine.dashSpeed, ForceMode.Impulse);
    }

    public override void ExitState()
    {
        Debug.Log("DASH END");
        _aiStateMachine._rb.velocity = Vector3.zero;
    }

    public override void UpdateState()
    {
        
    }

    public override AI_Warrior_StateMachine.EnemyState GetNextState()
    {
        return AI_Warrior_StateMachine.EnemyState.Chase;
    }

}
