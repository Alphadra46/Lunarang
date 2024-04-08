using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Bully_AttackState : BaseState<AI_Bully_StateMachine.EnemyState>
{
    public AI_Bully_AttackState(AI_Bully_StateMachine.EnemyState key, AI_Bully_StateMachine manager) : base(key,
        manager)
    {
        _aiStateMachine = manager;
    }

    private readonly AI_Bully_StateMachine _aiStateMachine;

    /// <summary>
    /// Call Attack function and launch the coroutine End Attack.
    /// </summary>
    public override void EnterState()
    {
        _aiStateMachine.StartCoroutine(Attack());
    }

    public override void ExitState()
    {
        // _aiStateMachine.StopCoroutine(Attack(_aiStateMachine.atkDuration));
    }

    public override void UpdateState()
    {
    }

    /// <summary>
    /// After a certain delay, deactivate the hurtbox and switch to Chase State.
    /// </summary>
    /// <param name="delay">Delay in seconds before switching state.</param>
    private IEnumerator Attack()
    {
        if (!_aiStateMachine.canCharge)
        {
            // Basic Attack
            _aiStateMachine.agent.isStopped = true;
            _aiStateMachine.agent.velocity = Vector3.zero;
            _aiStateMachine.agent.SetDestination(_aiStateMachine.transform.position);

            _aiStateMachine._renderer.SendTriggerToAnimator("Attack_01");
            _aiStateMachine.canRotate = false;

            yield return new WaitForSeconds(_aiStateMachine.atkDuration);

            _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
            _aiStateMachine.agent.enabled = true;
            _aiStateMachine.canRotate = true;
            
            // _aiStateMachine.canCharge = true;
            _aiStateMachine.StartCoroutine(_aiStateMachine.AttackCD());
            
            _aiStateMachine._stats.InitShield();
        }
        else
        {
            // Charge Attack
            _aiStateMachine.agent.isStopped = true;
            _aiStateMachine.agent.velocity = Vector3.zero;

            _aiStateMachine._renderer.SendTriggerToAnimator("Attack_02");
            _aiStateMachine.canRotate = false;
            
            _aiStateMachine._rb.isKinematic = false;

            var timer = 3f;
            
            // while (timer > 0)
            // {
            //
            //     _aiStateMachine._rb.velocity = new Vector3(
            //         _aiStateMachine.centerPoint.transform.forward.x * _aiStateMachine.chargeSpeed,
            //         _aiStateMachine._rb.velocity.y,
            //         _aiStateMachine.centerPoint.transform.forward.z * _aiStateMachine.chargeSpeed);
            //     
            //     timer -= Time.deltaTime;
            //     yield return null;
            // }

            yield return new WaitUntil(_aiStateMachine.ObstacleHitted);
        
            _aiStateMachine._rb.isKinematic = true;

            _aiStateMachine._renderer.SendTriggerToAnimator("EndCharge");

            _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Chase);
            
            _aiStateMachine.agent.enabled = true;
            _aiStateMachine.canRotate = true;
            
            _aiStateMachine.canCharge = false;
            _aiStateMachine.StartCoroutine(_aiStateMachine.AttackCD());
            
        }
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Attack;
    }
}