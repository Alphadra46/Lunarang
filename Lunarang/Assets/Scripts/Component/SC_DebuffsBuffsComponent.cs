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

    [SerializeField, PropertySpace(SpaceAfter = 10f)] private SC_ModifierPanel _modifierPanel;

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
    public float poisonDuration = 10f;
    [TabGroup("DoT", "Poison")]
    public float poisonDMGBonus = 0f;
    
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
                    StartCoroutine(PoisonDoT(applicator));
                    currentDebuffs.Add(newDebuff);
                    if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                }
                break;
            
            case Enum_Debuff.Bleed:
                currentDebuffs.Add(newDebuff);
                if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                break;
            
            case Enum_Debuff.Burn:
                currentDebuffs.Add(newDebuff);
                if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                break;
            
            case Enum_Debuff.Freeze:
                if(CheckHasDebuff(Enum_Debuff.Freeze)) return;
                
                StartCoroutine(FrozenState(applicator));
                currentDebuffs.Add(newDebuff);
                if(_modifierPanel != null) _modifierPanel.debuffAdded?.Invoke(newDebuff);
                break;
            
            case Enum_Debuff.Slowdown:

                StartCoroutine(Slowdown(applicator)
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

                var shieldValue = (_playerStats.currentMaxHealth * 0.2f);

                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_1_Tank"))
                {
                    shieldValue = (_playerStats.currentMaxHealth * (SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1_Tank").buffsParentEffect.TryGetValue("shieldValue", out var value1) 
                        ? float.Parse(value1)/100 : 0.2f));
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_2_Tank"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_2_Tank").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3_Tank"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3_Tank").buffsParentEffect.TryGetValue("shieldStrength", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.shieldStrength += value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_4_Tank"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_4_Tank").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                
                _playerStats.CreateShield(shieldValue);
                
                break;
            
            case Enum_Buff.SecondChance:

                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_1_Berserk"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_1_Berserk").buffsParentEffect.TryGetValue("hpRecovered", out var value1) 
                        ? float.Parse(value1) : 0;
                    effectBonus += value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_2_Berserk"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_2_Berserk").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_4_Berserk"))
                {
                    _playerStats.maxHealthModifier += 50;
                }
                
                _playerStats.currentHealth = (int) Mathf.Round(_playerStats.currentMaxHealth * (0.05f+(effectBonus/100)));

                _playerStats.damageReduction += 30;
                _playerStats.atkModifier += 25;
                _playerStats.damageBonus += 30;

                _playerStats.speedModifier += 10;
                _playerStats.atkSpeedModifier += 10;

                break;
            
            case Enum_Buff.Thorns:
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_3_Tank"))
                {
                    _playerStats.damageReduction += (SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_3_Tank").buffsParentEffect.TryGetValue("dmgReductionBonus", out var value1) 
                        ? float.Parse(value1) : 0);
                }
                
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

                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_1_DoT"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1_DoT").buffsParentEffect.TryGetValue("atkSPD", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.atkSpeedModifier += value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_2_DoT"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_2_DoT").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3_DoT"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3_DoT").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                    effectBonus += value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_4_DoT"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_4_DoT").buffsParentEffect.TryGetValue("duration", out var value1) 
                        ? float.Parse(value1) : 0;
                    duration += base_duration * (value/100);
                }

                _playerStats.dotDamageBonus += 40 * (1+(effectBonus/100));
                
                break;

            case Enum_Buff.ManaFury:

                float dmgTaken = 0;

                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_1_Berserk"))
                {
                    dmgTaken = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3_Berserk"))
                {
                    dmgTaken = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_4_Berserk"))
                {
                    effectBonus = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_4_Berserk").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                _playerStats.damageBonus += (50 + effectBonus);
                _playerStats.atkModifier += (40 + effectBonus);
                
                _playerStats.damageTaken += (25-dmgTaken);
                
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
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3_Tank"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3_Tank").buffsParentEffect.TryGetValue("shieldStrength", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.shieldStrength -= value;
                }
                
                _playerStats.BreakShield();

                break;
            
            case Enum_Buff.SecondChance:
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_4_Berserk"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_4_Berserk").buffsParentEffect.TryGetValue("maxHPBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.maxHealthModifier -= value;
                }
                
                _playerStats.damageReduction -= 30;
                _playerStats.atkModifier -= 25;
                _playerStats.damageBonus -= 30;

                _playerStats.speedModifier -= 10;
                _playerStats.atkSpeedModifier -= 10;
                
                break;
            
            case Enum_Buff.Thorns:
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_3_Tank"))
                {
                    _playerStats.damageReduction -= (SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_3_Tank").buffsParentEffect.TryGetValue("dmgReduction", out var value) 
                        ? float.Parse(value) : 0);
                }
                
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
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_1_DoT"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1_DoT").buffsParentEffect.TryGetValue("atkSPD", out var value1) 
                        ? float.Parse(value1) : 0;
                    _playerStats.atkSpeedModifier -= value;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3_DoT"))
                {
                    var value = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3_DoT").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                    effectBonus += value;
                }
                
                _playerStats.dotDamageBonus -= 40 * (1+(effectBonus/100));
                
                break;

            case Enum_Buff.ManaFury:

                float dmgTaken = 0;

                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_1_Berserk"))
                {
                    dmgTaken = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_1_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_3_Berserk"))
                {
                    dmgTaken = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_3_Berserk").buffsParentEffect.TryGetValue("dmgTaken", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_4_Berserk"))
                {
                    effectBonus = SC_SkillManager.instance.FindChildSkillByName("ChildSkill_3_4_Berserk").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                        ? float.Parse(value1) : 0;
                }
                
                _playerStats.damageBonus -= 50 + effectBonus;
                _playerStats.atkModifier -= 40 + effectBonus;
                
                _playerStats.damageTaken -= (25 - dmgTaken);
                
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

    /// <summary>
    /// Coroutine for the poison debuff, apply damage every ticks during a certain duration.
    /// </summary>
    /// <param name="applicator"></param>
    /// <returns></returns>
    private IEnumerator PoisonDoT(SC_DebuffsBuffsComponent applicator)
    {
          
        var duration = (applicator.poisonDuration * (1 + (dotDurationBonus / 100)));
        
        poisonCurrentStacks += applicator.poisonStackByHit;
        
        yield return new WaitForSeconds(applicator.poisonTick);
        
        while (duration > 0)
        {

            print("Poison Tick");
            print("Poison Stack :" + poisonCurrentStacks);
            
            for (var i = 0; i < poisonCurrentStacks; i++)
            {
                var rawDamage = (applicator.isPlayer ? applicator._playerStats.currentATK : applicator._aiStats.currentATK) * 0.1f;
        
                var effDamage = Mathf.Round(rawDamage * (1 + (applicator.poisonDMGBonus + (applicator.isPlayer ? applicator._playerStats.dotDamageBonus : 0))/100));
                print(effDamage);
        
                var effCrit = effDamage * (1 + (applicator.dotCritDamage/100));
                
                var isCritical = Random.Range(0, 100) < applicator.dotCritRate ? true : false;
                GetComponent<IDamageable>().TakeDoTDamage(isCritical ? effCrit : effDamage, isCritical, Enum_Debuff.Poison);
            }
            
            duration -= applicator.poisonTick;
            
            // print("Duration :" + duration);
            
            yield return new WaitForSeconds(applicator.poisonTick);
        }

        poisonCurrentStacks = 0;
        currentDebuffs.Remove(Enum_Debuff.Poison);
        if(_modifierPanel != null) _modifierPanel.debuffRemoved?.Invoke(Enum_Debuff.Poison);

    }
    
    /// <summary>
    /// Coroutine for the poison debuff, apply damage every ticks during a certain duration.
    /// </summary>
    /// <param name="applicator"></param>
    /// <returns></returns>
    private IEnumerator FrozenState(SC_DebuffsBuffsComponent applicator)
    {

        var duration = (applicator.freezeDuration * 
                        (1 + (applicator.freezeDurationBonus / 100)));

        // Bonus from Skills when Frozen
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_2_Freeze"))
        {

            _aiStats.damageTaken += float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_2_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_4_Freeze"))
        {

            _aiStats.damageTaken += float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_4_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_2_Freeze"))
        {

            applicator.StartCoroutine(BuffStatTemp(Stats.ATK, float.Parse(SC_SkillManager.instance
                .FindChildSkillByName("ChildSkill_3_2_Freeze")
                .buffsParentEffect["atkBonus"]), float.Parse(SC_SkillManager.instance
                .FindChildSkillByName("ChildSkill_3_2_Freeze")
                .buffsParentEffect["atkBonusDuration"]))
            );

        }
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_4_Freeze"))
        {

            applicator.StartCoroutine(BuffStatTemp(Stats.ATKSPD, float.Parse(SC_SkillManager.instance
                    .FindChildSkillByName("ChildSkill_3_4_Freeze")
                    .buffsParentEffect["atkSpdBonus"]), float.Parse(SC_SkillManager.instance
                    .FindChildSkillByName("ChildSkill_3_4_Freeze")
                    .buffsParentEffect["atkSpdBonusDuration"]))
            );

        }

        // Freeze Effect
        if (isPlayer)
        {
            SC_PlayerController.instance.FreezeMovement(true);
            SC_PlayerController.instance.FreezeDash(true);
        }
        else
        {
            if(_aiStats.TryGetComponent(out AI_StateMachine stateMachine))
                stateMachine.TransitionToState(AI_StateMachine.EnemyState.Freeze);
        }
        
        yield return new WaitForSeconds(duration);

        // Bonus from Skills when Unfrozen
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("Fracture Glaciaire"))
        {

            print("AOOOOE");
            
            var rawDamage = MathF.Round((applicator.unfreezeAoEMV/100) * applicator._playerStats.currentATK, MidpointRounding.AwayFromZero);
            var effDamage = rawDamage * (1 + (applicator._playerStats.dotDamageBonus / 100));
            var effCrit = effDamage * (1 + (applicator.dotCritDamage / 100));

            var pos = new Vector3(transform.position.x, 0.4f, transform.position.z);
            
            var ennemiesInAoE =
                Physics.OverlapSphere(pos, applicator.unfreezeAoESize,
                    SC_ComboController.instance.layerAttackable);

            foreach (var e in ennemiesInAoE)
            {
                if (!e.TryGetComponent(out IDamageable damageable)) continue;
                var isCritical = Random.Range(0, 100) < applicator.dotCritRate ? true : false;
                damageable.TakeDamage(isCritical ? effCrit : effDamage, isCritical, gameObject);

                if(SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_1_Freeze") && 
                   Random.Range(1, 100) < float.Parse(SC_SkillManager.instance
                       .FindChildSkillByName("ChildSkill_1_1_Freeze")
                       .buffsParentEffect["freezeHitRate"]))
                {
                    e.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Freeze, applicator);
                }
                
            }
            
        }

        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_2_Freeze"))
        {

            StartCoroutine(BuffStatTemp(Stats.DMGTaken,
                float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_2_Freeze").buffsParentEffect["dmgTaken"]),
                float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_2_Freeze").buffsParentEffect["duration"])));

        }

        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("Immobilisation Glaciale"))
        {
            ApplyDebuff(Enum_Debuff.Slowdown, applicator);
        }
        
        
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_2_Freeze"))
        {

            _aiStats.damageTaken -= float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_4_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_4_Freeze"))
        {

            _aiStats.damageTaken -= float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_4_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        
        // Unfreeze Effect
        if (isPlayer)
        {
            SC_PlayerController.instance.FreezeMovement(false);
            SC_PlayerController.instance.FreezeDash(false);
        }
        else
        {
            if(_aiStats.TryGetComponent(out AI_StateMachine stateMachine))
                stateMachine.TransitionToState(AI_StateMachine.EnemyState.Patrol);
        }
        
        currentDebuffs.Remove(Enum_Debuff.Freeze);
        if(_modifierPanel != null) _modifierPanel.debuffRemoved?.Invoke(Enum_Debuff.Freeze);

    }
    
    private IEnumerator Slowdown(SC_DebuffsBuffsComponent applicator)
    {

        var duration = applicator.slowdownDuration * (1 + ((SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_3_Freeze").buffsParentEffect
            .TryGetValue("slowdownDurationBonus", out var value))
            ? float.Parse(value)
            : 0) / 100);
        

        if (applicator.isPlayer)
        {
            _aiStats.speedModifier += -30;
        }
        else
        {
            _playerStats.speedModifier += -30;
            _playerStats.atkSpeedModifier += -30;
        }
        
        yield return new WaitForSeconds(duration);
        
        if (applicator.isPlayer)
        {
            _aiStats.speedModifier -= -30;
        }
        else
        {
            _playerStats.speedModifier -= -30;
            _playerStats.atkSpeedModifier -= -30;
        }
        
        currentDebuffs.Remove(Enum_Debuff.Slowdown);
        if(_modifierPanel != null) _modifierPanel.debuffRemoved?.Invoke(Enum_Debuff.Slowdown);
        
    }

    private IEnumerator BuffStatTemp(Stats stat, float value, float duration)
    {

        switch (stat)
        {
            case Stats.DMGTaken:
                if (isPlayer) _playerStats.damageTaken += value;
                else _aiStats.damageTaken += value;
                break;
            case Stats.ATK:
                if (isPlayer) _playerStats.atkModifier += value;
                else _aiStats.atkModifier += value;
                break;
            case Stats.ATKSPD:
                if (isPlayer) _playerStats.atkSpeedModifier += value;
                break;
        }
        
        yield return new WaitForSeconds(duration);
        
        switch (stat)
        {
            case Stats.DMGTaken:
                if (isPlayer) _playerStats.damageTaken -= value;
                else _aiStats.damageTaken -= value;
                break;
            case Stats.ATK:
                if (isPlayer) _playerStats.atkModifier -= value;
                else _aiStats.atkModifier -= value;
                break;
            case Stats.ATKSPD:
                if (isPlayer) _playerStats.atkSpeedModifier -= value;
                break;
        }

    }
    
    #endregion
    
    
}
