    using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Enum;
using Sirenix.OdinInspector;
    using Sirenix.Utilities;
    using TMPro;
using UnityEngine;
using UnityEngine.AI;
    using UnityEngine.SceneManagement;
    using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

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
    [Tooltip("Can have a shield")] public bool canHaveShield;
    
    
    [Space(10f)]
    [TabGroup("Shield")]
    [ShowIf("canHaveShield")]
    [Tooltip("His shield can regenerate ?")] public bool canRegenShield = false;
    
    [TabGroup("Shield")]
    [ShowIf("canRegenShield")]
    [Tooltip("After how many seconds ?")] public float delayBeforeRegen = 4f;
    
    [Space(10f)]
    [TabGroup("Shield")]
    [ShowIf("canHaveShield")]
    [Tooltip("Shield HP based on HP% ?")] public float shieldValue = 0;

    
    
    [TabGroup("Shield")]
    [ShowIf("canHaveShield")]
    [Tooltip("has Thorns?")] public bool hasThorns;
    
    
    [Space(10f)]
    [TabGroup("Shield")]
    [ShowIf("canHaveShield")]
    [Tooltip("Has a shield to break before taking damage"), ReadOnly] public bool hasShield;
    [Space(5f)]
    [TabGroup("Shield")]
    [ShowIf("canHaveShield")]
    [Tooltip("Is the shield broken?"), ReadOnly] public bool isBreaked;
    
    
    #endregion
    
    #endregion

    [PropertySpace(SpaceBefore = 10)]
    [Tooltip("How many % of the enemy ATK the attack does"),TabGroup("ATK")] public float[] moveValues;
    [Tooltip("Index of the current attack MV"),TabGroup("ATK"), ReadOnly] public int moveValueIndex = 0;
    
    public static Action<SC_AIStats> onDeath;
    public SO_Event onShieldBreaked;
    
    [FormerlySerializedAs("_renderer")] public SC_AIRenderer renderer;
    private NavMeshAgent _agent;
    private AI_StateMachine _stateMachine;
    private SC_DebuffsBuffsComponent _debuffsBuffsComponent;

    [ShowInInspector] public bool isDead = false;
    #endregion


    #region Init

    private void Awake()
    {
        if(!TryGetComponent(out renderer)) return;
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
        
        renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
        
        if(_agent != null) _agent.speed = currentStats.currentSpeed;
        
    }
    
    
    private void OnEnable()
    {
        ResetStats();
        if(_agent != null)
            _agent.enabled = false;
        renderer.showStatsUI?.Invoke();
        renderer.ResetColor();
        _debuffsBuffsComponent.ClearAllVFX();
    }

    private void OnDisable()
    {
        if(_agent != null)
            _agent.enabled = false;
        renderer.hideStatsUI?.Invoke();
        renderer.ResetColor();
        _debuffsBuffsComponent.ClearAllVFX();
    }
    
    public void ResetStats()
    {

        currentStats.currentHealth = currentStats.currentMaxHealth;
        renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
        renderer.RemoveDebugDamageChildren();

        isDead = false;
    }

    private void UpdateStats()
    {
        
        currentStats.currentHealth = currentStats.currentMaxHealth;
        
    }
    
    #endregion

    #region Shield Part
    
    public void CreateShield(float hpPercentage)
    {
        if(canHaveShield == false) return;
        
        if(currentStats is { shieldCurrentHP: > 0, shieldMaxHP: > 0 }) BreakShield();

        hasShield = true;

        var newShieldValue = (hpPercentage/100) * currentStats.maxHealth;
        
        currentStats.shieldMaxHP = newShieldValue * (1 + (currentStats.shieldStrength/100));
        currentStats.shieldCurrentHP = currentStats.shieldMaxHP;
        
        renderer.UpdateShieldBar(currentStats.shieldCurrentHP, currentStats.shieldMaxHP);
        
    }
    
    public void BreakShield()
    {
        
        hasShield = false;
        
        currentStats.shieldMaxHP = 0;
        currentStats.shieldCurrentHP = 0;
        
        renderer.UpdateShieldBar(currentStats.shieldCurrentHP, currentStats.shieldMaxHP);

    }

    /// <summary>
    /// Internal cooldown that regenerate Weaknesses Shield after a certain delay.
    /// </summary>
    private IEnumerator RegenerateShield()
    {
        
        yield return new WaitForSeconds(delayBeforeRegen);

        isBreaked = false;
        CreateShield(shieldValue);

    }
    

    #endregion

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
        StartCoroutine(renderer.DamageTaken());
        
        var damageTakenMultiplier = (1 + (currentStats.damageTaken/100));
        var damageReductionMultiplier = (1 - (currentStats.damageReduction/100));
        
        if (hasShield && currentStats.shieldCurrentHP > 0)
        {
            var shieldDamage = Mathf.Round(rawDamage * damageTakenMultiplier * damageReductionMultiplier);
            
            currentStats.shieldCurrentHP = currentStats.shieldCurrentHP - shieldDamage < 0 ? 0 : currentStats.shieldCurrentHP - shieldDamage;

            if (currentStats.shieldCurrentHP == 0)
            {
                
                SC_CameraShake.instance.StopAllCoroutines();
                StartCoroutine(SC_CameraShake.instance.ShakeCamera(5f, 1f, 0.3f));
                
                isBreaked = true;
                onShieldBreaked.RaiseEvent();
                
                if (canRegenShield)
                    StartCoroutine(RegenerateShield());
                
            }
            
            renderer.UpdateShieldBar(currentStats.shieldCurrentHP, currentStats.shieldMaxHP);
            
        }
        else
        {
            
            // Check if the damage is a Critical one and reduce damage by the current DEF of the entity.
            var finalDamage = MathF.Round((rawDamage * currentStats.defMultiplier) * damageTakenMultiplier);

            // Apply damage to the entity. Check if doesn't go below 0.
            currentStats.currentHealth = currentStats.currentHealth - finalDamage <= 0 ? 0 : currentStats.currentHealth - finalDamage;

            // Debug Part
            print(typeID + " : -" + finalDamage + " HP");
            // print(typeID + " : " + currentHealth + "/" + currentMaxHealth);

            renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
            renderer.DebugDamage(finalDamage, isCrit);

            if(isDead) return;
            
            if (currentStats.currentHealth <= 0)
            {
                
                if (_stateMachine == null)
                {
                    Death();
                    return;
                }
                if(isDead != true) {
                    isDead = true;
                    _stateMachine.TransitionToState(AI_StateMachine.EnemyState.Death);
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

        renderer.UpdateHealthBar(currentStats.currentHealth, currentStats.currentMaxHealth);
        renderer.DebugDamage(finalDamage, isCrit, dotType);

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
        renderer.hideStatsUI?.Invoke();
        
        _debuffsBuffsComponent.ResetAllBuffsAndDebuffs();

        if(SC_Pooling.instance != null) {
            SC_RewardManager.instance.ResourceDropSelection(isElite ? "Elite" : "Base", out int amount);
            if (SceneManager.GetActiveScene().buildIndex!=1)
                DropRessources(SC_PlayerController.instance.transform.position, amount);
            SC_Pooling.instance.ReturnItemToPool("Ennemis", gameObject);
            ResetStats();
            _debuffsBuffsComponent.ClearAllVFX();
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

    public void CreateProjectile(GameObject projectileGO = null)
    {
        if (projectileGO == null) projectileGO = _stateMachine.projectileGO;
        
        var projectile = Instantiate(projectileGO).GetComponent<SC_Projectile>();

        var centerPoint = transform.GetChild(0);

        projectile.sender = gameObject;
        
        projectile.transform.position = centerPoint.position + _stateMachine.projectileSpawnOffset;
        projectile.transform.forward = centerPoint.forward;

        projectile.direction = centerPoint.forward;
        
        projectile.damage = (int)Mathf.Round((moveValues[moveValueIndex] * currentStats.currentATK));
        
    }

    public void DropRessources(Vector3 goalPosition,int amount)
    {
        if (amount==0)
            return;
        
        VisualEffect dropRessourcesVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_RessourcesLoot").GetComponent<VisualEffect>();
        dropRessourcesVFX.gameObject.SetActive(true);
        dropRessourcesVFX.SetInt("Particles Number",amount);
        dropRessourcesVFX.transform.position = transform.position;
        dropRessourcesVFX.SetVector3("Spawn Position", transform.position);
        SC_PlayerController.instance.StartCoroutine(UpdateTargetPosition(dropRessourcesVFX));
        dropRessourcesVFX.Play();

        SC_Pooling.instance.ReturnItemToPool("VFX", dropRessourcesVFX.gameObject);
    }

    private IEnumerator UpdateTargetPosition(VisualEffect vfx)
    {
        var totalTimer = vfx.GetFloat("Particles Life") + 3f;
        var timer = totalTimer;

        // print("S T A R T");
        while (timer > 0)
        {
            vfx.SetVector3("Target Position", SC_PlayerController.instance.transform.position);
            yield return null;
            timer -= Time.deltaTime;
            // print("ON GOING");
        }
        yield return null;
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
