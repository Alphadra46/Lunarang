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

public class SC_AIStats : MonoBehaviour, IDamageable
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
     ProgressBar(0, "maxHealthEffective", r: 0, g: 1, b: 0, Height = 20), ReadOnly]
    public float currentHealth;
    
    [TabGroup("Settings/Stats/Subtabs", "HP")]
    [FoldoutGroup("Settings/Stats/Subtabs/HP/Max HP")]
    [Tooltip("Current base MaxHP of the enemy")] public float maxHealthBase = 15;
    
    [TabGroup("Settings/Stats/Subtabs", "HP")]
    [FoldoutGroup("Settings/Stats/Subtabs/HP/Max HP")]
    [Tooltip("Current MaxHP multiplier of the enemy")] public float maxHealthModifier = 0;
    public float maxHealthEffective => maxHealthBase * (1 + maxHealthModifier);

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
    
    #endregion

    [Space(10)]
    
    #region Status - Buff, Debuff, and Shields

    #region Shield
    
    [TabGroup("Settings", "Shield")]
    
    [Space(2.5f)]
    [Tooltip("Has a shield to break before taking damage")] public bool hasShield;
    
    [FoldoutGroup("Settings/Shield/Shield Settings")]
    [ShowIfGroup("Settings/Shield/Shield Settings/hasShield")]
    [Tooltip("How many weaknesses to broke the shield")] public int weaknessLength = 3;
    [FoldoutGroup("Settings/Shield/Shield Settings")]
    [ShowIfGroup("Settings/Shield/Shield Settings/hasShield")]
    [Tooltip("Is the weaknesses randomize ?")] public bool randomWeakness = true;
    [Space(10f)]
    [FoldoutGroup("Settings/Shield/Shield Settings")]
    [ShowIfGroup("Settings/Shield/Shield Settings/hasShield")]
    [Tooltip("His weakness can regenerate ?")] public bool canRegenShield = false;
    
    [FoldoutGroup("Settings/Shield/Shield Settings/Regeneration Settings")]
    [ShowIfGroup("Settings/Shield/Shield Settings/Regeneration Settings/canRegenShield")]
    [Tooltip("After how many seconds ?")] public float delayBeforeRegen = 4f;
    
    [PropertySpace(SpaceAfter = 10)]
    
    [FoldoutGroup("Settings/Shield/Shield Settings"), ShowIfGroup("Settings/Shield/Shield Settings/hasShield")]
    [Tooltip("Current weakness of the shield")] public List<WeaponType> currentWeakness;
    [FoldoutGroup("Settings/Shield/Shield Settings"), ShowIfGroup("Settings/Shield/Shield Settings/hasShield")]
    [Tooltip("Previous weakness of the shield")] public List<WeaponType> previousWeakness;

    [TabGroup("Settings", "Status")]
    [ShowIf("hasShield")]
    [Tooltip("Is the shield broken?")] public bool isBreaked;
    
    #endregion
    
    [TabGroup("Settings", "Status")]
    [Title("Debuffs")]
    [Tooltip("List of all current debuffs on this enemy"), SerializeField] private List<Enum_Debuff> currentDebuffs;

    
    #endregion

    private SC_AIRenderer _renderer;
    private NavMeshAgent _agent;
    
    #endregion


    #region Init

    private void Awake()
    {
        if(!TryGetComponent(out _renderer)) return;
        if(!TryGetComponent(out _agent)) return;
    }

    /// <summary>
    /// Initialize HP
    /// Initialize Shield
    /// </summary>
    private void Start()
    {
        
        currentHealth = maxHealthEffective;
        InitWeaknessShield();
        
        if(_agent != null) _agent.speed = currentSPD;
        
    }
    
    #endregion

    #region Shield Part
    
    /// <summary>
    /// Initialize the Weaknesses and create a shield that should be broken before apply damage to the Entity.
    /// </summary>
    private void InitWeaknessShield()
    {
        if(!hasShield) return;

        if (randomWeakness)
        {
            for (var i = 0; i < weaknessLength; i++)
            {
                currentWeakness.Add((WeaponType)Random.Range(1, 4));
            }
        }
        
        if(previousWeakness.Count != 0)
            previousWeakness.Clear();
        previousWeakness = currentWeakness.ToList();
        
        // Debug Part

        _renderer.UpdateWeaknessBar(currentWeakness);

    }

    /// <summary>
    /// Internal cooldown that regenerate Weaknesses Shield after a certain delay.
    /// </summary>
    private IEnumerator RegenerateShield()
    {
        
        yield return new WaitForSeconds(delayBeforeRegen);

        isBreaked = false;
        InitWeaknessShield();

    }
    

    #endregion

    #region Status

    private void ApplyDebuffToSelf(Enum_Debuff newDebuff, float tick=1, float duration=5)
    {
        
        currentDebuffs.Add(newDebuff);

        switch (newDebuff)
        {
            case Enum_Debuff.Poison:
                StartCoroutine(PoisonDoT((maxHealthEffective * 0.1f), tick, duration));
                break;
        }
        
    }

    private IEnumerator PoisonDoT(float incomingDamage, float tick, float duration)
    {
        var finalDamage = incomingDamage - currentDEF;
        
        
        while (duration != 0)
        {
            currentHealth = currentHealth - finalDamage <= 0 ? 0 : currentHealth - finalDamage;
            duration -= tick;
            
            // // Debug Part
            // print("Dummy : -" + finalDamage + " HP");
            // print((duration+1) + " seconds reamining");
            
            _renderer.UpdateHealthBar(currentHealth, maxHealthEffective);
            _renderer.DebugDamage(finalDamage);
            
            yield return new WaitForSeconds(tick);
        }

    }

    #endregion

    #region Damage Part

    /// <summary>
    /// Apply a damage to the weaknesses.
    /// </summary>
    /// <param name="incomingType"></param>
    private void TakeWeaknessDamage(WeaponType incomingType)
    {

        print("Incoming Type : " + incomingType);

        if (currentWeakness[0] == incomingType)
        {
            currentWeakness.Remove(currentWeakness[0]);
            _renderer.UpdateWeaknessBar(currentWeakness);
        }
        else
        {
            currentWeakness.Clear();
            currentWeakness = previousWeakness.ToList();
            _renderer.UpdateWeaknessBar(currentWeakness);
        }

        if (currentWeakness.Count != 0) return;
        
        isBreaked = true;
        print("BREAKED");

        if (canRegenShield)
            StartCoroutine(RegenerateShield());

    }

    /// <summary>
    /// Calculating real taken damage by the entity.
    /// Apply this amount to the entity.
    /// </summary>
    /// <param name="rawDamage">Amount of a non-crit damage</param>
    public void TakeDamage(float rawDamage)
    {
        // Check if the damage is a Critical one and reduce damage by the current DEF of the entity.
        var finalDamage = rawDamage * defMultiplier;

        // Apply damage to the entity. Check if doesn't go below 0.
        currentHealth = currentHealth - finalDamage <= 0 ? 0 : currentHealth - finalDamage;

        // Debug Part
        print("Dummy : -" + finalDamage + " HP");
        print("Dummy : " + currentHealth + "/" + maxHealthEffective);

        _renderer.UpdateHealthBar(currentHealth, maxHealthEffective);
        _renderer.DebugDamage(finalDamage);
        
        if(currentHealth == 0) Destroy(gameObject);

    }
    

    #endregion
    
    #region Collisions Part

    /// <summary>
    /// Detect collisions and if collide with Player's HurtBox, take damage or weakness break.
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("HurtBox_Player")) return;
            
        var player = col.transform.parent.gameObject;
        var pCombo = player.GetComponent<SC_ComboController>();
        var pStats = player.GetComponent<SC_PlayerStats>();

        var isCritical = Random.value < pStats.critRate ? true : false;

        var currentMV = pCombo.currentWeapon.MovesValues[pCombo.comboCounter-1];
        
        var rawDamage = currentMV * pStats.currentATK;
        var rawCrit = rawDamage * (1 + pStats.critDMG);
        
        if (hasShield && !isBreaked)
        {
            TakeWeaknessDamage(pCombo.currentWeapon.type);
        }
        else
        {
            TakeDamage(rawDamage);
        }

    }

    private void OnTriggerExit(Collider col)
    {
        
    }

    #endregion
    
}
