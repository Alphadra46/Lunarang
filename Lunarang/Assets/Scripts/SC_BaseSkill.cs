using System;
using System.Collections.Generic;
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
    
    
    PoisonHitRate,
    
}

public class SC_BaseSkill : ScriptableObject
{

    public string skillName;
    public ConstellationName constellation;
    
    public string shortDescription;
    public string longDescription;

    public Dictionary<Stats, string> statsChangedPlayer = new Dictionary<Stats, string>();
    public List<string> buffsApplied = new List<string>();

    public void Init()
    {
        
        foreach (var stat in statsChangedPlayer)
        {
            var value = 0f;
            
            if(stat.Value.Contains("+") || stat.Value.Contains("-")) {
                var subs = stat.Value.Split('+', '-');
                value = float.Parse(subs[1]);
            }
            else
            {
                value = float.Parse(stat.Value);
            }
            
            switch (stat.Key)
            {
                case Stats.HP:
                    SC_PlayerStats.instance.currentHealth = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.HPMAX:
                    SC_PlayerStats.instance.maxHealthModifier = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.Healing:
                    SC_PlayerStats.instance.healingBonus = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.DishesEffect:
                    SC_PlayerStats.instance.dishesEffectBonus = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.ATK:
                    SC_PlayerStats.instance.atkModifier = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.DEF:
                    SC_PlayerStats.instance.defModifier = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.DMGReduction:
                    SC_PlayerStats.instance.damageReduction = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.DMGTaken:
                    SC_PlayerStats.instance.damageTaken = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.DoTDMGTaken:
                    SC_PlayerStats.instance.dotDamageTaken = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.ATKSPD:
                    SC_PlayerStats.instance.atkSpeedModifier = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.MovementSPD:
                    SC_PlayerStats.instance.speedModifier = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.DashSPD:
                    // TODO
                    break;
                case Stats.DMG:
                    SC_PlayerStats.instance.damageBonus = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.MultiHitDMG:
                    SC_PlayerStats.instance.mhDamageBonus = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.AoEDMG:
                    SC_PlayerStats.instance.aoeDamageBonus = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.ProjectileDMG:
                    SC_PlayerStats.instance.projectileDamageBonus = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.DoTDMG:
                    SC_PlayerStats.instance.dotDamageBonus = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
                case Stats.PoisonHitRate:
                    SC_PlayerStats.instance.poisonHitRate = stat.Value.Contains("+") ? +value : stat.Value.Contains("-") ? -value : value ;
                    break;
            }
        }
        
    }
    
    
}
