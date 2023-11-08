using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI_Warrior_StateMachine : StateManager<AI_Warrior_StateMachine.EnemyState>
{
    
    public enum EnemyState
    {
        
        Idle,
        Patrol,
        Chase,
        Attack,
        Stun
        
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
    [Range(1f, 100f)] public float attackRange = 1f;
    [TabGroup("States", "Attack")]
    [Tooltip("How many % of the enemy ATK the attack does")] public float moveValue = 0.5f;
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
    
    
    #endregion

    #region Stun

    

    #endregion

    [HideInInspector] public NavMeshAgent agent;

    public SC_AIStats _stats;
    public Rigidbody _rb;

    #endregion
    
    private void Awake()
    {
        if (!TryGetComponent(out agent)) return;
        if (!TryGetComponent(out _stats)) return;
        if (!TryGetComponent(out _rb)) return;
        
        States.Add(EnemyState.Idle, new AI_Warrior_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Warrior_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Warrior_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Warrior_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Stun, new AI_Warrior_StunState(EnemyState.Stun, this));
        
        CurrentState = States[EnemyState.Idle];
    }

    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        var randomPoint = center + Random.insideUnitSphere * range;

        if (NavMesh.SamplePosition(randomPoint, out var hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        
        result = Vector3.zero;
        return false;
    }

    public float RandomPatrolRange()
    {
        patrolRadius = Random.Range(patrolRadiusMin, patrolRadiusMax);

        return patrolRadius;
    }

    public void Attack()
    {
    }


    #region Gizmos

    private void OnDrawGizmos()
    {

        // Chase Area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseAreaRadius);
        
        // Attack Area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Forward Ray
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector3(centerPoint.position.x, 1, centerPoint.position.z), centerPoint.forward);
    }

    #endregion
    
    
}
