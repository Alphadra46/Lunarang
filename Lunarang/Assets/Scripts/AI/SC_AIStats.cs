using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AIStats : MonoBehaviour
{

    #region Variables
    
    [Header("Stats Settings")]
    [Tooltip("Current base HP of the enemy")] public float hpBase = 0;
    [Tooltip("Current HP multiplier of the enemy")] public float hpMultiplier = 0;
    [Tooltip("Current effective HP of the enemy")] private float hpEffective;
    
    [Tooltip("Current base DEF of the enemy")] public float defBase = 0;
    [Tooltip("Current DEF multiplier of the enemy")] public float defMultiplier = 0;
    [Tooltip("Current effective DEF of the enemy")] private float defEffective;
    
    [Tooltip("Current base ATK of the enemy")] public float atkBase = 0;
    [Tooltip("Current ATK multiplier of the enemy")] public float atkMultiplier = 0;
    [Tooltip("Current effective ATK of the enemy")] private float atkEffective;
    
    [Tooltip("Current base Speed of the enemy")] public float speedBase = 0;
    [Tooltip("Current Speed multiplier of the enemy")] public float speedMultiplier = 0;
    [Tooltip("Current effective Speed of the enemy")] private float speedEffective;

    [Header("Status")] public List<WeaponType> currentWeakness;
    
    #endregion


    private void Start()
    {
        
        
        
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
                
        }
        
    }

    private void OnTriggerExit(Collider col)
    {
        
    }
    
}
