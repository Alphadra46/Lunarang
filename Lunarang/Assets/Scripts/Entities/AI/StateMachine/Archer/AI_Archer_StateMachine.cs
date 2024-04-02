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

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public GameObject projectileGO;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public Vector3 ProjectileSpawnOffset = new Vector3(0, 0.5f, 0);
    [TabGroup("States", "Attack")] 
    public List<string> tags = new List<string>();
    
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
    /// Summon a projectile from the spawn offset.
    /// Set all the settings of the projectile.
    /// </summary>
    public void SpawnProjectile()
    {
        
        var projectile = Instantiate(projectileGO).GetComponent<SC_Projectile>();

        projectile.sender = gameObject;
        
        projectile.transform.position = centerPoint.position + ProjectileSpawnOffset;
        projectile.transform.forward = centerPoint.forward;

        projectile.direction = centerPoint.forward;
        projectile.hitNumber = 1;
        
        projectile.speed = atkSpdBase;
        projectile.damage = (int)Mathf.Round((_stats.moveValues[_stats.moveValueIndex] * _stats.currentStats.currentATK));

        projectile.tags = tags;

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
