using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_AIStats : SC_Subject, IDamageable
{

    #region Variables
    
    [Title("IDs")] 
    [Tooltip("Plus tard flemme")] public string typeID;
    
    #region Stats
    
    [TabGroup("Settings", "Stats")]
    [Title("Parameters")]
    // Max HP and HP

    #region HP
    
    [PropertySpace(SpaceAfter = 10)]
    [TabGroup("Settings/Stats/Subtabs", "HP", SdfIconType.HeartFill, TextColor = "green"),
     ProgressBar(0, "currentMaxHealth", r: 0, g: 1, b: 0, Height = 20), ReadOnly]
    public float currentHealth;

    [TabGroup("Settings/Stats/Subtabs", "HP")]
    [FoldoutGroup("Settings/Stats/Subtabs/HP/Max HP")]
    [Tooltip("Current base MaxHP of the enemy"), ShowInInspector]
    public float maxHealthBase = 15;
    
    [TabGroup("Settings/Stats/Subtabs", "HP")]
    [FoldoutGroup("Settings/Stats/Subtabs/HP/Max HP")]
    [Tooltip("Current MaxHP multiplier of the enemy")] public float maxHealthModifier = 0;
    public float currentMaxHealth => maxHealthBase * (1 + maxHealthModifier);

    #endregion
    [Space(5)]

    #region DEF

    // DEF
    [TabGroup("Settings/Stats/Subtabs", "DEF", SdfIconType.ShieldFill, TextColor = "blue")]
    [Tooltip("Current base DEF of the enemy")] public float defBase = 1;
    
    [TabGroup("Settings/Stats/Subtabs", "DEF")]
    [Tooltip("Current DEF modifier of the enemy")] public float defModifier = 0;
    
    [TabGroup("Settings/Stats/Subtabs", "DEF")]
    [Tooltip("Current DEF of the enemy"), ShowInInspector, ReadOnly] public float currentDEF => defBase * (1 + defModifier);
    
    [TabGroup("Settings/Stats/Subtabs", "DEF")]
    [Tooltip("DEF Stat used to reduce damage taken"), ShowInInspector, ReadOnly]
    public float defMultiplier => (100 / (100 + currentDEF));

    #endregion
    [Space(5)]

    #region ATK

    // ATK
    [TabGroup("Settings/Stats/Subtabs", "ATK", TextColor = "red")]
    [Tooltip("Current base ATK of the enemy")] public float atkBase = 1;
    [TabGroup("Settings/Stats/Subtabs", "ATK")]
    [Tooltip("Current ATK multiplier of the enemy")] public float atkModifier = 0;
    public float currentATK => atkBase * (1 + atkModifier);
    
    [PropertySpace(SpaceBefore = 10)]
    [Tooltip("How many % of the enemy ATK the attack does"),TabGroup("Settings/Stats/Subtabs", "ATK")] public float[] moveValues;
    [Tooltip("Index of the current attack MV"),TabGroup("Settings/Stats/Subtabs", "ATK"), ReadOnly] public int moveValueIndex = 0;

    #endregion
    [Space(5)]

    #region SPD

    // Speed
    [TabGroup("Settings/Stats/Subtabs", "SPD", SdfIconType.Speedometer, TextColor = "purple")]
    [Tooltip("Current base Speed of the enemy")] public float speedBase = 5;
    [TabGroup("Settings/Stats/Subtabs", "SPD")]
    [Tooltip("Current Speed multiplier of the enemy")] public float speedModifier = 0;
    public float currentSPD => speedBase * (1 + speedModifier);
    

    #endregion
    [Space(5)]
    
    #region DMG

    // DMG
    [TabGroup("Settings/Stats/Subtabs", "DMG",TextColor = "darkred")]
    public float damageTaken = 0;
    
    [TabGroup("Settings/Stats/Subtabs", "DMG")]
    public float dotDamageTaken = 0;

    #endregion
    
    #endregion

    [Space(10)]
    
    #region Status - Buff, Debuff, and Shields

    #region Shield
    
    [TabGroup("Settings", "Shield")]
    
    [Space(2.5f)]
    [Tooltip("Has a shield to break before taking damage")] public bool hasShield;
    
    [Space(10f)]
    [FoldoutGroup("Settings/Shield/Shield Settings")]
    [ShowIfGroup("Settings/Shield/Shield Settings/hasShield")]
    [Tooltip("His weakness can regenerate ?")] public bool canRegenShield = false;
    
    [FoldoutGroup("Settings/Shield/Shield Settings/Regeneration Settings")]
    [ShowIfGroup("Settings/Shield/Shield Settings/Regeneration Settings/canRegenShield")]
    [Tooltip("After how many seconds ?")] public float delayBeforeRegen = 4f;
    
    [PropertySpace(SpaceAfter = 10)]

    [TabGroup("Settings", "Status")]
    [ShowIf("hasShield")]
    [Tooltip("Is the shield broken?")] public bool isBreaked;
    
    [Space(10f)]
    [FoldoutGroup("Settings/Shield/Shield Settings")]
    [ShowIfGroup("Settings/Shield/Shield Settings/hasShield")]
    [Tooltip("has Thorns?")] public bool hasThorns;
    
    #endregion

    
    #endregion

    
    public static Action onDeath;
    public SO_Event onShieldBreaked;
    
    private SC_AIRenderer _renderer;
    private NavMeshAgent _agent;
    private AI_StateMachine _stateMachine;
    private SC_DebuffsBuffsComponent _debuffsBuffsComponent;
    
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
        
        UpdateStats();
        InitShield();
        
        _renderer.UpdateHealthBar(currentHealth, currentMaxHealth);
        
        if(_agent != null) _agent.speed = currentSPD;
        
    }

    private void UpdateStats()
    {
        
        currentHealth = currentMaxHealth;
        
    }
    
    #endregion

    private void ResetStats()
    {

        currentHealth = currentMaxHealth;
        _renderer.UpdateHealthBar(currentHealth, currentMaxHealth);
        _renderer.RemoveDebugDamageChildren();

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
        
            var damageTakenMultiplier = (1 + (damageTaken/100));
            
            // Check if the damage is a Critical one and reduce damage by the current DEF of the entity.
            var finalDamage = MathF.Round((rawDamage * defMultiplier) * damageTakenMultiplier);

            // Apply damage to the entity. Check if doesn't go below 0.
            currentHealth = currentHealth - finalDamage <= 0 ? 0 : currentHealth - finalDamage;

            // Debug Part
            print(typeID + " : -" + finalDamage + " HP");
            // print(typeID + " : " + currentHealth + "/" + currentMaxHealth);

            _renderer.UpdateHealthBar(currentHealth, currentMaxHealth);
            _renderer.DebugDamage(finalDamage, isCrit);

            if (currentHealth <= 0)
            {
                if (_stateMachine == null)
                {
                    Death();
                    return;
                }
                
                _stateMachine.TransitionToState(AI_StateMachine.EnemyState.Death);
            }
                
        }
        
        #region Thorns
        
        if(!_debuffsBuffsComponent.CheckHasBuff(Enum_Buff.Thorns)) return;
        
        const float thornsMV = 0.1f;
        var rawDMG = thornsMV * currentATK;
            
        attacker.GetComponent<IDamageable>().TakeDamage(MathF.Round(rawDMG, MidpointRounding.AwayFromZero), false, gameObject);
        
        #endregion
        
        if(_stateMachine == null) return;
        
        _stateMachine.OnDamageTaken();
        
    }
    
    public void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType)
    {
        
        // Check if the damage is a Critical one and reduce damage by the current DEF of the entity.
        var finalDamage = MathF.Round(rawDamage * defMultiplier);

        // Apply damage to the entity. Check if doesn't go below 0.
        currentHealth = currentHealth - finalDamage <= 0 ? 0 : currentHealth - finalDamage;

        // Debug Part
        print(isCrit ? typeID + " : -" + finalDamage + " CRIIIIT HP" : typeID + " : -" + finalDamage + " HP");
        // print(typeID + " : " + currentHealth + "/" + currentMaxHealth);

        _renderer.UpdateHealthBar(currentHealth, currentMaxHealth);
        _renderer.DebugDamage(finalDamage, isCrit, dotType);

        if (!(currentHealth <= 0)) return;
        
        if (_stateMachine == null)
        {
            Death();
            return;
        }
                
        _stateMachine.TransitionToState(AI_StateMachine.EnemyState.Death);
    }

    public void Death()
    {
        NotifyObservers("enemyDeath");
        onDeath?.Invoke();
        
        if(SC_Pooling.instance != null) {
            SC_Pooling.instance.ReturnItemToPool("Ennemis", gameObject);
            gameObject.SetActive(false);
            ResetStats();
        }
        else
        {
            Destroy(gameObject, 1);    
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
