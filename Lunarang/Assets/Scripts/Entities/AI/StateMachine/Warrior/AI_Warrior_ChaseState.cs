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
    // private bool canBeStun = true;

    #endregion
    

    // ReSharper disable Unity.PerformanceAnalysis
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
    /// If switch to Attack State, start internal cooldown for attacks.
    /// </summary>
    public override void ExitState()
    {
        if (_aiStateMachine.NextState == AI_Warrior_StateMachine.EnemyState.Attack) _aiStateMachine.StartCoroutine(AttackCooldown());
    }
    
    
    /// <summary>
    /// Check the distance between Archer and the Player.
    /// If Player is in the attack area, Warrior can attack and has Player in line of sight, switch to Attack State.
    /// If Player is in the chase Area, follow the player.
    /// If Player is no longer in the chase Area, switch to Patrol State.
    /// Rotate toward the player.
    /// </summary>
    public override void UpdateState()
    {
        //
        // if (_aiStateMachine._stats.isDead)
        // {
        //     _aiStateMachine.StopCoroutine(AttackCooldown());
        //     _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Death);
        // }

        var distance = Vector3.Distance(_aiStateMachine.transform.position, player.transform.position);
        var playerPos = player.transform.position;
        
        if (distance <= _aiStateMachine.attackRange)
        {
            
            _agent.isStopped = true;
            if (canAttack && _aiStateMachine.hasLineOfSightTo(player.transform, _transform, _aiStateMachine.detectionAreaRadius, _aiStateMachine.layersAttackable))
            {
                _aiStateMachine.TryToTransition(AI_StateMachine.EnemyState.Attack);
            }
                
        }
        else if (distance <= _aiStateMachine.detectionAreaRadius)
        {
            
            _agent.isStopped = false;
            _aiStateMachine.hasSeenPlayer = true;

        }

        if (_aiStateMachine.hasSeenPlayer)
        {
            _agent.SetDestination(playerPos);
        }
        // else
        // {
        //     _aiStateMachine.TransitionToState(AI_StateMachine.EnemyState.Patrol);
        // }
        
        _aiStateMachine.centerPoint.LookAt(new Vector3(playerPos.x, _aiStateMachine.centerPoint.position.y, playerPos.z));
        
    }
    


    public override AI_Warrior_StateMachine.EnemyState GetNextState()
    {
        return AI_Warrior_StateMachine.EnemyState.Chase;
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
    
}
