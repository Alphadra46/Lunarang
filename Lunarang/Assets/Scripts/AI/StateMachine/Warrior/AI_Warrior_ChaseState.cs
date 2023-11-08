using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI_Warrior_ChaseState : BaseState<AI_Warrior_StateMachine.EnemyState>
{
    
    public AI_Warrior_ChaseState(AI_Warrior_StateMachine.EnemyState key, AI_Warrior_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    #region Variables

    private readonly AI_Warrior_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;
    private Transform _transform;
    
    private Collider[] objectsInArea;

    private GameObject player;
    
    private bool canAttack = true;
    private bool canBeStun = true;

    #endregion

    public override void EnterState()
    {
        
        _transform = _aiStateMachine.centerPoint;
        player = GameObject.FindWithTag("Player");
        _agent = _aiStateMachine.agent;
        _agent.updateRotation = false;
        
    }

    public override void ExitState()
    {
        switch (_aiStateMachine.NextState)
        {
            case AI_Warrior_StateMachine.EnemyState.Attack:
                _aiStateMachine.StartCoroutine(AttackCooldown());
                break;
        }
        
    }

    public override void UpdateState()
    {

        var distance = Vector3.Distance(_aiStateMachine.transform.position, player.transform.position);
        
        if (distance <= _aiStateMachine.attackRange)
        {
            
            _agent.isStopped = true;
            if (canAttack && hasLineOfSightTo(player.transform))
            {
                _aiStateMachine.TransitionToState(AI_Warrior_StateMachine.EnemyState.Attack);
            }
                
        }
        else if (distance <= _aiStateMachine.chaseAreaRadius)
        {
            
            _agent.isStopped = false;
            _agent.SetDestination(player.transform.position);
            
        }
        else
        {
            _aiStateMachine.TransitionToState(AI_Warrior_StateMachine.EnemyState.Patrol);
        }
        
        _aiStateMachine.centerPoint.LookAt(new Vector3(player.transform.position.x, _aiStateMachine.centerPoint.position.y, player.transform.position.z));
        
    }

    private bool hasLineOfSightTo(Transform target)
    {
        return Physics.SphereCast(_transform.position + _aiStateMachine.ProjectileSpawnOffset, 0.1f,
            ((target.position + _aiStateMachine.ProjectileSpawnOffset) -
             (_transform.position + _aiStateMachine.ProjectileSpawnOffset)).normalized, out var Hit,
            _aiStateMachine.chaseAreaRadius, LayerMask.NameToLayer("Player")) && Hit.collider.CompareTag("Player");
    }

    public override AI_Warrior_StateMachine.EnemyState GetNextState()
    {
        return AI_Warrior_StateMachine.EnemyState.Chase;
    }
    
    public IEnumerator AttackCooldown()
    {
        
        canAttack = false;
        yield return new WaitForSeconds(_aiStateMachine.atkCDBase);
        canAttack = true;

    }
}
