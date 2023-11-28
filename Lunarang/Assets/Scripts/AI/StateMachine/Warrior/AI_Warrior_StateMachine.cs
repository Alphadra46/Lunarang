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
    
    public enum EnemyState // TODO : Combiner les Enums
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
    [Tooltip("Current base ATK Speed of the enemy")] public float atkSpdBase = 1;
    [TabGroup("States", "Attack")]
    [Tooltip("Current base ATK Cooldown of the enemy")] public float atkCDBase = 1;
    [TabGroup("States", "Attack")]
    [Tooltip("Attack Duration")] public float atkDuration = 1f;
    
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public GameObject hurtBox;
    [TabGroup("States", "Attack")]
    public LayerMask layersAttackable;
    
    
    #endregion

    #region Stun
    
    [TabGroup("States", "Stun")]
    public float stunDuration = 0.2f;

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
        
        States.Add(EnemyState.Idle, new AI_Warrior_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Warrior_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Warrior_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Warrior_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Stun, new AI_Warrior_StunState(EnemyState.Stun, this));
        
        CurrentState = States[EnemyState.Idle];
    }

    // TODO : Refaire les commentaires.

    /// <summary>
    /// Activate the hurtbox to deal damage to the forward entity.
    /// </summary>
    public void Attack()
    {
        
        hurtBox.SetActive(true);
        
    }

    /// <summary>
    /// Switch to Stun State when Player's Hurtbox touche him.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        
        if(!other.CompareTag("HurtBox_Player")) return;
        
        TransitionToState(EnemyState.Stun);
        
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
