using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;


public enum ConstellationName
{
    Lunar,
    DoT,
    Berserker,
    Tank,
    Freeze
}

public class SO_BaseSkill : SerializedScriptableObject
{

    public string skillName;
    public ConstellationName constellation;
    
    [MultiLineProperty] public string shortDescription;
    [MultiLineProperty] public string longDescription;

    [PropertySpace(SpaceBefore = 5)]
    public List<SC_StatModification> statsChangedOnInit = new List<SC_StatModification>();
    public Dictionary<Enum_Buff, string> buffsAppliedOnInit = new Dictionary<Enum_Buff, string>();
    
    [PropertySpace(SpaceBefore = 5)]
    public SO_Event eventEnabler;
    public SO_Event eventDisabler;
    
    [PropertySpace(SpaceBefore = 5)]
    public List<SC_StatModification> statsChangedOnEvent = new List<SC_StatModification>();
    [PropertySpace(SpaceAfter = 5)]
    public Dictionary<Enum_Buff, string> buffsAppliedOnEvent = new Dictionary<Enum_Buff, string>();
    
    
    private Dictionary<StatTypes, string> tempStatsSave = new Dictionary<StatTypes, string>();
    
    public virtual void Init()
    {
        
        // StatsChanges(statsChangedOnInit);
        StatsChanges(statsChangedOnInit);
        ApplyBuffs(buffsAppliedOnInit);

        if(eventEnabler != null)
            eventEnabler.OnEventRaised += OnEvent;
        if(eventDisabler != null)
            eventDisabler.OnEventRaised += OnEventEnd;

    }

    public void OnEvent()
    {
        // StatsChanges(statsChangedOnEvent);
        ApplyBuffs(buffsAppliedOnEvent);
    }
    
    public void OnEventEnd()
    {
        RemoveBuffs(buffsAppliedOnEvent);
    }

    public void StatsChanges(List<SC_StatModification> statModifications)
    {
        
        if (SC_PlayerStats.instance == null) return;
        var playerStats = SC_PlayerStats.instance;

        foreach (var statModification in statModifications)
        {
            playerStats.ModifyStats(playerStats.currentStats, out var modifiedStats ,statModification);
            playerStats.currentStats = modifiedStats;
        }
        
    }
    
    public void ApplyBuffs(Dictionary<Enum_Buff, string> buffsList)
    {
        if (!SC_PlayerStats.instance.gameObject.TryGetComponent(out SC_DebuffsBuffsComponent debuffsBuffsComponent)) return;

        foreach (var buff in buffsList)
        {
            if(buff.Value == "")
                debuffsBuffsComponent.ApplyBuff(buff.Key);
            else
                debuffsBuffsComponent.ApplyBuff(buff.Key, Convert.ToSingle(buff.Value));
        }
        
    }
    
    public void RemoveBuffs(Dictionary<Enum_Buff, string> buffsList)
    {
        if (!SC_PlayerStats.instance.gameObject.TryGetComponent(out SC_DebuffsBuffsComponent debuffsBuffsComponent)) return;

        foreach (var buff in buffsList.Where(buff => debuffsBuffsComponent.CheckHasBuff(buff.Key)))
        {
            debuffsBuffsComponent.RemoveBuff(buff.Key);
        }
    }
    
}
