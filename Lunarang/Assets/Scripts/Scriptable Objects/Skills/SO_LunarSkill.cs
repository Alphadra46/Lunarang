using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/LunarSkill", fileName = "LunarSkill")]
public class SO_LunarSkill : SO_BaseSkill
{
    [FoldoutGroup("LunarSkill")]
    public bool isUpgradable = false;
    
    [FoldoutGroup("LunarSkill")]
    public int currentLevel = 1;
    [FoldoutGroup("LunarSkill")]
    public int startLevel = 1;

    [PropertySpace(SpaceBefore = 5f)]
    [FoldoutGroup("LunarSkill")]
    public Dictionary<StatTypes, float> baseValue;
    [FoldoutGroup("LunarSkill")]
    public float valueGrowth;
    
    public override void Init()
    {
        
        currentLevel = startLevel;
        SetModifiedValueToBaseValue();
        
        base.Init();
        
    }

    public void IncrementLevel(int newValue)
    {
        if(!isUpgradable) return;
        
        currentLevel += newValue;
        
        UpdateModificationValue();

    }

    public void UpdateModificationValue()
    {
        
        if (SC_PlayerStats.instance == null) return;
        var playerStats = SC_PlayerStats.instance;
        
        foreach (var statModification in statsChangedOnInit)
        {
            var value = baseValue[statModification.StatToModify] + (valueGrowth * (currentLevel-1));
            var previous_value = baseValue[statModification.StatToModify] + (valueGrowth * (currentLevel-2));
            
            var isPositive = statModification.ModificationValue > 0;
            statModification.ModificationValue =
                isPositive ? -previous_value : Math.Abs(previous_value);
            
            playerStats.ModifyStats(playerStats.currentStats, out var preChangeStats, statModification);
            playerStats.currentStats = preChangeStats;
            
            statModification.ModificationValue = value;
            
            playerStats.ModifyStats(playerStats.currentStats, out var updatedStats, statModification);
            playerStats.currentStats = updatedStats;
        }
        
    }

    public void SetModifiedValueToBaseValue()
    {

        foreach (var statModification in statsChangedOnInit)
        {

            statModification.ModificationValue = baseValue[statModification.StatToModify];

        }
    }
    
}
