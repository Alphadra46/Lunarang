using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class SC_DebuffsBuffsComponent : MonoBehaviour
{
    [HideInInspector] public SC_AIStats _aiStats;
    [HideInInspector] public SC_PlayerStats _playerStats;

    [SerializeField, PropertySpace(SpaceAfter = 10f)]
    public SC_ModifierPanel _modifierPanel;

    [ReadOnly] public bool isPlayer;

    [HideInInspector] public SC_DoT_States doTStates = new SC_DoT_States();
    private Dictionary<Enum_Debuff, GameObject> debuffsVFX = new Dictionary<Enum_Debuff, GameObject>();
    
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
    public float poisonDuration = 10f;
    [TabGroup("DoT", "Poison")]
    public float poisonDMGBonus = 0f;

    [TabGroup("DoT", "Poison")]
    public GameObject poisonVFX;
    
    #endregion
    
    #region Burn
    
    [TabGroup("DoT", "Burn"), MaxValue("burnMaxStack"), MinValue(0)]
    public int burnCurrentStacks = 0;
    
    [PropertySpace(SpaceBefore = 5)]
    [TabGroup("DoT", "Burn")]
    public int burnMaxStack = 5;
    
    [TabGroup("DoT", "Burn")]
    public float burnTick = 1f;
    [TabGroup("DoT", "Burn")]
    public int burnHitRequired = 3;
    [TabGroup("DoT", "Burn")]
    public int burnHitToProc = 0;
    
    [TabGroup("DoT", "Burn")]
    public bool burnCanProc = true;
    [TabGroup("DoT", "Burn")]
    public float burnICD = 1f;
    
    [TabGroup("DoT", "Burn")]
    public float burnAoEMV = 10f;
    [TabGroup("DoT", "Burn")]
    public float burnAoESize = 0.75f;
    [TabGroup("DoT", "Burn")]
    public float burnAoEHitRate = 0f;
    
    [TabGroup("DoT", "Burn")]
    public float burnDoTMV = 5f;
    
    [TabGroup("DoT", "Burn")]
    public float burnDMGBonus = 0f;
    
    [TabGroup("DoT", "Burn")]
    public GameObject burnVFX;
    
    #endregion
    
    #region Bleed
    
    [TabGroup("Debuff", "Bleed")]
    public int bleedCurrentStacks = 0;
    
    [PropertySpace(SpaceBefore = 5)]
    [TabGroup("Debuff", "Bleed")]
    public int bleedMaxStacks = 5;
    
    [TabGroup("Debuff", "Bleed")]
    public int bleedHits = 5;
    
    [TabGroup("Debuff", "Bleed")]
    public float bleedMV = 1f;
    
    [TabGroup("Debuff", "Bleed")]
    public float bleedDMGBonus = 0f;
    
    [TabGroup("Debuff", "Bleed")]
    public GameObject bleedVFX;
    
    #endregion

    #region Freeze
    
    [TabGroup("Debuff", "Freeze"), MaxValue(120f), MinValue(0f)]
    public float freezeDuration = 3f;
    [TabGroup("Debuff", "Freeze"), MaxValue(120f), MinValue(0f)]
    public float freezeDurationBonus = 0f;
    
    [TabGroup("Debuff", "Freeze")]
    public float unfreezeAoESize = 2f;
    [TabGroup("Debuff", "Freeze")]
    public float unfreezeAoEMV = 45f;
    
    [TabGroup("Debuff", "Freeze")]
    public GameObject freezeVFX;
    
    #endregion
    
    #region Slowdown
    
    [TabGroup("Debuff", "Freeze"), MaxValue(120f), MinValue(0f)]
    public float slowdownDuration = 5f;

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

    public void ResetAllBuffsAndDebuffs()
    {

        foreach (var buff in currentBuffs)
        {
            RemoveBuff(buff);
        }

        foreach (var debuff in currentDebuffs)
        {
            _modifierPanel.debuffRemoved?.Invoke(debuff);
        }
        
        currentBuffs.Clear();
        currentDebuffs.Clear();
        
    }
    
    public bool CheckHasDebuff(Enum_Debuff debuff)
    {
        return currentDebuffs.Contains(debuff);
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
                    StartCoroutine(doTStates.PoisonDoT(applicator, this));
                    currentDebuffs.Add(newDebuff);
                    if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                }
                break;
            
            case Enum_Debuff.Bleed:
                
                if(CheckHasDebuff(Enum_Debuff.Bleed))
                {
                    
                    if (bleedCurrentStacks+1 < bleedMaxStacks)
                    {
                        bleedCurrentStacks++;
                    }
                    else
                    {
                        doTStates.BleedDMG(applicator, this);
                        bleedCurrentStacks = 0;
                        currentDebuffs.Remove(newDebuff);
                        if(_modifierPanel != null) _modifierPanel.debuffRemoved?.Invoke(newDebuff);
                        RemoveVFX(Enum_Debuff.Bleed);
                        
                    }
                    
                    return;
                }
                
                currentDebuffs.Add(newDebuff);
                CreateVFX(newDebuff, bleedVFX, transform.position, applicator);
                if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                break;
            
            case Enum_Debuff.Burn:
                if(CheckHasDebuff(Enum_Debuff.Burn)) return;
                
                burnMaxStack = applicator.burnMaxStack;
                
                currentDebuffs.Add(newDebuff);
                if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                
                CreateVFX(newDebuff, burnVFX, transform.position, applicator);
                
                break;
            
            case Enum_Debuff.Freeze:
                if(CheckHasDebuff(Enum_Debuff.Freeze)) return;
                
                StartCoroutine(doTStates.FrozenState(applicator, this));
                currentDebuffs.Add(newDebuff);
                if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                
                CreateVFX(newDebuff, freezeVFX, transform.position, applicator);
                break;
            
            case Enum_Debuff.Slowdown:

                StartCoroutine(doTStates.Slowdown(applicator, this)
                );
                currentDebuffs.Add(newDebuff);
                if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newDebuff), newDebuff, null);
        }

    }


    #region Buff

    public bool CheckHasBuff(Enum_Buff buff)
    {
        return currentBuffs.Contains(buff);
    }
    
    public void ApplyBuff(Enum_Buff newBuff, float duration = 0)
    {

        float effectBonus = 0;
        var base_duration = duration;
        
        if(currentBuffs.Contains(newBuff)) return;
        
        switch (newBuff)
        {
            case Enum_Buff.Armor:

                var shieldValue = (_playerStats.currentStats.currentMaxHealth * 0.2f);

                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_1_Tank"))
                {
                    shieldValue = (_playerStats.currentStats.currentMaxHealth * (SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_1_Tank").buffsParentEffect.TryGetValue("shieldValue", out var value1) 
                        ? float.Parse(value1)/100 : 0.2f));
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_2_Tank"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_2_Tank").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_3_Tank"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_3_Tank").buffsParentEffect.TryGetValue("shieldStrength", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.currentStats.shieldStrength += value;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_4_Tank"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_4_Tank").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                
                _playerStats.CreateShield(shieldValue);
                
                break;
            
            case Enum_Buff.SecondChance:

                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_1_1_Berserk"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_1_1_Berserk").buffsParentEffect.TryGetValue("hpRecovered", out var value1) 
                        ? float.Parse(value1) : 0;
                    effectBonus += value;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_1_2_Berserk"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_1_2_Berserk").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_1_4_Berserk"))
                {
                    _playerStats.currentStats.maxHealthModifier += 50;
                }
                
                _playerStats.currentStats.currentHealth = (int) Mathf.Round(_playerStats.currentStats.currentMaxHealth * (0.05f+(effectBonus/100)));

                _playerStats.currentStats.damageReduction += 30;
                _playerStats.currentStats.atkModifier += 25;
                _playerStats.currentStats.damageBonus += 30;

                _playerStats.currentStats.speedModifier += 10;
                _playerStats.currentStats.atkSpeedModifier += 10;

                break;
            
            case Enum_Buff.Thorns:
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_1_3_Tank"))
                {
                    _playerStats.currentStats.damageReduction += (SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_1_3_Tank").buffsParentEffect.TryGetValue("dmgReductionBonus", out var value1) 
                        ? float.Parse(value1) : 0);
                }
                
                break;
            
            case Enum_Buff.God:

                if (isPlayer)
                {
                    _playerStats.currentStats.atkModifier += 5000;
                    _playerStats.currentStats.damageBonus += 5000;
                    _playerStats.currentStats.bonusCritDMG += 5000;
                    _playerStats.currentStats.bonusCritRate += 95;
                }
                
                break;
            
            case Enum_Buff.RelentlessTorment:

                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_1_DoT"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_1_DoT").buffsParentEffect.TryGetValue("atkSPD", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.currentStats.atkSpeedModifier += value;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_2_DoT"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_2_DoT").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_3_DoT"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_3_DoT").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                    effectBonus += value;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_4_DoT"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_4_DoT").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }

                _playerStats.currentStats.dotDamageBonus += 40 * (1+(effectBonus/100));
                
                break;

            case Enum_Buff.ManaFury:

                float dmgTaken = 0;

                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_1_Berserk"))
                {
                    dmgTaken = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_1_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_3_Berserk"))
                {
                    dmgTaken = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_3_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_4_Berserk"))
                {
                    effectBonus = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_4_Berserk").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                _playerStats.currentStats.damageBonus += (50 + effectBonus);
                _playerStats.currentStats.atkModifier += (40 + effectBonus);
                
                _playerStats.currentStats.damageTaken += (25-dmgTaken);
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(newBuff), newBuff, null);
        }
        
        currentBuffs.Add(newBuff);
        if(_modifierPanel != null) _modifierPanel.buffAdded?.Invoke(newBuff);
        
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
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_3_Tank"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_3_Tank").buffsParentEffect.TryGetValue("shieldStrength", out var value1) 
                        ? float.Parse(value1) : 0;
                    
                    _playerStats.currentStats.shieldStrength -= value;
                }
                
                _playerStats.BreakShield();

                break;
            
            case Enum_Buff.SecondChance:
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_1_4_Berserk"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_1_4_Berserk").buffsParentEffect.TryGetValue("maxHPBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.currentStats.maxHealthModifier -= value;
                }
                
                _playerStats.currentStats.damageReduction -= 30;
                _playerStats.currentStats.atkModifier -= 25;
                _playerStats.currentStats.damageBonus -= 30;

                _playerStats.currentStats.speedModifier -= 10;
                _playerStats.currentStats.atkSpeedModifier -= 10;
                
                break;
            
            case Enum_Buff.Thorns:
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_1_3_Tank"))
                {
                    _playerStats.currentStats.damageReduction -= (SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_1_3_Tank").buffsParentEffect.TryGetValue("dmgReduction", out var value) 
                        ? float.Parse(value) : 0);
                }
                
                break;
            
            case Enum_Buff.God:
                
                if (isPlayer)
                {
                    _playerStats.currentStats.atkModifier -= 5000;
                    _playerStats.currentStats.damageBonus -= 5000;
                    _playerStats.currentStats.bonusCritDMG -= 5000;
                    _playerStats.currentStats.bonusCritRate -= 95;
                }
                
                break;
            
            case Enum_Buff.RelentlessTorment:
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_1_DoT"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_1_DoT").buffsParentEffect.TryGetValue("atkSPD", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.currentStats.atkSpeedModifier -= value;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_3_DoT"))
                {
                    var value = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_3_DoT").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                    effectBonus += value;
                }
                
                _playerStats.currentStats.dotDamageBonus -= 40 * (1+(effectBonus/100));
                
                break;

            case Enum_Buff.ManaFury:

                float dmgTaken = 0;

                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_1_Berserk"))
                {
                    dmgTaken = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_1_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_3_Berserk"))
                {
                    dmgTaken = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_3_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("ChildSkill_3_4_Berserk"))
                {
                    effectBonus = SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("ChildSkill_3_4_Berserk").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                _playerStats.currentStats.damageBonus -= 50 + effectBonus;
                _playerStats.currentStats.atkModifier -= 40 + effectBonus;
                
                _playerStats.currentStats.damageTaken -= (25 - dmgTaken);
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(buff), buff, null);
        }

        currentBuffs.Remove(buff);
        if(_modifierPanel != null) _modifierPanel.buffRemoved?.Invoke(buff);
        
        if (isPlayer && _playerStats.statsDebug != null)
        {
            _playerStats.statsDebug.RefreshStats();
        }

    }

    #endregion

    private IEnumerator RemoveBuffAfterDuration(Enum_Buff buff, float duration)
    {

        yield return new WaitForSeconds(duration);

        RemoveBuff(buff);
        
    }
    
    #region Coroutines

    public IEnumerator BuffStatTemp(SC_StatModification newModification)
    {
        
        if (_playerStats == null) yield break;
        
        var isPositive = newModification.ModificationValue > 0;
        
        _playerStats.ModifyStats(_playerStats.currentStats, out var modifiedStats, newModification);
        _playerStats.currentStats = modifiedStats;

        yield return new WaitForSeconds(newModification.timer);

        newModification.ModificationValue =
            isPositive ? -newModification.ModificationValue : Math.Abs(newModification.ModificationValue);

        _playerStats.ModifyStats(_playerStats.currentStats, out modifiedStats, newModification);
        _playerStats.currentStats = modifiedStats;

    }
    
    #endregion


    public void CreateVFX(Enum_Debuff debuff, GameObject vfxObject, Vector3 position, SC_DebuffsBuffsComponent applicator)
    {

        var go = Instantiate(vfxObject, transform);
        var vfx = go.GetComponent<VisualEffect>();
        vfx.transform.position = position;

        if (debuff == Enum_Debuff.Freeze)
        {
            var duration = ( applicator.freezeDuration * 
                             (1 + ( applicator.freezeDurationBonus / 100)));

            vfx.SetFloat("Duration", duration);

        }
        
        vfx.Play();
        
        debuffsVFX.Add(debuff, go);
        
    }

    public void RemoveVFX(Enum_Debuff debuff)
    {

        var vfx = debuffsVFX[debuff].GetComponent<VisualEffect>();
        
        vfx.Stop();
        
        Destroy(vfx.gameObject);

        debuffsVFX.Remove(debuff);

    }
    
}
