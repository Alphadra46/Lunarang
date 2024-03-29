using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI_BadKyu_StateMachine : AI_StateMachine
{

    #region Variables
    
    [PropertySpace(SpaceBefore = 10)]
    [Range(1, 100f)]
    [TabGroup("States", "Attack")]
    public int maxProjectiles = 1;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public int currentProjectiles = 0;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public GameObject projectileArms;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public GameObject projectileBody;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public Vector3 ProjectileSpawnOffset = new Vector3(0, 0.5f, 0);
    

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
        States.Add(EnemyState.Patrol, new AI_BadKyu_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_BadKyu_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_BadKyu_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Defense, new AI_BadKyu_DefenseState(EnemyState.Defense, this));
        States.Add(EnemyState.Death, new AI_DeathState(EnemyState.Death, this));
        States.Add(EnemyState.Freeze, new AI_FreezeState(EnemyState.Freeze, this));
        
        CurrentState = States[EnemyState.Idle];
    }

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        currentProjectiles = 0;
    }

    /// <summary>
    /// Summon a projectile from the spawn offset.
    /// Set all the settings of the projectile.
    /// </summary>
    public void SpawnProjectile()
    {
        
        var projectile = currentProjectiles == 2 ? 
            Instantiate(projectileBody).GetComponent<SC_Projectile>() : Instantiate(projectileArms).GetComponent<SC_Projectile>();

        projectile.sender = gameObject;
        
        projectile.transform.position = centerPoint.position + ProjectileSpawnOffset;
        projectile.transform.forward = centerPoint.forward;

        projectile.direction = centerPoint.forward;
        projectile.hitNumber = 1;
        
        projectile.speed = atkSpdBase;
        projectile.damage = (int)Mathf.Round(((_stats.moveValues[currentProjectiles == 2 ? 1 : 0] / 100) * _stats.currentStats.currentATK));

        currentProjectiles++;
        
        if(currentProjectiles >= maxProjectiles)
        {
            print("MEURT");
            TransitionToState(EnemyState.Death);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        
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
            ((new Vector3(target.position.x, target.localScale.y/1, target.position.z) + ProjectileSpawnOffset) -
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
