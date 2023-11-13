using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_DefenseState : BaseState<AI_Archer_StateMachine.EnemyState>
{
    public AI_Archer_DefenseState(AI_Archer_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key, manager)
    {
        _aiArcherStateMachine = manager;
    }
    
    private readonly AI_Archer_StateMachine _aiArcherStateMachine;
    private NavMeshAgent _agent;
    

    public override void EnterState()
    {
        // _aiArcherStateMachine._rb.AddForce(-_aiArcherStateMachine.centerPoint.transform.forward * _aiArcherStateMachine.dashSpeed, ForceMode.Impulse);
        _aiArcherStateMachine.StartCoroutine(Dash());
    }

    public override void ExitState()
    {
        _aiArcherStateMachine._rb.velocity = Vector3.zero;
    }

    public override void UpdateState()
    {
        
    }

    public override AI_Archer_StateMachine.EnemyState GetNextState()
    {
        return AI_Archer_StateMachine.EnemyState.Chase;
    }

    IEnumerator Dash()
    {
        var timer = _aiArcherStateMachine.dashDuration;
        
        _aiArcherStateMachine._rb.isKinematic = false;
        
        while (timer > 0)
        {
            
            _aiArcherStateMachine._rb.velocity = new Vector3(
                -_aiArcherStateMachine.centerPoint.transform.forward.x * _aiArcherStateMachine.dashSpeed,
                _aiArcherStateMachine._rb.velocity.y,
                -_aiArcherStateMachine.centerPoint.transform.forward.z * _aiArcherStateMachine.dashSpeed);
            
            timer -= Time.deltaTime;
            yield return null;
        }
        
        _aiArcherStateMachine._rb.isKinematic = true;
        _aiArcherStateMachine.TransitionToState(AI_Archer_StateMachine.EnemyState.Chase);
        
    }
}
