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
    Freeze,
    Burn,
    Bleed
}

public class SO_BaseSkill : SerializedScriptableObject
{
    
    public string skillName;
    public ConstellationName constellation;
    public SC_Constellation constellationSC;
    
    [MultiLineProperty] public string shortDescription;
    [MultiLineProperty] public string longDescription;
    public Sprite crystal;
    public Sprite crystalIcon;
    public int spCost;
    
    [FoldoutGroup("BaseSkill")]
    [PropertySpace(SpaceBefore = 5)]
    public List<SC_StatModification> statsChangedOnInit = new List<SC_StatModification>();
    [FoldoutGroup("BaseSkill")]
    public Dictionary<Enum_Buff, string> buffsAppliedOnInit = new Dictionary<Enum_Buff, string>();
    
    
    [FoldoutGroup("BaseSkill")]
    [PropertySpace(SpaceBefore = 5)]
    public SO_Event eventEnabler;
    [FoldoutGroup("BaseSkill")]
    public SO_Event eventDisabler;
    
    [FoldoutGroup("BaseSkill")]
    [PropertySpace(SpaceBefore = 5)]
    public List<SC_StatModification> statsChangedOnEvent = new List<SC_StatModification>();
    [FoldoutGroup("BaseSkill")]
    [PropertySpace(SpaceAfter = 5)]
    public Dictionary<Enum_Buff, string> buffsAppliedOnEvent = new Dictionary<Enum_Buff, string>();
    
    [FoldoutGroup("BaseSkill")]
    private Dictionary<StatTypes, string> tempStatsSave = new Dictionary<StatTypes, string>();

    public bool isKnown;
    
    public virtual void Init()
    {
        if (!isKnown) //Used to change the skill icon on the skill tree UI
            isKnown = true;
        
        
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
        
        SC_PlayerStats.onUpdatedStats?.Invoke();
    }
    
    public void ApplyBuffs(Dictionary<Enum_Buff, string> buffsList)
    {
        if (SC_PlayerStats.instance == null)
            return;
        
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
