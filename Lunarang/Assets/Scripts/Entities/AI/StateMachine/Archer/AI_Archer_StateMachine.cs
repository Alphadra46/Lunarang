using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI_Archer_StateMachine : AI_StateMachine
{

    #region Variables

    [TabGroup("States", "Attack")]
    public float atkBlockRotationDelay = 1f;
    
    #region Defense

    [TabGroup("States", "Defense")]
    [Range(1f, 100f)]
    public float defenseAreaRadius = 1;
    [TabGroup("States", "Defense")]
    [Range(1f, 100f)]
    public float defenseCDBase = 2f;
    [TabGroup("States", "Defense")]
    [Range(1f, 100f)]
    public float dashSpeed = 4f;
    [TabGroup("States", "Defense")]
    [Range(0f, 1f)]
    public float dashDuration = 1f;
    

    #endregion
    

    #endregion
    
    /// <summary>
    /// Initialize all references.
    /// Add all states to the state list.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        
        States.Add(EnemyState.Idle, new AI_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Archer_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Archer_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Archer_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Defense, new AI_Archer_DefenseState(EnemyState.Defense, this));
        States.Add(EnemyState.Death, new AI_DeathState(EnemyState.Death, this));
        States.Add(EnemyState.Freeze, new AI_FreezeState(EnemyState.Freeze, this));
        
        CurrentState = States[EnemyState.Idle];
    }

    /// <summary>
    /// Check if the target is in line of sight.
    /// </summary>
    /// <param name="target">Transform targeted</param>
    /// <param name="start"></param>
    /// <returns>
    /// Boolean of has in line of sight.
    /// </returns>
    public bool hasLineOfSightTo(Transform target, Transform start)
    {
        return Physics.SphereCast(start.position + projectileSpawnOffset, 0.1f,
            ((target.position + projectileSpawnOffset) -
             (start.position + projectileSpawnOffset)).normalized, out var Hit,
            detectionAreaRadius, layersAttackable) && Hit.collider.CompareTag("Player");
    }

    #region Gizmos

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        
        // Defense Area
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, defenseAreaRadius);
        
    }

    #endregion
    
    
}
