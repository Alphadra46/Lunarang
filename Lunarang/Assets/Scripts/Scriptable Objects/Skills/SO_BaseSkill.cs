using System;
using System.Collections.Generic;
using System.Linq;
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

public enum Stats
{
    HP,
    HPMAX,
    Healing,
    DishesEffect,
    
    ATK,
    
    DEF,
    DMGReduction,
    
    DMGTaken,
    DoTDMGTaken,
    
    ATKSPD,
    MovementSPD,
    DashSPD,
    
    DMG,
    MultiHitDMG,
    AoEDMG,
    ProjectileDMG,
    
    DoTDMG,
    
    DoTCritRate,
    DoTCritDMG,
    DoTDurationBonus,
    
    PoisonHitRate,
    PoisonStackByHit,
    PoisonMaxStack,
    PoisonTick,
    PoisonDuration,
    PoisonDMG,
    
    BleedHitRate,
    BleedStackByHit,
    BleedMaxStack,
    BleedTick,
    BleedDuration,
    BleedDMG,
    
    BurnHitRate,
    BurnAoESize,
    BurnTick,
    BurnMaxDuration,
    BurnAoEHitRate,
    BurnDMG,
    
    FreezeHitRate,
    FreezeDuration,
    FreezeDurationBonus,
    UnfreezeAoESize,
    UnfreezeAoEMV,
    UnfreezeAoEHitRate,
    
    ManaOverloadMaxStack,
    ManaOverloadDamageTick,
    ManaOverloadDuration,
    
    ManaFuryMaxHP
    
}

public class SO_BaseSkill : SerializedScriptableObject
{

    public string skillName;
    public ConstellationName constellation;
    
    [MultiLineProperty] public string shortDescription;
    [MultiLineProperty] public string longDescription;

    [PropertySpace(SpaceBefore = 5)]
    public Dictionary<Stats, string> statsChangedOnInit = new Dictionary<Stats, string>();
    public Dictionary<Enum_Buff, string> buffsAppliedOnInit = new Dictionary<Enum_Buff, string>();
    
    [PropertySpace(SpaceBefore = 5)]
    public SO_Event eventEnabler;
    public SO_Event eventDisabler;
    
    [PropertySpace(SpaceBefore = 5)]
    public Dictionary<Stats, string> statsChangedOnEvent = new Dictionary<Stats, string>();
    [PropertySpace(SpaceAfter = 5)]
    public Dictionary<Enum_Buff, string> buffsAppliedOnEvent = new Dictionary<Enum_Buff, string>();
    
    
    private Dictionary<Stats, string> tempStatsSave = new Dictionary<Stats, string>();
    
    public virtual void Init()
    {
        
        StatsChanges(statsChangedOnInit);
        ApplyBuffs(buffsAppliedOnInit);

        if(eventEnabler != null)
            eventEnabler.OnEventRaised += OnEvent;
        if(eventDisabler != null)
            eventDisabler.OnEventRaised += OnEventEnd;

    }

    public void OnEvent()
    {
        SaveStatsBeforeEvent(statsChangedOnEvent);
        
        StatsChanges(statsChangedOnEvent);
        ApplyBuffs(buffsAppliedOnEvent);
    }
    
    public void OnEventEnd()
    {
        LoadStatsBeforeEvent();
        RemoveBuffs(buffsAppliedOnEvent);
    }

    public void StatsChanges(Dictionary<Stats, string> dictionary)
    {
        if (!SC_PlayerStats.instance.gameObject.TryGetComponent(out SC_DebuffsBuffsComponent debuffsBuffsComponent)) return;
        
        foreach (var stat in dictionary)
        {
            var value = 0f;
            
            if(stat.Value.Contains("+") || stat.Value.Contains("-")) {
                var subs = stat.Value.Split('+', '-', '%');
                value = float.Parse(subs[1]);
            }
            else
            {
                value = float.Parse(stat.Value);
            }
            
            switch (stat.Key)
            {
                case Stats.HP:
                    SC_PlayerStats.instance.currentHealth = stat.Value.Contains("+") ? SC_PlayerStats.instance.currentHealth+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.currentHealth-value : value ;
                    break;
                
                case Stats.HPMAX:
                    SC_PlayerStats.instance.maxHealthModifier = stat.Value.Contains("+") ? SC_PlayerStats.instance.maxHealthModifier+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.maxHealthModifier-value : value ;
                    break;
                
                case Stats.Healing:
                    SC_PlayerStats.instance.healingBonus = stat.Value.Contains("+") ?  SC_PlayerStats.instance.healingBonus+value : stat.Value.Contains("-") ?  SC_PlayerStats.instance.healingBonus-value : value ;
                    break;
                
                case Stats.DishesEffect:
                    SC_PlayerStats.instance.dishesEffectBonus = stat.Value.Contains("+") ? SC_PlayerStats.instance.dishesEffectBonus+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.dishesEffectBonus-value : value ;
                    break;
                
                case Stats.ATK:
                    SC_PlayerStats.instance.atkModifier = stat.Value.Contains("+") ? SC_PlayerStats.instance.atkModifier+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.atkModifier-value : value ;
                    break;
                
                case Stats.DEF:
                    SC_PlayerStats.instance.defModifier = stat.Value.Contains("+") ? SC_PlayerStats.instance.defModifier+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.defModifier-value : value ;
                    break;
                case Stats.DMGReduction:
                    SC_PlayerStats.instance.damageReduction = stat.Value.Contains("+") ? SC_PlayerStats.instance.damageReduction+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.damageReduction-value : value ;
                    break;
                case Stats.DMGTaken:
                    SC_PlayerStats.instance.damageTaken = stat.Value.Contains("+") ? SC_PlayerStats.instance.damageTaken+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.damageTaken-value : value ;
                    break;
                case Stats.DoTDMGTaken:
                    SC_PlayerStats.instance.dotDamageTaken = stat.Value.Contains("+") ? SC_PlayerStats.instance.dotDamageTaken+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.dotDamageTaken-value : value ;
                    break;
                case Stats.ATKSPD:
                    SC_PlayerStats.instance.atkSpeedModifier = stat.Value.Contains("+") ? SC_PlayerStats.instance.atkSpeedModifier+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.atkSpeedModifier-value : value ;
                    break;
                case Stats.MovementSPD:
                    SC_PlayerStats.instance.speedModifier = stat.Value.Contains("+") ? SC_PlayerStats.instance.speedModifier+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.speedModifier-value : value ;
                    break;
                case Stats.DashSPD:
                    // TODO
                    break;
                case Stats.DMG:
                    SC_PlayerStats.instance.damageBonus = stat.Value.Contains("+") ? SC_PlayerStats.instance.damageBonus+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.damageBonus-value : value ;
                    break;
                case Stats.MultiHitDMG:
                    SC_PlayerStats.instance.mhDamageBonus = stat.Value.Contains("+") ? SC_PlayerStats.instance.mhDamageBonus+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.mhDamageBonus-value : value ;
                    break;
                case Stats.AoEDMG:
                    SC_PlayerStats.instance.aoeDamageBonus = stat.Value.Contains("+") ? SC_PlayerStats.instance.aoeDamageBonus+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.aoeDamageBonus-value : value ;
                    break;
                case Stats.ProjectileDMG:
                    SC_PlayerStats.instance.projectileDamageBonus = stat.Value.Contains("+") ? SC_PlayerStats.instance.projectileDamageBonus+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.projectileDamageBonus-value : value ;
                    break;
                case Stats.DoTDMG:
                    SC_PlayerStats.instance.dotDamageBonus = stat.Value.Contains("+") ? SC_PlayerStats.instance.dotDamageBonus+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.dotDamageBonus-value : value ;
                    break;
                case Stats.PoisonHitRate:
                    SC_PlayerStats.instance.poisonHitRate = stat.Value.Contains("+") ? SC_PlayerStats.instance.poisonHitRate+value : stat.Value.Contains("-") ? SC_PlayerStats.instance.poisonHitRate-value : value ;
                    break;
                case Stats.PoisonStackByHit:
                    debuffsBuffsComponent.poisonStackByHit = (int) 
                        (stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.poisonStackByHit+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.poisonStackByHit-value 
                            : value);
                    
                    break;
                case Stats.PoisonTick:
                    debuffsBuffsComponent.poisonTick = 
                        stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.poisonTick+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.poisonTick-value 
                                : value;
                    
                    break;
                
                case Stats.PoisonDuration:
                    debuffsBuffsComponent.poisonDuration =
                        stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.poisonDuration+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.poisonDuration-value 
                                : value;
                    
                    break;
                    
                case Stats.PoisonDMG:
                    debuffsBuffsComponent.poisonDMGBonus = 
                        stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.poisonDMGBonus+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.poisonDMGBonus-value 
                                : value;
                    
                    break;

                case Stats.PoisonMaxStack:
                    debuffsBuffsComponent.poisonMaxStack = 
                        (int)(stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.poisonMaxStack+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.poisonMaxStack-value 
                                : value);
                    
                    break;

                case Stats.DoTCritRate:
                    debuffsBuffsComponent.dotCritRate = 
                        (int)(stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.dotCritRate+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.dotCritRate-value 
                                : value);
                    
                    break;
                
                case Stats.DoTCritDMG:
                    debuffsBuffsComponent.dotCritDamage = 
                        (int)(stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.dotCritDamage+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.dotCritDamage-value 
                                : value);
                    
                    break;
                
                case Stats.DoTDurationBonus:
                    debuffsBuffsComponent.dotDurationBonus = 
                        (int)(stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.dotDurationBonus+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.dotDurationBonus-value 
                                : value);
                    break;

                case Stats.BleedHitRate:
                    break;
                case Stats.BleedStackByHit:
                    break;
                case Stats.BleedMaxStack:
                    break;
                case Stats.BleedTick:
                    break;
                case Stats.BleedDuration:
                    break;
                case Stats.BleedDMG:
                    break;
                
                case Stats.BurnHitRate:
                    break;
                case Stats.BurnAoESize:
                    break;
                case Stats.BurnTick:
                    break;
                case Stats.BurnMaxDuration:
                    break;
                case Stats.BurnAoEHitRate:
                    break;
                case Stats.BurnDMG:
                    break;
                
                case Stats.FreezeHitRate:
                    break;
                case Stats.FreezeDuration:
                    debuffsBuffsComponent.freezeDuration = 
                        (int)(stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.freezeDuration+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.freezeDuration-value 
                                : value);
                    break;
                case Stats.FreezeDurationBonus:
                    debuffsBuffsComponent.freezeDurationBonus = 
                        (int)(stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.freezeDurationBonus+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.freezeDurationBonus-value 
                                : value);
                    
                    break;
                case Stats.UnfreezeAoESize:
                    debuffsBuffsComponent.unfreezeAoESize = 
                        (int)(stat.Value.Contains("+") ? 
                            debuffsBuffsComponent.unfreezeAoESize+value 
                            : stat.Value.Contains("-") ?
                                debuffsBuffsComponent.unfreezeAoESize-value 
                                : value);
                    break;
                case Stats.UnfreezeAoEMV:
                    debuffsBuffsComponent.unfreezeAoEMV = 
                        (int)(stat.Value.Contains("+") ? 
                        debuffsBuffsComponent.unfreezeAoEMV+value 
                        : stat.Value.Contains("-") ?
                            debuffsBuffsComponent.unfreezeAoEMV-value 
                            : value);
                    break;
                case Stats.UnfreezeAoEHitRate:
                    break;
                
                case Stats.ManaOverloadMaxStack:
                    SC_PlayerStats.instance.manaOverloadMaxStack = 
                        (int) (stat.Value.Contains("+") ? SC_PlayerStats.instance.manaOverloadMaxStack+value 
                            : stat.Value.Contains("-") ? 
                                SC_PlayerStats.instance.manaOverloadMaxStack-value 
                                : value);
                    break;
                case Stats.ManaOverloadDamageTick:
                    SC_PlayerStats.instance.manaOverloadTick = 
                        stat.Value.Contains("+") && stat.Value.Contains("%")? SC_PlayerStats.instance.manaOverloadTick*(1+(value/100))
                            : stat.Value.Contains("-") && stat.Value.Contains("%") ? 
                                SC_PlayerStats.instance.manaOverloadTick*(1-(value/100))
                                : value;
                    break;
                case Stats.ManaOverloadDuration:
                    SC_PlayerStats.instance.manaOverloadDuration = 
                        stat.Value.Contains("+") && stat.Value.Contains("%")? SC_PlayerStats.instance.manaOverloadDuration*(1+(value/100))
                            : stat.Value.Contains("-") && stat.Value.Contains("%") ? 
                                SC_PlayerStats.instance.manaOverloadDuration*(1-(value/100))
                                : value;
                    break;

                case Stats.ManaFuryMaxHP:
                    SC_PlayerStats.instance.manaFuryMaxHPGate = 
                        stat.Value.Contains("+")? SC_PlayerStats.instance.manaFuryMaxHPGate+value
                            : stat.Value.Contains("-") ? 
                                SC_PlayerStats.instance.manaFuryMaxHPGate-value
                                : value;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void SaveStatsBeforeEvent(Dictionary<Stats, string> dictionary)
    {
        if (!SC_PlayerStats.instance.gameObject.TryGetComponent(out SC_DebuffsBuffsComponent debuffsBuffsComponent)) return;
        
        foreach (var stat in dictionary)
        {
            
            switch (stat.Key)
            {
                case Stats.HP:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.currentHealth.ToString());
                    break;
                
                case Stats.HPMAX:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.maxHealthModifier.ToString());
                    break;
                
                case Stats.Healing:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.healingBonus.ToString());
                    break;
                
                case Stats.DishesEffect:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.dishesEffectBonus.ToString());
                    break;
                
                case Stats.ATK:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.atkModifier.ToString());
                    break;
                
                case Stats.DEF:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.defModifier.ToString());
                    break;
                case Stats.DMGReduction:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.damageReduction.ToString());
                    break;
                case Stats.DMGTaken:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.damageTaken.ToString());
                    break;
                case Stats.DoTDMGTaken:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.dotDamageTaken.ToString());
                    break;
                case Stats.ATKSPD:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.atkSpeedModifier.ToString());
                    break;
                case Stats.MovementSPD:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.speedModifier.ToString());
                    break;
                case Stats.DashSPD:
                    // TODO
                    break;
                case Stats.DMG:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.damageBonus.ToString());
                    break;
                case Stats.MultiHitDMG:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.mhDamageBonus.ToString());
                    break;
                case Stats.AoEDMG:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.aoeDamageBonus.ToString());
                    break;
                case Stats.ProjectileDMG:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.projectileDamageBonus.ToString());
                    break;
                case Stats.DoTDMG:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.dotDamageBonus.ToString());
                    break;
                case Stats.PoisonHitRate:
                    tempStatsSave.Add(stat.Key, SC_PlayerStats.instance.poisonHitRate.ToString());
                    break;
                case Stats.PoisonStackByHit:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.poisonStackByHit.ToString());
                    break;
                case Stats.PoisonTick:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.poisonTick.ToString());
                    break;
                
                case Stats.PoisonDuration:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.poisonDuration.ToString());
                    break;
                    
                case Stats.PoisonDMG:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.poisonDMGBonus.ToString());
                    break;

                case Stats.PoisonMaxStack:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.poisonMaxStack.ToString());
                    break;

                case Stats.DoTCritRate:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.dotCritRate.ToString());
                    break;
                
                case Stats.DoTCritDMG:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.dotCritDamage.ToString());
                    break;
                
                case Stats.DoTDurationBonus:
                    tempStatsSave.Add(stat.Key, debuffsBuffsComponent.dotDurationBonus.ToString());
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public void LoadStatsBeforeEvent()
    {
        
        if (!SC_PlayerStats.instance.gameObject.TryGetComponent(out SC_DebuffsBuffsComponent debuffsBuffsComponent)) return;
    
        foreach (var stat in tempStatsSave)
        {
        
            switch (stat.Key)
            {
                case Stats.HP:
                    SC_PlayerStats.instance.currentHealth = Convert.ToSingle(stat.Value);
                    break;
                
                case Stats.HPMAX:
                    SC_PlayerStats.instance.maxHealthModifier = (float)stat.Key;
                    break;
                
                case Stats.Healing:
                    SC_PlayerStats.instance.healingBonus = (float)stat.Key;
                    break;
                
                case Stats.DishesEffect:
                    SC_PlayerStats.instance.dishesEffectBonus = (float)stat.Key;;
                    break;
                
                case Stats.ATK:
                    SC_PlayerStats.instance.atkModifier = (float)stat.Key;;
                    break;
                
                case Stats.DEF:
                    SC_PlayerStats.instance.defModifier = (float)stat.Key;
                    break;
                case Stats.DMGReduction:
                    SC_PlayerStats.instance.damageReduction = (float)stat.Key;;
                    break;
                case Stats.DMGTaken:
                    SC_PlayerStats.instance.damageTaken = (float)stat.Key;
                    break;
                case Stats.DoTDMGTaken:
                    SC_PlayerStats.instance.dotDamageTaken = (float)stat.Key;
                    break;
                case Stats.ATKSPD:
                    SC_PlayerStats.instance.atkSpeedModifier =  (float)stat.Key;
                    break;
                case Stats.MovementSPD:
                    SC_PlayerStats.instance.speedModifier = (float)stat.Key;
                    break;
                case Stats.DashSPD:
                    // TODO
                    break;
                case Stats.DMG:
                    SC_PlayerStats.instance.damageBonus = (float)stat.Key;
                    break;
                case Stats.MultiHitDMG:
                    SC_PlayerStats.instance.mhDamageBonus = (float)stat.Key;
                    break;
                case Stats.AoEDMG:
                    SC_PlayerStats.instance.aoeDamageBonus = (float)stat.Key;
                    break;
                case Stats.ProjectileDMG:
                    SC_PlayerStats.instance.projectileDamageBonus = (float)stat.Key;
                    break;
                case Stats.DoTDMG:
                    SC_PlayerStats.instance.dotDamageBonus = (float)stat.Key;
                    break;
                case Stats.PoisonHitRate:
                    SC_PlayerStats.instance.poisonHitRate = (float)stat.Key;
                    break;
                case Stats.PoisonStackByHit:
                    debuffsBuffsComponent.poisonStackByHit = (int)stat.Key;
                    
                    break;
                case Stats.PoisonTick:
                    debuffsBuffsComponent.poisonTick = (float)stat.Key;
                    
                    break;
                
                case Stats.PoisonDuration:
                    debuffsBuffsComponent.poisonDuration = (float)stat.Key;
                    
                    break;
                    
                case Stats.PoisonDMG:
                    debuffsBuffsComponent.poisonDMGBonus = (float)stat.Key;
                    break;

                case Stats.PoisonMaxStack:
                    debuffsBuffsComponent.poisonMaxStack = (int)stat.Key;
                    break;

                case Stats.DoTCritRate:
                    debuffsBuffsComponent.dotCritRate = (float)stat.Key;
                    break;
                
                case Stats.DoTCritDMG:
                    debuffsBuffsComponent.dotCritDamage = (float)stat.Key;
                    break;
                
                case Stats.DoTDurationBonus:
                    debuffsBuffsComponent.dotDurationBonus = (float)stat.Key;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        
        tempStatsSave.Clear();
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
