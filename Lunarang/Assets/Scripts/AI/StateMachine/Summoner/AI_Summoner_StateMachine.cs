using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AI_Summoner_StateMachine : AI_StateMachine
{
    
    #region Attack

    #region Summon

    [TabGroup("States/Attack/Subtab", "Summon")]
    [Range(1f, 100f)] public float summonCooldown = 9f;
    [TabGroup("States/Attack/Subtab", "Summon")]
    [Range(1, 100)] public int numbersOfSummons = 2;
    [TabGroup("States/Attack/Subtab", "Summon")]
    [Range(1, 100)] public int maxNumbersOfSummons = 8;
    [FormerlySerializedAs("summonPrefab")] [TabGroup("States/Attack/Subtab", "Summon")]
    public GameObject summonGO;
    #endregion

    #region Projectiles

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States/Attack/Subtab", "Attack")]
    public int projectileNumbers = 5;
    [TabGroup("States/Attack/Subtab", "Attack")]
    public float projectileSpeed = 5;
    [TabGroup("States/Attack/Subtab", "Attack")]
    public float projectileDamage = 2f;
    [TabGroup("States/Attack/Subtab", "Attack")]
    public float delayBetweenProjectiles = 0.1f;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States/Attack/Subtab", "Attack")]
    public GameObject projectileGO;
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("States/Attack/Subtab", "Attack")]
    public Vector3 ProjectileSpawnOffset = new Vector3(0, 0.5f, 0);

    #endregion
    
    [TabGroup("States/Attack/Subtab", "Attack")]
    public bool canSummon = true;
    
    #endregion
    
    [TabGroup("States", "Defense")]
    public int hitCounter = 0;
    [TabGroup("States", "Defense")]
    public float knockbackStrength = 1f;

    public GameObject player;
    
    /// <summary>
    /// Initialize all references.
    /// Add all states to the state list.
    /// </summary>
    private void Awake()
    {
        if (!TryGetComponent(out agent)) return;
        if (!TryGetComponent(out _stats)) return;
        if (!TryGetComponent(out _rb)) return;
        
        States.Add(EnemyState.Idle, new AI_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Summoner_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Summoner_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Summoner_AttackState(EnemyState.Attack, this));

        CurrentState = States[EnemyState.Idle];
        
        player = GameObject.FindWithTag("Player");
    }

    // TODO : Refaire les commentaires.

    /// <summary>
    /// Activate the hurtbox to deal damage to the forward entity.
    /// </summary>
    public void Attack()
    {
        var playerPos = player.transform.position;
        if(canSummon){
            Summon();
        }
        else
        {
            StartCoroutine(SpawnProjectile());
            print("Attack");
        }
        
    }

    public void Summon()
    {
        for (var i = 0; i < numbersOfSummons; i++)
        {
            var summon = Instantiate(summonGO);
            var angle = Mathf.PI * (i+1) / (numbersOfSummons+1);
            print(angle);
                
            var x = Mathf.Sin(angle) * detectionAreaRadius;
            var z = Mathf.Cos(angle) * detectionAreaRadius;
            var pos = new Vector3(x, transform.position.y, z);
            
            var centerDirection = Quaternion.LookRotation(-centerPoint.right, centerPoint.up);
            
            pos = centerDirection * pos;
            
            summon.transform.position = transform.position + pos;
            
        }

        StartCoroutine(SummonCooldown());
        print("Summoning");
    }
    
    public IEnumerator SpawnProjectile()
    {
        for (var i = 0; i < projectileNumbers; i++)
        {
            var projectile = Instantiate(projectileGO).GetComponent<SC_Projectile>();

            projectile.transform.position = centerPoint.position + ProjectileSpawnOffset;
            projectile.transform.forward = centerPoint.forward;
        
            projectile.speed = projectileSpeed;
            projectile.damage = (int)Mathf.Round(((projectileDamage/100) * _stats.currentATK));
            projectile._rb.AddForce(centerPoint.transform.forward * projectile.speed, ForceMode.VelocityChange);
            
            yield return new WaitForSeconds(delayBetweenProjectiles);
        }
    }

    private IEnumerator SummonCooldown()
    {
        canSummon = false;
        
        yield return new WaitForSeconds(summonCooldown);

        canSummon = true;
    }
    
    public override void OnDamageTaken()
    {
        base.OnDamageTaken();
        
        hitCounter += 1;

        if (hitCounter != 5) return;

        hitCounter = 0;
        
        if (!player.TryGetComponent(out SC_PlayerController pController)) return;
        
        pController.TakeKnockback();

    }

    private IEnumerator ResetKB(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.15f);

        rb.velocity = Vector3.zero;

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
    
    public void Signal()
    {
        TransitionToState(EnemyState.Chase);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(player != null) Gizmos.DrawRay(centerPoint.position, (player.transform.position - centerPoint.position));
    }
}
