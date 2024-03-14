using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_LunarSkill : SO_BaseSkill
{

    public bool isUpgradable = false;

    public int maxLevel = 10;
    public int currentLevel = 1;

    public float valueGrowth;
    
    public override void Init()
    {
        base.Init();
    }

    public void IncrementLevel(int newValue)
    {

        currentLevel += newValue;

    }

    public void UpdateModificationValue()
    {
        
        foreach (var statModification in statsChangedOnInit)
        {
            var value = statModification.ModificationValue + (valueGrowth * (currentLevel-1));
            statModification.ModificationValue = value;
        }
        
    }
    
}
