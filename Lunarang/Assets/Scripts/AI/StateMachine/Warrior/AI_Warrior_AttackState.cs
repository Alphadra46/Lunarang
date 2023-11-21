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
    private NavMeshAgent _agent;

    public override void EnterState()
    {
        _aiStateMachine.Attack();
        _aiStateMachine.StartCoroutine(EndAttack(_aiStateMachine.atkDuration));
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
    }

    public IEnumerator EndAttack(float delay)
    {

        yield return new WaitForSeconds(delay);
        _aiStateMachine.hurtBox.SetActive(false);
        _aiStateMachine.TransitionToState(AI_Warrior_StateMachine.EnemyState.Chase);
        

    }
    
    public override AI_Warrior_StateMachine.EnemyState GetNextState()
    {
        return AI_Warrior_StateMachine.EnemyState.Attack;
    }
    
    
}
