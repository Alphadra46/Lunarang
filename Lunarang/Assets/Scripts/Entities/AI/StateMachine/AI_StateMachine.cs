using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AI_StateMachine : StateManager<AI_StateMachine.EnemyState>
{
    
    public enum EnemyState
    {
        
        Idle,
        Patrol,
        Chase,
        Attack,
        Stun,
        Defense,
        Death,
        Freeze
        
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
    
    [TabGroup("States", "Patrol")]
    [Range(1f, 100f)]
    public float patrolSpeed = 1;

    #endregion

    #region Chase
    
    [TabGroup("States", "Chase")]
    [Range(1f, 100f)]
    public float detectionAreaRadius = 1;
    [TabGroup("States", "Chase")]
    [Range(1f, 100f)]
    public float chaseSpeed = 1;
    [TabGroup("States", "Chase")]
    public bool hasSeenPlayer = false;
    [TabGroup("States", "Chase")]
    public bool canRotate = true;
    
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
    
    [PropertySpace(SpaceBefore = 20f)]
    [TabGroup("States", "Attack")]
    public GameObject projectileGO;
    [TabGroup("States", "Attack")]
    public Vector3 projectileSpawnOffset = new Vector3(0, 0.5f, 0);
    
    [PropertySpace(SpaceBefore = 10)]
    
    [TabGroup("States", "Attack")]
    public LayerMask layersAttackable;
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States", "Attack")]
    public bool canAttack = true;
    
    
    #endregion

    [HideInInspector] public NavMeshAgent agent;

    public SC_AIStats _stats;
    public SC_AIRenderer _renderer;
    public Rigidbody _rb;
    public CapsuleCollider _collider;
    
    #endregion

    protected virtual void Awake()
    {
        if (!TryGetComponent(out agent)) return;
        if (!TryGetComponent(out _stats)) return;
        if (!TryGetComponent(out _renderer)) return;
        if (!TryGetComponent(out _rb)) return;
        if (!TryGetComponent(out _collider)) return;
    }

    public virtual void OnDamageTaken()
    {}
    
    /// <summary>
    /// Activate the hurtbox to deal damage to the forward entity.
    /// </summary>
    public void CanAttack(bool value)
    {
        
        canAttack = value;
        _renderer.SendBoolToAnimator("canAttack", value);
        
    }
    
    public void TryToTransition(EnemyState newState, bool isFreezeBreak = false)
    {
        
        if (_stats.isDead) return;
        
        if (CurrentState == States[EnemyState.Freeze] && !isFreezeBreak) return;
        
        TransitionToState(newState);
        
    }
    
    #region Gizmos

    protected virtual void OnDrawGizmos()
    {

        // Chase Area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionAreaRadius);
        
        // Attack Area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Forward Ray
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector3(centerPoint.position.x, 1, centerPoint.position.z), centerPoint.forward);
        
        // Patrol Zone
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        
    }

    #endregion
}
