    using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Enum;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_AIStats : SC_EntityBase, IDamageable
{

    #region Variables
    
    [Title("IDs")] 
    [Tooltip("ID of the enemy")] public string typeID;
    [Tooltip("Is an Elite enemy ?")] public bool isElite = false;

    [Space(10)]
    
    #region Status - Buff, Debuff, and Shields

    #region Shield
    
    [TabGroup("Shield")]
    [Space(2.5f)]
    [Tooltip("Has a shield to break before taking damage")] public bool hasShield;
    
    [Space(10f)]
    [TabGroup("Shield")]
    [ShowIf("hasShield")]
    [Tooltip("His weakness can regenerate ?")] public bool canRegenShield = false;
    
    [TabGroup("Shield")]
    [ShowIf("canRegenShield")]
    [Tooltip("After how many seconds ?")] public float delayBeforeRegen = 4f;

    [TabGroup("Shield")]
    [ShowIf("hasShield")]
    [Tooltip("Is the shield broken?")] public bool isBreaked;
    
    [TabGroup("Shield")]
    [ShowIf("hasShield")]
    [Tooltip("has Thorns?")] public bool hasThorns;
    
    #endregion
    
    #endregion

    [PropertySpace(SpaceBefore = 10)]
    [Tooltip("How many % of the enemy ATK the attack does"),TabGroup("ATK")] public float[] moveValues;
    [Tooltip("Index of the current attack MV"),TabGroup("ATK"), ReadOnly] public int moveValueIndex = 0;
    
    public static Action<SC_AIStats> onDeath;
    public SO_Event onShieldBreaked;
    
    private SC_AIRenderer _renderer;
    private NavMeshAgent _agent;
    private AI_StateMachine _stateMachine;
    private SC_DebuffsBuffsComponent _debuffsBuffsComponent;

    [ShowInInspector] public bool isDead = false;
    #endregion


    #region Init

    private void Awake()
    {
        if(!TryGetComponent(out _renderer)) return;
        if(!TryGetComponent(out _debuffsBuffsComponent)) return;
        if(!TryGetComponent(out _agent)) return;
        if(!TryGetComponent(out _stateMachine)) return;
    }

    /// <summary>
    /// Initialize HP
    /// Initialize Shield
    /// </summary>
    private void Start()
    {
        currentStats = baseStats;
        
        UpdateStats();
        InitShield();
        
        _renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
        
        if(_agent != null) _agent.speed = currentStats.currentSpeed;
        
    }

    private void UpdateStats()
    {
        
        currentStats.currentHealth = currentStats.currentMaxHealth;
        
    }
    
    #endregion

    public void ResetStats()
    {

        currentStats.currentHealth = currentStats.currentMaxHealth;
        _renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
        _renderer.RemoveDebugDamageChildren();

        isDead = false;
    }

    #region Shield Part
    
    /// <summary>
    /// Initialize the Weaknesses and create a shield that should be broken before apply damage to the Entity.
    /// </summary>
    private void InitShield()
    {
        if(!hasShield) return;
        
        _renderer.UpdateShieldBar(isBreaked);
    }

    /// <summary>
    /// Internal cooldown that regenerate Weaknesses Shield after a certain delay.
    /// </summary>
    private IEnumerator RegenerateShield()
    {
        
        yield return new WaitForSeconds(delayBeforeRegen);

        isBreaked = false;
        InitShield();

    }
    

    #endregion

    private void OnEnable()
    {
        ResetStats();
        if(_agent != null)
            _agent.enabled = true;
        _renderer.showStatsUI?.Invoke();
    }

    private void OnDisable()
    {
        if(_agent != null)
            _agent.enabled = false;
        _renderer.hideStatsUI?.Invoke();
    }

    #region Damage Part

    /// <summary>
    /// Calculating real taken damage by the entity.
    /// Apply this amount to the entity.
    /// </summary>
    /// <param name="rawDamage">Amount of a non-crit damage</param>
    /// <param name="isCrit"></param>
    /// <param name="attacker"></param>
    /// <param name="trueDamage"></param>
    public void TakeDamage(float rawDamage, bool isCrit, GameObject attacker,bool trueDamage = false)
    {
        
        
        if (hasShield & !isBreaked)
        {
            
            isBreaked = true;
            onShieldBreaked.RaiseEvent();
            _renderer.UpdateShieldBar(isBreaked);
            print("BREAKED");

            if (canRegenShield)
                StartCoroutine(RegenerateShield());
        }
        else
        {
        
            var damageTakenMultiplier = (1 + (currentStats.damageTaken/100));
            
            // Check if the damage is a Critical one and reduce damage by the current DEF of the entity.
            var finalDamage = MathF.Round((rawDamage * currentStats.defMultiplier) * damageTakenMultiplier);

            // Apply damage to the entity. Check if doesn't go below 0.
            currentStats.currentHealth = currentStats.currentHealth - finalDamage <= 0 ? 0 : currentStats.currentHealth - finalDamage;

            // Debug Part
            print(typeID + " : -" + finalDamage + " HP");
            // print(typeID + " : " + currentHealth + "/" + currentMaxHealth);

            _renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
            _renderer.DebugDamage(finalDamage, isCrit);

            if(isDead) return;
            
            if (currentStats.currentHealth <= 0)
            {
                
                if (_stateMachine == null)
                {
                    Death();
                    return;
                }
                if(isDead != true) {
                    _stateMachine.TransitionToState(AI_StateMachine.EnemyState.Death);
                    isDead = true;
                }
            }
                
        }
        
        #region Thorns
        
        if(!_debuffsBuffsComponent.CheckHasBuff(Enum_Buff.Thorns)) return;
        
        const float thornsMV = 0.1f;
        var rawDMG = thornsMV * currentStats.currentATK;
            
        attacker.GetComponent<IDamageable>().TakeDamage(MathF.Round(rawDMG, MidpointRounding.AwayFromZero), false, gameObject);
        
        #endregion
        
        if(_stateMachine == null) return;
        
        _stateMachine.OnDamageTaken();
        
    }
    
    public void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType)
    {
        
        var damageTakenMultiplier = (1 + (currentStats.dotDamageTaken/100));
        
        // Check if the damage is a Critical one and reduce damage by the current DEF of the entity.
        var finalDamage = MathF.Round((rawDamage * currentStats.defMultiplier) * damageTakenMultiplier);

        // Apply damage to the entity. Check if doesn't go below 0.
        currentStats.currentHealth = currentStats.currentHealth - finalDamage <= 0 ? 0 : currentStats.currentHealth - finalDamage;

        // Debug Part
        print(isCrit ? typeID + " : -" + finalDamage + " CRIIIIT HP" : typeID + " : -" + finalDamage + " HP");
        // print(typeID + " : " + currentHealth + "/" + currentMaxHealth);

        _renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
        _renderer.DebugDamage(finalDamage, isCrit, dotType);

        if(isDead) return;
        
        if (currentStats.currentHealth <= 0)
        {
                
            if (_stateMachine == null)
            {
                Death();
                return;
            }
            if(isDead != true) {
                _stateMachine.TransitionToState(AI_StateMachine.EnemyState.Death);
                isDead = true;
            }
        }
    }

    public void Death()
    {
        onDeath?.Invoke(this);
        _debuffsBuffsComponent.ResetAllBuffsAndDebuffs();
        
        SC_RewardManager.instance.ResourceDropSelection(isElite ? "Elite" : "Base");
        
        if(SC_Pooling.instance != null) {
            SC_Pooling.instance.ReturnItemToPool("Ennemis", gameObject);
            ResetStats();
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject, 1);
        }
        
    }
    
    public void CreateHitBox(SO_HitBox hb)
    {
        var hbTransform = transform.GetChild(0).GetChild(2);
        transform.GetChild(0).GetChild(2).localPosition = hb.center;
        
        // print(hb.name);
        var hits = hb.type switch
        {
            HitBoxType.Box => Physics.OverlapBox(hbTransform.position, hb.halfExtents,
                GetCurrentForwardVector(hb.orientation), hb.layer),
            HitBoxType.Sphere => Physics.OverlapSphere(hb.pos, hb.radiusSphere, hb.layer),
            HitBoxType.Capsule => Physics.OverlapCapsule(hb.point0, hb.point1, hb.radiusCapsule, hb.layer),
            _ => throw new ArgumentOutOfRangeException()
        };
        

        foreach (var e in hits)
        {
            var aiCurrentAtk = currentStats.currentATK;
            var aiCurrentMV = moveValues[moveValueIndex];

            var rawDamage = Mathf.Round(aiCurrentMV * aiCurrentAtk);
            
            e.GetComponent<IDamageable>().TakeDamage(rawDamage, false, gameObject);
        }

    }

    #endregion

    #region Debug

    [Button]
    public void AttackPlayer()
    {
        
        GameObject.FindWithTag("Player").GetComponent<IDamageable>().TakeDamage(5, false, gameObject);
        
    }

    #endregion
}
