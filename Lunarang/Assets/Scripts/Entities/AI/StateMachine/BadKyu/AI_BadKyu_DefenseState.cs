using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_BadKyu_DefenseState : BaseState<AI_StateMachine.EnemyState>
{
    public AI_BadKyu_DefenseState(AI_StateMachine.EnemyState key, AI_BadKyu_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    private readonly AI_BadKyu_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;
    
    /// <summary>
    /// Call Dash function.
    /// </summary>
    public override void EnterState()
    {
        // _aiArcherStateMachine._rb.AddForce(-_aiArcherStateMachine.centerPoint.transform.forward * _aiArcherStateMachine.dashSpeed, ForceMode.Impulse);
        _aiStateMachine.StartCoroutine(Dash());
    }
    
    /// <summary>
    /// Reset Velocity
    /// </summary>
    public override void ExitState()
    {
        _aiStateMachine._rb.velocity = Vector3.zero;
    }

    public override void UpdateState()
    {
        
    }

    public override AI_Archer_StateMachine.EnemyState GetNextState()
    {
        return AI_Archer_StateMachine.EnemyState.Chase;
    }

    /// <summary>
    /// Move at a high speed to a certain direction for a short time.
    /// Return to Chase State after finishing.
    /// </summary>
    IEnumerator Dash()
    {
        var timer = _aiStateMachine.dashDuration;
        
        _aiStateMachine._rb.isKinematic = false;
        
        while (timer > 0)
        {
            
            _aiStateMachine._rb.velocity = new Vector3(
                -_aiStateMachine.centerPoint.transform.forward.x * _aiStateMachine.dashSpeed,
                _aiStateMachine._rb.velocity.y,
                -_aiStateMachine.centerPoint.transform.forward.z * _aiStateMachine.dashSpeed);
            
            timer -= Time.deltaTime;
            yield return null;
        }
        
        _aiStateMachine._rb.isKinematic = true;
        _aiStateMachine.TryToTransition(AI_Archer_StateMachine.EnemyState.Chase);
        
    }
}
