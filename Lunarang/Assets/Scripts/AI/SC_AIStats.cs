using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_AIStats : MonoBehaviour
{

    #region Variables

    #region Stats
    
    [TabGroup("Settings", "Stats")]
    
    // Max HP and HP

    #region HP
    
    [PropertySpace(SpaceAfter = 10)]
    [TabGroup("Settings/Stats/Subtabs", "HP", SdfIconType.HeartFill, TextColor = "green"),
     ProgressBar(0, "maxHPEffective", r: 0, g: 1, b: 0, Height = 20), ReadOnly]
    public float currentHP;
    
    [TabGroup("Settings/Stats/Subtabs", "HP")]
    [FoldoutGroup("Settings/Stats/Subtabs/HP/Max HP")]
    [Tooltip("Current base MaxHP of the enemy")] public float maxHPBase = 15;
    
    [TabGroup("Settings/Stats/Subtabs", "HP")]
    [FoldoutGroup("Settings/Stats/Subtabs/HP/Max HP")]
    [Tooltip("Current MaxHP multiplier of the enemy")] public float maxHPModifier = 0;
    private float maxHPEffective => maxHPBase * (1 + maxHPModifier);

    #endregion
    [Space(5)]
    
    // DEF
    [TabGroup("Settings/Stats/Subtabs", "DEF", SdfIconType.ShieldFill, TextColor = "blue")]
    [Tooltip("Current base DEF of the enemy")] public float defBase = 1;
    [TabGroup("Settings/Stats/Subtabs", "DEF")]
    [Tooltip("Current DEF multiplier of the enemy")] public float defModifier = 0;
    private float defEffective => defBase * (1 + defModifier);
    
    [Space(5)]
    
    // ATK
    [TabGroup("Settings/Stats/Subtabs", "ATK", TextColor = "red")]
    [Tooltip("Current base ATK of the enemy")] public float atkBase = 1;
    [TabGroup("Settings/Stats/Subtabs", "ATK")]
    [Tooltip("Current ATK multiplier of the enemy")] public float atkModifier = 0;
    private float atkEffective => atkBase * (1 + atkModifier);
    
    [Space(5)]
    
    // Speed
    [TabGroup("Settings/Stats/Subtabs", "SPD", SdfIconType.Speedometer, TextColor = "purple")]
    [Tooltip("Current base Speed of the enemy")] public float speedBase = 5;
    [TabGroup("Settings/Stats/Subtabs", "SPD")]
    [Tooltip("Current Speed multiplier of the enemy")] public float speedModifier = 0;
    private float speedEffective => speedBase * (1 + speedModifier);
    
    
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
    
    #endregion


    #region Init

    private void Awake()
    {
        _renderer = GetComponent<SC_AIRenderer>();
    }

    /// <summary>
    /// Initialize HP
    /// Initialize Shield
    /// </summary>
    private void Start()
    {

        currentHP = maxHPEffective;
        InitWeaknessShield();
        
    }
    
    #endregion

    #region Shield Part

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
                StartCoroutine(PoisonDoT((maxHPEffective * 0.1f), tick, duration));
                break;
        }
        
    }

    private IEnumerator PoisonDoT(float incomingDamage, float tick, float duration)
    {
        var finalDamage = incomingDamage - defEffective;
        
        
        while (duration != 0)
        {
            currentHP -= finalDamage;
            duration -= tick;
            
            // Debug Part
            print("Dummy : -" + finalDamage + " HP");
            print((duration+1) + " seconds reamining");
            
            _renderer.UpdateHealthBar(currentHP, maxHPEffective);
            _renderer.DebugDamage(finalDamage);
            
            yield return new WaitForSeconds(tick);
        }

    }

    #endregion

    #region Damage Part

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

    private float TakeDamage(float incomingDamage)
    {
        var finalDamage = incomingDamage - defEffective; // Here for the second part of the formula.

        currentHP -= finalDamage;

        // Debug Part
        print("Dummy : -" + finalDamage + " HP");
        print("Dummy : " + currentHP + "/" + maxHPEffective);

        _renderer.UpdateHealthBar(currentHP, maxHPEffective);
        _renderer.DebugDamage(finalDamage);

        return finalDamage;
    }
    

    #endregion
    
    #region Collisions Part

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("HurtBox_Player")) return;
        
        Debug.Log("Tamerelacoli");
            
        var player = col.transform.parent.gameObject;
        var playerComboController = player.GetComponent<SC_ComboController>();
        
        if (hasShield && !isBreaked)
        {
            TakeWeaknessDamage(playerComboController.weaponType);
        }
        else
        {
            TakeDamage(5);
            //ApplyDebuffToSelf(Enum_Debuff.Poison, 1, 10);
        }

    }

    private void OnTriggerExit(Collider col)
    {
        
    }

    #endregion
    
    
}
