using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_ChaseState : BaseState<AI_StateMachine.EnemyState>
{
    
    public AI_Archer_ChaseState(AI_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key, manager)
    {
        _aiStateMachine = manager;
    }
    
    #region Variables

    private readonly AI_Archer_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;
    private Transform _transform;
    
    private Collider[] objectsInArea;

    private GameObject player;
    
    private bool canAttack = true;
    private bool canDefense = true;

    #endregion

    /// <summary>
    /// Initialize references.
    /// </summary>
    public override void EnterState()
    {
        
        _transform = _aiStateMachine.centerPoint;
        player = GameObject.FindWithTag("Player");
        _agent = _aiStateMachine.agent;
        _agent.updateRotation = false;
        _agent.speed = _aiStateMachine.chaseSpeed;
        
    }

    /// <summary>
    /// If switch to Attack State, start the cooldown for attack.
    /// If switch to Defense State, start the cooldown for dash.
    /// </summary>
    public override void ExitState()
    {
        
        switch (_aiStateMachine.NextState)
        {
            case AI_StateMachine.EnemyState.Attack:
                _aiStateMachine.StartCoroutine(AttackCooldown());
                break;
            case AI_StateMachine.EnemyState.Defense:
                _aiStateMachine.StartCoroutine(DefenseCooldown());
                break;
            default:
                _aiStateMachine.StopCoroutine(AttackCooldown());
                _aiStateMachine.StopCoroutine(DefenseCooldown());
                break;
        }
        
    }
    
    /// <summary>
    /// Check the distance between Archer and the Player.
    /// If Player is in the defense area, Archer dash backward.
    /// If Player is in the chase area, Archer can attack and has Player in line of sight, switch to Attack State.
    /// If Player is no longer in the chase Area, follow the player.
    /// Rotate toward the player.
    /// </summary>
    public override void UpdateState()
    {
        
        var distance = Vector3.Distance(_aiStateMachine.transform.position, player.transform.position);
        var playerPos = player.transform.position;
        
        if (distance <= _aiStateMachine.defenseAreaRadius)
        {
            _agent.isStopped = true;
            if (canDefense)
            {
                _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Defense);
            }
        }
        else if (distance <= _aiStateMachine.attackRange)
        {
            _agent.isStopped = true;
            if (canAttack && _aiStateMachine.hasLineOfSightTo(player.transform, _transform))
            {
                _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Attack);
            }
                
        }
        else if (distance <= _aiStateMachine.detectionAreaRadius)
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.transform.position);
        }
        
        if (_aiStateMachine.hasSeenPlayer)
        {
            _agent.SetDestination(playerPos);
        }
        
        _aiStateMachine.centerPoint.LookAt(new Vector3(playerPos.x, _aiStateMachine.centerPoint.position.y, playerPos.z));
        
    }
    
    
    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Chase;
    }
    
    /// <summary>
    /// Internal Cooldown before next attack.
    /// </summary>
    public IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(_aiStateMachine.atkCDBase);
        canAttack = true;

    }
    
    /// <summary>
    /// Internal Cooldown before next dash.
    /// </summary>
    public IEnumerator DefenseCooldown()
    {
        canDefense = false;
        yield return new WaitForSeconds(_aiStateMachine.defenseCDBase);
        canDefense = true;

    }
    
}
