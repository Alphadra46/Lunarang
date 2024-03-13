using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI_Warrior_StateMachine : AI_StateMachine
{

    #region Variables

    [TabGroup("States", "Attack")]
    public GameObject hurtBox;
    
    #region Stun
    
    [TabGroup("States", "Stun")]
    public float stunDuration = 0.2f;

    #endregion

    #endregion
    
    /// <summary>
    /// Initialize all references.
    /// Add all states to the state list.
    /// </summary>
    protected override void Awake()
    {

        base.Awake();
        
        States.Add(EnemyState.Idle, new AI_Warrior_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Warrior_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Warrior_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Warrior_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Stun, new AI_Warrior_StunState(EnemyState.Stun, this));
        States.Add(EnemyState.Death, new AI_DeathState(EnemyState.Death, this));
        States.Add(EnemyState.Freeze, new AI_FreezeState(EnemyState.Freeze, this));
        
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
        
        TryToTransition(EnemyState.Stun);
        
    }
    
    
}
