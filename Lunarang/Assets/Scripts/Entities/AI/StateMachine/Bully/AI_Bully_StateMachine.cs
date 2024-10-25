using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI_Bully_StateMachine : AI_StateMachine
{
    
    [TabGroup("States", "Attack")]
    public GameObject hurtBox;
    [TabGroup("States", "Attack")]
    public bool canShield = true;

    public bool obstacleHitted = false;
    public float shieldCD = 10f;
    
    
    /// <summary>
    /// Initialize all references.
    /// Add all states to the state list.
    /// </summary>
    protected override void Awake()
    {
        
        base.Awake();
        
        States.Add(EnemyState.Idle, new AI_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Bully_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Bully_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Bully_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Death, new AI_DeathState(EnemyState.Death, this));
        States.Add(EnemyState.Freeze, new AI_FreezeState(EnemyState.Freeze, this));
        
        CurrentState = States[EnemyState.Idle];
        
    }

    public bool ObstacleHitted()
    {
        return obstacleHitted;
    }

    public IEnumerator AttackCD()
    {
        
        CanAttack(false);
        yield return new WaitForSeconds(atkCDBase);
        CanAttack(true);

    }
    
    public IEnumerator ShieldCD()
    {
        
        CanShield(false);
        yield return new WaitForSeconds(shieldCD);
        CanShield(true);

    }
    
    public void CanShield(bool value)
    {
        
        canShield = value;
        
    }
    
    /// <summary>
    /// Switch to Stun State when Player's Hurtbox touche him.
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("HurtBox_Player"))
        {
            obstacleHitted = true;
            return;
        }

        if (!other.TryGetComponent(out SC_ComboController playerCombo)) return;
        
        if (playerCombo.comboCounter == 3)
        {
            _rb.AddForce(other.transform.forward, ForceMode.Impulse);
        }

    }
    
    
}
