using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_AIStats : MonoBehaviour
{

    #region Variables

    #region Stats
    
    [Header("Stats Settings")]
    
    // Max HP and HP
    [Tooltip("Current base MaxHP of the enemy")] public float maxHPBase = 15;
    [Tooltip("Current MaxHP multiplier of the enemy")] public float maxHPModifier = 0;
    private float maxHPEffective => maxHPBase * (1 + maxHPModifier);
    private float currentHP;
    
    [Space(5)]
    
    // DEF
    [Tooltip("Current base DEF of the enemy")] public float defBase = 1;
    [Tooltip("Current DEF multiplier of the enemy")] public float defModifier = 0;
    private float defEffective => defBase * (1 + defModifier);
    
    [Space(5)]
    
    // ATK
    [Tooltip("Current base ATK of the enemy")] public float atkBase = 1;
    [Tooltip("Current ATK multiplier of the enemy")] public float atkModifier = 0;
    private float atkEffective => atkBase * (1 + atkModifier);
    
    [Space(5)]
    
    // Speed
    [Tooltip("Current base Speed of the enemy")] public float speedBase = 5;
    [Tooltip("Current Speed multiplier of the enemy")] public float speedModifier = 0;
    private float speedEffective => speedBase * (1 + speedModifier);
    
    
    #endregion

    [Space(10)]
    
    #region Status - Buff, Debuff, and Shields
    
    [Header("Status")]

    #region Shield

    [Space(2.5f)]
    [Tooltip("Has a shield to break before taking damage")] public bool hasShield;
    [Tooltip("How many weaknesses to broke the shield")] public int weaknessLength = 3;
    [Tooltip("Is the weaknesses randomize ?")] public bool randomWeakness = true;
    [Space(2.5f)]
    [Tooltip("His weakness can regenerate ?")] public bool canRegenShield = true;
    [Tooltip("After how many seconds ?")] public float delayBeforeRegen = 4f;
    [Space(2.5f)]
    [Tooltip("Is the shield broken?")] public bool isBreaked;
    [Space(5)]
    [Tooltip("Current weakness of the shield")] public List<WeaponType> currentWeakness;
    [Tooltip("Previous weakness of the shield")] public List<WeaponType> previousWeakness;

    #endregion
    
    
    [Tooltip("List of all current debuffs on this enemy"), SerializeField] private List<Enum_Debuff> currentDebuffs;
    
    #endregion
    
    #region Debug

    [Space(10)]
    [Header("Debug")] 
    public TextMeshProUGUI debugUIHP;
    public TextMeshProUGUI debugUIWeaknesses;

    #endregion
    
    #endregion


    #region Init
    
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

        DebugWeaknesses();

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

            debugUIHP.text = currentHP + "/" + maxHPEffective;
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
            DebugWeaknesses();
        }
        else
        {
            currentWeakness.Clear();
            currentWeakness = previousWeakness.ToList();
            DebugWeaknesses();
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

        debugUIHP.text = currentHP + "/" + maxHPEffective;

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
            // TakeDamage(5);
            ApplyDebuffToSelf(Enum_Debuff.Poison, 1, 10);
        }

    }

    private void OnTriggerExit(Collider col)
    {
        
    }

    #endregion

    #region Debug Part

    private void DebugWeaknesses()
    {
        debugUIWeaknesses.text = "";

        for (var i = 0; i < currentWeakness.Count; i++)
        {
            if(i != currentWeakness.Count - 1)
            {
                debugUIWeaknesses.text += currentWeakness[i].ToString()[6..] + " | ";
            }
            else
            {
                debugUIWeaknesses.text += currentWeakness[i].ToString()[6..];
            }
        }
    }

    #endregion
    
}
