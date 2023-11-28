using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI_Archer_StateMachine : StateManager<AI_Archer_StateMachine.EnemyState>
{
    
    public enum EnemyState
    {
        
        Idle,
        Patrol,
        Chase,
        Attack,
        Defense
        
    }

    #region Variables

    public Transform centerPoint;

    #region Idle

    [TabGroup("States", "Idle")]
    [Range(1f, 25f)]
    public float idleDelay = 1;

    #endregion

    #region Patrol

    [TabGroup("States", "Patrol")]
    [Range(0.1f, 100f)]
    public float patrolRadiusMin = 0.1f;
    
    [TabGroup("States", "Patrol")]
    [Range(0.1f, 100f)]
    public float patrolRadiusMax = 1;

    [TabGroup("States", "Patrol"), ReadOnly]
    public float patrolRadius;
    
    [TabGroup("States", "Patrol")]
    [Range(1f, 100f), PropertySpace(SpaceBefore = 10)]
    public float patrolDelay = 1;

    #endregion

    #region Chase
    
    [TabGroup("States", "Chase")]
    [Range(1f, 100f)]
    public float chaseAreaRadius = 1;

    #endregion

    #region Attack
    
    [TabGroup("States", "Attack")]
    [Tooltip("Current base ATK Speed of the enemy")] public float atkSpdBase = 1;
    [TabGroup("States", "Attack")]
    [Tooltip("Current base ATK Cooldown of the enemy")] public float atkCDBase = 1;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public GameObject projectileGO;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public Vector3 ProjectileSpawnOffset = new Vector3(0, 0.5f, 0);
    [TabGroup("States", "Attack")]
    public LayerMask layersAttackable;
    
    
    #endregion

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

    [HideInInspector] public NavMeshAgent agent;

    public SC_AIStats _stats;
    public Rigidbody _rb;

    #endregion
    
    /// <summary>
    /// Initialize all references.
    /// Add all states to the state list.
    /// </summary>
    private void Awake()
    {
        if (!TryGetComponent(out agent)) return;
        if (!TryGetComponent(out _stats)) return;
        if (!TryGetComponent(out _rb)) return;
        
        States.Add(EnemyState.Idle, new AI_Archer_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Archer_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Archer_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Archer_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Defense, new AI_Archer_DefenseState(EnemyState.Defense, this));
        
        CurrentState = States[EnemyState.Idle];
    }
    
    /// <summary>
    /// Summon a projectile from the spawn offset.
    /// Set all the settings of the projectile.
    /// </summary>
    public void SpawnProjectile()
    {
        
        var projectile = Instantiate(projectileGO).GetComponent<SC_Projectile>();

        projectile.transform.position = centerPoint.position + ProjectileSpawnOffset;
        projectile.transform.forward = centerPoint.forward;
        
        projectile.speed = atkSpdBase;
        projectile.damage = (int)Mathf.Round((_stats.moveValues[_stats.moveValueIndex] * _stats.currentATK));
        projectile._rb.AddForce(centerPoint.transform.forward * projectile.speed, ForceMode.VelocityChange);

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
        return Physics.SphereCast(start.position + ProjectileSpawnOffset, 0.1f,
            ((target.position + ProjectileSpawnOffset) -
             (start.position + ProjectileSpawnOffset)).normalized, out var Hit,
            chaseAreaRadius, layersAttackable) && Hit.collider.CompareTag("Player");
    }

    #region Gizmos

    private void OnDrawGizmos()
    {
        // Patrol Zone
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        
        // Chase Area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseAreaRadius);
        
        // Defense Area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, defenseAreaRadius);
        
        // Forward Ray
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector3(centerPoint.position.x, 1, centerPoint.position.z), centerPoint.forward);
    }

    #endregion
    
    
}
