using System;
using System.Collections;
using System.Collections.Generic;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_PlayerStats : SC_Subject, IDamageable
{

    public static SC_PlayerStats instance;

    #region Variables

    #region Health

    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    [TabGroup("Stats", "HP", SdfIconType.HeartFill, TextColor = "green"), 
     ProgressBar(0, "maxHealthEffective", r: 0, g: 1, b: 0, Height = 20), ReadOnly] 
    public float currentHealth;
    
    [TabGroup("Stats", "HP")]
    public int maxHealth = 30;
    [TabGroup("Stats", "HP")]
    public float maxHealthModifier = 0f;
    [TabGroup("Stats", "HP")] private float maxHealthEffective => maxHealth * (1 + maxHealthModifier);
    
    #endregion

    #region DEF
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "DEF",SdfIconType.ShieldFill, TextColor = "blue"), ShowInInspector, ReadOnly]
    public float currentDEF => defBase * (1 + defModifier);
    
    [TabGroup("Stats", "DEF")]
    public int defBase = 10;
    [TabGroup("Stats", "DEF")]
    public float defModifier = 0f;
    

    #endregion

    #region ATK

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "ATK",TextColor = "red"), ShowInInspector, ReadOnly]
    public float currentATK => atkBase * (1 + (atkModifier/100));
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "ATK"), ShowInInspector] public int atkBase = 5;
    [TabGroup("Stats", "ATK")] public float atkModifier;

    #endregion

    #region Crit

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "Crit",TextColor = "darkred")]
    [FoldoutGroup("Stats/Crit/Base Rate")]
    [Range(0, 100)]
    public float baseCritRate = 5;
    [PropertySpace(SpaceBefore = 5)]
    [TabGroup("Stats", "Crit",TextColor = "darkred")]
    [FoldoutGroup("Stats/Crit/Base Rate")]
    public float bonusCritRate = 0;
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "Crit",TextColor = "darkred"), ShowInInspector, ReadOnly]
    public float critRate => baseCritRate + bonusCritRate;
    
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "Crit")]
    [FoldoutGroup("Stats/Crit/Base DMG")]
    [Range(50, 1000)]
    public float baseCritDMG = 50;
    [PropertySpace(SpaceBefore = 5)]
    [TabGroup("Stats", "Crit")]
    [FoldoutGroup("Stats/Crit/Base DMG")]
    public float bonusCritDMG = 0;
    
    
    [TabGroup("Stats", "Crit",TextColor = "darkred"), ShowInInspector, ReadOnly]
    public float critDMG => (baseCritDMG + bonusCritDMG);
    
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "Crit"), ShowInInspector, ReadOnly]
    public float critValue => critDMG + (critRate * 2);

    #endregion

    #region DMG

    [TabGroup("Stats", "DMG"), PropertySpace(SpaceAfter = 5f)]
    public float damageReduction = 0;
    
    #region Taken
    
    [TabGroup("Stats", "DMG",TextColor = "darkred")]
    [FoldoutGroup("Stats/DMG/Taken")]
    public float damageTaken = 0;
    
    [TabGroup("Stats", "DMG")]
    [FoldoutGroup("Stats/DMG/Taken")]
    public float dotDamageTaken = 0;

    #endregion
    
    #region Bonus

    [TabGroup("Stats", "DMG")]
    [FoldoutGroup("Stats/DMG/Bonus")]
    public float damageBonus = 0;

    [TabGroup("Stats", "DMG")]
    [FoldoutGroup("Stats/DMG/Bonus")]
    public float mhDamageBonus = 0;
    
    [TabGroup("Stats", "DMG")]
    [FoldoutGroup("Stats/DMG/Bonus")]
    public float aoeDamageBonus = 0;
    
    [TabGroup("Stats", "DMG")]
    [FoldoutGroup("Stats/DMG/Bonus")]
    public float projectileDamageBonus = 0;
    
    [TabGroup("Stats", "DMG")]
    [FoldoutGroup("Stats/DMG/Bonus")]
    public float dotDamageBonus = 0;

    #endregion
    
    #endregion

    #region Debuffs

    [TabGroup("Stats", "Hit Rates"), MaxValue(100f), MinValue(0f)]
    public float poisonHitRate = 0f;
    
    [TabGroup("Stats", "Hit Rates"), MaxValue(4), MinValue(0)]
    public int poisonStackByHit = 1;
    [TabGroup("Stats", "Hit Rates"), MaxValue(4), MinValue(3)]
    public int poisonMaxStack = 3;
    [TabGroup("Stats", "Hit Rates"), MaxValue("poisonMaxStack"), MinValue(0)]
    public int poisonCurrentStacks = 3;
    [TabGroup("Stats", "Hit Rates"), MaxValue(2f), MinValue(0.25f)]
    public float poisonTick = 2f;
    [TabGroup("Stats", "Hit Rates"), MaxValue(120f), MinValue(0.25f)]
    public float poisonDuration = 1;
    
    
    #endregion
    
    #region SPD

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector]
    public float currentSpeed => baseSpeed * (1 + speedModifier);
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD"), SerializeField] public int baseSpeed = 7;
    [TabGroup("Stats", "SPD")] public float speedModifier;

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector]
    public float currentATKSpeed => baseATKSpeed * (1 + atkSpeedModifier);
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD"), SerializeField] public int baseATKSpeed = 7;
    [TabGroup("Stats", "SPD")] public float atkSpeedModifier;
    
    #endregion

    #region Others

    [TabGroup("Stats", "Others")]
    public float healingBonus = 0f;

    [TabGroup("Stats", "Others")]
    public float dishesEffectBonus = 0f;
    
    #endregion
    
    #region Status
    
    [TabGroup("Status", "Debuffs")]
    public List<Enum_Buff> currentBuffs;
    [TabGroup("Status", "Debuffs")]
    public List<Enum_Debuff> currentDebuffs;
    
    [TabGroup("Status", "Debugs")]
    public bool isGod;
    
    
    #endregion
    
    private SC_PlayerController _controller;
    private SC_ComboController _comboController;

    public SC_StatsDebug statsDebug = null;

    #endregion


    #region Init

    /// <summary>
    /// Set this code into a Singleton
    /// Get the PlayerController
    /// </summary>
    void Awake()
    {
        instance = this;
        
        if(!TryGetComponent(out _controller)) return;
        if(!TryGetComponent(out _comboController)) return;
    }
    
    /// <summary>
    /// At the start, set currentHP to the maxHP
    /// </summary>
    private void Start()
    {
        currentHealth = maxHealthEffective;
        NotifyObservers(currentHealth, maxHealthEffective);
    }
    
    #endregion

    #region Status
    
    
    public void ApplyBuffToSelf(Enum_Buff newBuff)
    {

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

                atkModifier += 1000;
                bonusCritDMG += 200;
                bonusCritRate += 95;
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newBuff), newBuff, null);
        }
        
        currentBuffs.Add(newBuff);

        if (statsDebug != null)
        {
            statsDebug.RefreshStats();
        }
        
    }
    
    public void RemoveBuffFromSelf(Enum_Buff newBuff)
    {

        if(!currentBuffs.Contains(newBuff)) return;
        
        switch (newBuff)
        {
            case Enum_Buff.Armor:
                break;
            case Enum_Buff.SecondChance:
                break;
            case Enum_Buff.Thorns:
                break;
            case Enum_Buff.God:
                
                atkModifier -= 1000;
                bonusCritDMG -= 200;
                bonusCritRate -= 95;
                
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newBuff), newBuff, null);
        }

        currentBuffs.Remove(newBuff);
        
        if (statsDebug != null)
        {
            statsDebug.RefreshStats();
        }
        
    }
    
    /// <summary>
    /// Apply a debuff to self with a certain type, a certain activation cooldown and a duration.
    /// </summary>
    /// <param name="newDebuff">Debuff to apply</param>
    public void ApplyDebuffToSelf(Enum_Debuff newDebuff)
    {
        

        switch (newDebuff)
        {
            
            case Enum_Debuff.Poison:
                
                if (currentDebuffs.Contains(Enum_Debuff.Poison))
                {
                    
                }
                else
                {
                    StartCoroutine(PoisonDoT((maxHealthEffective * 0.1f), poisonTick, poisonDuration));
                }
                
                break;
            
            case Enum_Debuff.Bleed:
                break;
            case Enum_Debuff.Burn:
                break;
            case Enum_Debuff.Freeze:
                break;
            case Enum_Debuff.Slowdown:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newDebuff), newDebuff, null);
        }
        
        currentDebuffs.Add(newDebuff);
        
    }
    
    /// <summary>
    /// Coroutine for the poison debuff, apply damage every ticks during a certain duration.
    /// </summary>
    /// <param name="incomingDamage">Damage before Damage Reduction</param>
    /// <param name="tick">Cooldown between to damage</param>
    /// <param name="duration">Duration before poison expire</param>
    /// <returns></returns>
    private IEnumerator PoisonDoT(float incomingDamage, float tick, float duration)
    {
        var finalDamage = incomingDamage; // TO-DO : Place Defense here
        
        while (duration != 0)
        {
            for (int i = 0; i < poisonMaxStack; i++)
            {
                TakeDamage(finalDamage);
            }
            duration -= tick;
            
            NotifyObservers(currentHealth, maxHealth);
            
            yield return new WaitForSeconds(tick);
        }

    }
    
    #endregion

    /// <summary>
    /// Apply Damage to the Player.
    /// </summary>
    /// <param name="rawDamage">Damage before Damage Reduction</param>
    public void TakeDamage(float rawDamage)
    {
        
        if(_controller.isDashing || isGod) return;
            
        var finalDamage = rawDamage - currentDEF;
        
        currentHealth = currentHealth - finalDamage < 0 ? 0 : currentHealth - finalDamage;
        if (DeathCheck())
        {
            Death();
        }
        
        NotifyObservers(currentHealth, maxHealthEffective);
    }

    public void TakeDamage(float rawDamage, WeaponType weaponType, bool isCrit){}

    /// <summary>
    /// Heal the player by a certain amount
    /// </summary>
    /// <param name="healAmount"></param>
    public void Heal(int healAmount)
    {
        // Check if the heal don't exceed the Max HP limit, if yes, set to max hp, else increment currentHP by healAmount.
        currentHealth = currentHealth + healAmount > maxHealth ? maxHealth : currentHealth + healAmount;
        
        NotifyObservers(currentHealth, maxHealthEffective);
    }

    public bool DeathCheck()
    {
        return currentHealth <= 0;
    }
    
    public void Death()
    {
        // _controller.canMove = false;
        // _comboController.canAttack = false;
        
        SC_GameManager.instance.ChangeState(GameState.DEFEAT);
    }

    public void ResetModifiers()
    {
        maxHealthModifier = 0;
        speedModifier = 0;
        atkModifier = 0;
        defModifier = 0;
        
        bonusCritDMG = 0;
        bonusCritRate = 0;
        
        currentHealth = maxHealthEffective;
        currentDebuffs.Clear();
    }
    
    /// <summary>
    /// Detect Hurtbox collision, set up taking damage.
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter(Collider col)
    {
        
        if (!col.CompareTag("HurtBox_AI")) return;
        
        if(!col.transform.parent.parent.TryGetComponent(out SC_AIStats aiStats)) return;
        
        
        var aiCurrentAtk = aiStats.currentATK;
        var aiCurrentMV = aiStats.moveValues[aiStats.moveValueIndex];

        var rawDamage = Mathf.Round(aiCurrentMV * aiCurrentAtk);
        
        TakeDamage(rawDamage);
          
    }
    
}
