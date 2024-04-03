using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    protected override void Awake()
    {
        
        base.Awake();
        
        States.Add(EnemyState.Idle, new AI_IdleState(EnemyState.Idle, this));
        States.Add(EnemyState.Patrol, new AI_Summoner_PatrolState(EnemyState.Patrol, this));
        States.Add(EnemyState.Chase, new AI_Summoner_ChaseState(EnemyState.Chase, this));
        States.Add(EnemyState.Attack, new AI_Summoner_AttackState(EnemyState.Attack, this));
        States.Add(EnemyState.Death, new AI_DeathState(EnemyState.Death, this));
        States.Add(EnemyState.Freeze, new AI_FreezeState(EnemyState.Freeze, this));
        
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

    /// <summary>
    /// Instantiate X number of summonGO in an arc in front of the entity.
    /// </summary>
    private void Summon()
    {
        for (var i = 0; i < numbersOfSummons; i++)
        {
            var enemyPool = SC_Pooling.instance.poolList.Find(s => s.poolName == "Ennemis");
            var kyuEnemyList = enemyPool.subPoolsList.ToList();
            kyuEnemyList = kyuEnemyList.Where(e => e.subPoolTransform.gameObject.name == "GO_BadKyu").ToList();
            
            var summon = SC_Pooling.instance.GetItemFromPool("Ennemis", kyuEnemyList[Random.Range(0, kyuEnemyList.Count)].subPoolTransform.gameObject.name);
            
            summon.GetComponent<NavMeshAgent>().enabled = false;
            var angle = Mathf.PI * (i+1) / (numbersOfSummons+1);
            print(angle);
                
            var x = Mathf.Sin(angle) * detectionAreaRadius;
            var z = Mathf.Cos(angle) * detectionAreaRadius;
            var pos = new Vector3(x, 0.5f, z);
            
            var centerDirection = Quaternion.LookRotation(-centerPoint.right, centerPoint.up);
            
            pos = centerDirection * pos;
            
            summon.transform.position = transform.position + pos;
            summon.GetComponent<NavMeshAgent>().enabled = true;
            
        }

        StartCoroutine(SummonCooldown());
        print("Summoning");
    }

    /// <summary>
    /// Spawn X projectileGO in front of the entity
    /// </summary>
    private IEnumerator SpawnProjectile()
    {
        for (var i = 0; i < projectileNumbers; i++)
        {
            _stats.CreateProjectile(projectileGO);
            
            yield return new WaitForSeconds(delayBetweenProjectiles);
        }
    }
    
    /// <summary>
    /// Launch a cooldown of X seconds before regive the possibility to use Summon Function
    /// </summary>
    private IEnumerator SummonCooldown()
    {
        canSummon = false;
        
        yield return new WaitForSeconds(summonCooldown);

        canSummon = true;
    }
    
    /// <summary>
    /// After X hits, knock-back the attacker.
    /// </summary>
    public override void OnDamageTaken()
    {
        base.OnDamageTaken();
        
        hitCounter += 1;

        if (hitCounter != 5) return;

        hitCounter = 0;
        
        if (!player.TryGetComponent(out SC_PlayerController pController)) return;
        
        pController.TakeKnockback();

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
    
    /// <summary>
    /// When it called, transition to the Chase State.
    /// </summary>
    public void Signal()
    {
        TryToTransition(EnemyState.Chase);
    }

    
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if(player != null) Gizmos.DrawRay(centerPoint.position, (player.transform.position - centerPoint.position));
    }
}
