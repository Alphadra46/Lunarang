using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_ChaseState : BaseState<AI_Archer_StateMachine.EnemyState>
{
    
    public AI_Archer_ChaseState(AI_Archer_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key, manager)
    {
        _aiArcherStateMachine = manager;
    }
    
    #region Variables

    private readonly AI_Archer_StateMachine _aiArcherStateMachine;
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
        
        _transform = _aiArcherStateMachine.centerPoint;
        player = GameObject.FindWithTag("Player");
        _agent = _aiArcherStateMachine.agent;
        _agent.updateRotation = false;
        _agent.speed = _aiArcherStateMachine.chaseSpeed;
        
    }

    /// <summary>
    /// If switch to Attack State, start the cooldown for attack.
    /// If switch to Defense State, start the cooldown for dash.
    /// </summary>
    public override void ExitState()
    {
        switch (_aiArcherStateMachine.NextState)
        {
            case AI_StateMachine.EnemyState.Attack:
                _aiArcherStateMachine.StartCoroutine(AttackCooldown());
                break;
            case AI_StateMachine.EnemyState.Defense:
                _aiArcherStateMachine.StartCoroutine(DefenseCooldown());
                break;
            default:
                throw new ArgumentOutOfRangeException();
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

        var distance = Vector3.Distance(_aiArcherStateMachine.transform.position, player.transform.position);
        
        if (distance <= _aiArcherStateMachine.defenseAreaRadius)
        {
            _agent.isStopped = true;
            if (canDefense)
            {
                _aiArcherStateMachine.TransitionToState(AI_StateMachine.EnemyState.Defense);
            }
        }
        else if (distance <= _aiArcherStateMachine.attackRange)
        {
            _agent.isStopped = true;
            if (canAttack && _aiArcherStateMachine.hasLineOfSightTo(player.transform, _transform))
            {
                _aiArcherStateMachine.TransitionToState(AI_StateMachine.EnemyState.Attack);
            }
                
        }
        else if (distance <= _aiArcherStateMachine.detectionAreaRadius)
        {
            _agent.isStopped = false;
            _agent.SetDestination(player.transform.position);
        }
        
        _aiArcherStateMachine.centerPoint.LookAt(new Vector3(player.transform.position.x, _aiArcherStateMachine.centerPoint.position.y, player.transform.position.z));
        
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
        yield return new WaitForSeconds(_aiArcherStateMachine.atkCDBase);
        canAttack = true;

    }
    
    /// <summary>
    /// Internal Cooldown before next dash.
    /// </summary>
    public IEnumerator DefenseCooldown()
    {
        canDefense = false;
        yield return new WaitForSeconds(_aiArcherStateMachine.defenseCDBase);
        canDefense = true;

    }
    
}
