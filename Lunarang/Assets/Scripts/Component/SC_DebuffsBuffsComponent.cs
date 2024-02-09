using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_DebuffsBuffsComponent : MonoBehaviour
{

    private SC_AIStats _aiStats;
    private SC_PlayerStats _playerStats;

    private bool isPlayer;
    
    #region Status
    
    [TabGroup("Status", "Debuffs")]
    public List<Enum_Buff> currentBuffs;
    [TabGroup("Status", "Debuffs")]
    public List<Enum_Debuff> currentDebuffs;
    
    #endregion

    #region Poison
    
    [TabGroup("DoT", "Poison"), MaxValue("poisonMaxStack"), MinValue(0)]
    public int poisonCurrentStacks = 0;
    
    [PropertySpace(SpaceBefore = 5)]
    [TabGroup("DoT", "Poison"), MaxValue(4), MinValue(0)]
    public int poisonStackByHit = 1;
    [TabGroup("DoT", "Poison")]
    public int poisonMaxStack = 3;
    
    [TabGroup("DoT", "Poison"), MaxValue(2f), MinValue(0.25f)]
    public float poisonTick = 2f;
    [TabGroup("DoT", "Poison"), MaxValue(120f), MinValue(0.25f)]
    public float poisonDuration = 1f;
    [TabGroup("DoT", "Poison")]
    public float poisonDMGBonus = 0f;
    
    #endregion

    #region DoT Damage
    
    [TabGroup("DoT", "Bonus")]
    public float dotCritDamage = 0;
    [TabGroup("DoT", "Bonus")]
    public float dotCritRate = 0;
    [TabGroup("DoT", "Bonus")]
    public float dotDurationBonus = 0;
    
    #endregion


    private void Awake()
    {
        if (TryGetComponent(out _playerStats))
        {
            isPlayer = true;
        }
        else if (TryGetComponent(out _aiStats))
        {
            isPlayer = false;
        }
    }

    public void ApplyDebuff(Enum_Debuff newDebuff, SC_DebuffsBuffsComponent applicator)
    {
         

        switch (newDebuff)
        {
            
            case Enum_Debuff.Poison:
                
                if (currentDebuffs.Contains(Enum_Debuff.Poison))
                {
                    
                    if(poisonCurrentStacks < applicator.poisonMaxStack)
                        
                        poisonCurrentStacks += applicator.poisonStackByHit;
                    
                    else if(poisonCurrentStacks+applicator.poisonStackByHit > applicator.poisonMaxStack)
                        
                        poisonCurrentStacks = applicator.poisonMaxStack;
                    
                }
                else
                {
                    print("POISON");
                    StartCoroutine(PoisonDoT(applicator));
                    currentDebuffs.Add(newDebuff);
                }
                break;
            
            case Enum_Debuff.Bleed:
                currentDebuffs.Add(newDebuff);
                break;
            case Enum_Debuff.Burn:
                currentDebuffs.Add(newDebuff);
                break;
            case Enum_Debuff.Freeze:
                currentDebuffs.Add(newDebuff);
                break;
            case Enum_Debuff.Slowdown:
                currentDebuffs.Add(newDebuff);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newDebuff), newDebuff, null);
        }
        
        

    }


    public bool CheckHasBuff(Enum_Buff buff)
    {
        return currentBuffs.Contains(buff);
    }
    
    public void ApplyBuff(Enum_Buff newBuff, float duration = 0)
    {

        float effectBonus = 0;
        
        if(currentBuffs.Contains(newBuff)) return;
        
        switch (newBuff)
        {
            case Enum_Buff.Armor:
                break;
            case Enum_Buff.SecondChance:
                break;
            case Enum_Buff.Thorns:
                break;
            case Enum_Buff.God:

                if (isPlayer)
                {
                    _playerStats.atkModifier += 1000;
                    _playerStats.bonusCritDMG += 200;
                    _playerStats.bonusCritRate += 95;
                }
                
                break;
            
            case Enum_Buff.RelentlessTorment:

                var base_duration = duration;
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_1"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1").buffsParentEffect.ContainsKey("atkSPD") 
                        ? float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1")
                            .buffsParentEffect["atkSPD"]) : 0;
                    _playerStats.atkSpeedModifier += value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_2"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_2").buffsParentEffect.ContainsKey("duration") 
                        ? float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_2")
                            .buffsParentEffect["duration"]) : 0;
                    duration += base_duration * (value/100);
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3").buffsParentEffect.ContainsKey("effectBonus") 
                        ? float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3")
                            .buffsParentEffect["effectBonus"]) : 0;
                    effectBonus += value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_4"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_4").buffsParentEffect.ContainsKey("duration") 
                        ? float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_4")
                            .buffsParentEffect["duration"]) : 0;
                    duration += base_duration * (value/100);
                }

                _playerStats.dotDamageBonus += 20 * (1+(effectBonus/100));
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newBuff), newBuff, null);
        }
        
        currentBuffs.Add(newBuff);
        
        print(duration);
        if (duration > 0) StartCoroutine(RemoveBuffAfterDuration(newBuff, duration));
        
        if (isPlayer && _playerStats.statsDebug != null)
        {
            _playerStats.statsDebug.RefreshStats();
        }
        
    }

    public void RemoveBuff(Enum_Buff buff)
    {
        float effectBonus = 0;
        
        if(!currentBuffs.Contains(buff)) return;
        
        switch (buff)
        {
            case Enum_Buff.Armor:
                break;
            case Enum_Buff.SecondChance:
                break;
            case Enum_Buff.Thorns:
                break;
            case Enum_Buff.God:
                
                if (isPlayer)
                {
                    _playerStats.atkModifier -= 1000;
                    _playerStats.bonusCritDMG -= 200;
                    _playerStats.bonusCritRate -= 95;
                }
                
                break;
            
            case Enum_Buff.RelentlessTorment:
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_1"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1").buffsParentEffect.ContainsKey("atkSPD") 
                        ? float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1")
                            .buffsParentEffect["atkSPD"]) : 0;
                    _playerStats.atkSpeedModifier -= value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3").buffsParentEffect.ContainsKey("effectBonus") 
                        ? float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3")
                            .buffsParentEffect["effectBonus"]) : 0;
                    effectBonus += value;
                }
                
                _playerStats.dotDamageBonus -= 20 * (1+(effectBonus/100));
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buff), buff, null);
        }

        currentBuffs.Remove(buff);
        
        if (isPlayer && _playerStats.statsDebug != null)
        {
            _playerStats.statsDebug.RefreshStats();
        }

    }

    public IEnumerator RemoveBuffAfterDuration(Enum_Buff buff, float duration)
    {

        yield return new WaitForSeconds(duration);

        RemoveBuff(buff);
        
    }
    
    #region DoT

    /// <summary>
    /// Coroutine for the poison debuff, apply damage every ticks during a certain duration.
    /// </summary>
    /// <param name="applicator"></param>
    /// <returns></returns>
    private IEnumerator PoisonDoT(SC_DebuffsBuffsComponent applicator)
    {
          
        var duration = (applicator.poisonDuration * (1 + (dotDurationBonus / 100)));
        
        var isCritical = Random.Range(0, 100) < dotCritRate ? true : false;
        
        var rawDamage = (applicator.isPlayer ? applicator._playerStats.currentATK : applicator._aiStats.currentATK) * 0.1f;
        
        var effDamage = rawDamage * (1 + (applicator.poisonDMGBonus + (isPlayer ? applicator._playerStats.dotDamageBonus : 0))/100);
        print(effDamage);
        
        var effCrit = effDamage * (1 + (dotCritDamage/100));

        poisonCurrentStacks += applicator.poisonStackByHit;
        
        yield return new WaitForSeconds(applicator.poisonTick);
        
        while (duration > 0)
        {
            print("Poison Tick");
            print("Poison Stack :" + poisonCurrentStacks);
            
            for (var i = 0; i < poisonCurrentStacks; i++)
            {
                GetComponent<IDamageable>().TakeDoTDamage(isCritical ? effCrit : effDamage, isCritical, Enum_Debuff.Poison);
            }
            
            duration -= applicator.poisonTick;
            
            print("Duration :" + duration);
            
            yield return new WaitForSeconds(applicator.poisonTick);
        }

        poisonCurrentStacks = 0;
        currentDebuffs.Remove(Enum_Debuff.Poison);

    }

    #endregion
    
    
}
