using System;
using System.Collections;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_PlayerStats : SC_Subject, IDamageable
{

    public static SC_PlayerStats instance;

    #region Variables

    #region Health

    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    [TabGroup("Stats", "HP", SdfIconType.HeartFill, TextColor = "green"), 
     ProgressBar(0, "currentMaxHealth", r: 0, g: 1, b: 0, Height = 20), ReadOnly] 
    public float currentHealth;
    
    [TabGroup("Stats", "HP")]
    public int maxHealth = 30;
    [TabGroup("Stats", "HP")]
    public float maxHealthModifier = 0f;
    [TabGroup("Stats", "HP")] public float currentMaxHealth => maxHealth * (1 + (maxHealthModifier/100));
    
    #endregion

    #region DEF
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "DEF",SdfIconType.ShieldFill, TextColor = "blue"), ShowInInspector, ReadOnly]
    public float currentDEF => defBase * (1 + defModifier);
    
    [TabGroup("Stats", "DEF")]
    public int defBase = 10;
    [TabGroup("Stats", "DEF")]
    public float defModifier = 0f;
    
    [TabGroup("Stats", "DEF")]
    [Tooltip("DEF Stat used to reduce damage taken"), ShowInInspector, ReadOnly]
    public float defMultiplier => (100 / (100 + currentDEF));

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

    #region Hit Rates

    [TabGroup("Stats", "Hit Rates"), MaxValue(100f), MinValue(0f)]
    public float poisonHitRate = 0f;
    
    #endregion
    
    #region SPD

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector]
    public float currentSpeed => baseSpeed * (1 + (speedModifier/100));
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD"), SerializeField] public int baseSpeed = 7;
    [TabGroup("Stats", "SPD")] public float speedModifier;

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector]
    public float currentATKSpeed => baseATKSpeed * (1 + (atkSpeedModifier/100));
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD"), SerializeField] public int baseATKSpeed = 7;
    [TabGroup("Stats", "SPD")] public float atkSpeedModifier;
    
    #endregion

    #region Others
    
    [TabGroup("Status", "Debugs")]
    public bool isGod;
    
    [TabGroup("Stats", "Others")]
    public float healingBonus = 0f;

    [TabGroup("Stats", "Others")]
    public float dishesEffectBonus = 0f;

    #region Shield

    [TabGroup("Stats", "Shield")]
    public float shieldMaxHP = 0f;
    [TabGroup("Stats", "Shield")]
    public float shieldCurrentHP = 0f;
    [TabGroup("Stats", "Shield")]
    public float shieldStrength = 0f;

    #endregion

    #region Mana Overload

    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
    public int manaOverloadStack = 0;
    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
    public int manaOverloadMaxStack = 2;
    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
    public float manaOverloadDamageBoost = 20;
    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
    public float manaOverloadTick = 0.5f;
    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
    public float manaOverloadDuration = 5f;
    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
    public float manaOverloadDamage = 5f;
    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload"),ShowInInspector]
    private bool inManaOverload;
    private Coroutine manaOverload;

    #endregion

    #region Mana Fury

    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Fury")]
    public float manaFuryMaxHPGate = 25;
    [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Fury")]
    public bool inManaFury = false;

    #endregion

    #region Events

    public SO_Event onDeathEvent;
    public SO_Event onManaFuryEnableEvent;
    public SO_Event onManaFuryDisableEvent;

    #endregion
    
    #endregion


    public static Action<float, float> onHealthChange;
    public static Action<float, float> onShieldHPChange;
    
    private SC_PlayerController _controller;
    private SC_ComboController _comboController;
    [HideInInspector] public SC_DebuffsBuffsComponent debuffsBuffsComponent;

    public SC_StatsDebug statsDebug = null;

    #endregion

    #region Init

    /// <summary>
    /// Set this code into a Singleton
    /// Get the PlayerController
    /// </summary>
    private void Awake()
    {
        instance = this;
        
        if(!TryGetComponent(out _controller)) return;
        if(!TryGetComponent(out _comboController)) return;
        if(!TryGetComponent(out debuffsBuffsComponent)) return;
    }

    /// <summary>
    /// At the start, set currentHP to the maxHP
    /// </summary>
    private void Start()
    {
        currentHealth = currentMaxHealth;
        onHealthChange?.Invoke(currentHealth, currentMaxHealth);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad9)) TakeDamage(5, true);
        if(Input.GetKeyDown(KeyCode.Keypad8)) Heal(10);
    }


    private void OnEnable()
    {
        SC_AIStats.onDeath += onEnemyKilled;
    }

    private void OnDisable()
    {
        SC_AIStats.onDeath -= onEnemyKilled;
    }

    #endregion

    /// <summary>
    /// Apply Damage to the Player.
    /// </summary>
    /// <param name="rawDamage">Damage before Damage Reduction</param>
    /// <param name="trueDamage"></param>
    public void TakeDamage(float rawDamage, bool trueDamage = false)
    {
        
        if(_controller.isDashing || isGod) return;

        var damageTakenMultiplier = (1 + (damageTaken/100));
            
        var damageReductionMultiplier = (1 - (damageReduction/100));
            
        var finalDamage = !trueDamage ? Mathf.Round((rawDamage * defMultiplier) * damageTakenMultiplier * damageReductionMultiplier) : rawDamage;
        
        if (shieldCurrentHP > 0)
        {
            
            shieldCurrentHP = shieldCurrentHP - finalDamage < 0 ? 0 : shieldCurrentHP - finalDamage;
            
            onShieldHPChange?.Invoke(shieldCurrentHP, shieldMaxHP);
            
        }
        else
        {
            
            currentHealth = currentHealth - finalDamage < 0 ? 0 : currentHealth - finalDamage;

            HealthCheck();
            
            if (DeathCheck())
            {
                Death();
            }
            
            onHealthChange?.Invoke(currentHealth, currentMaxHealth);
            
        }
    }

    public void TakeDamage(float rawDamage, WeaponType weaponType, bool isCrit){}
    public void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType)
    {
        if(_controller.isDashing || isGod) return;
            
        var damageTakenMultiplier = (1 * (1 + (dotDamageTaken/100)));
        
        var damageReductionMultiplier = (1 * (1 - (damageReduction/100)));
        
        var finalDamage = (rawDamage * defMultiplier) * damageTakenMultiplier * damageReductionMultiplier;
        
        currentHealth = currentHealth - finalDamage < 0 ? 0 : currentHealth - finalDamage;
        
        HealthCheck();
        
        if (DeathCheck())
        {
            Death();
        }
        
        onHealthChange?.Invoke(currentHealth, currentMaxHealth);
    }

    /// <summary>
    /// Heal the player by a certain amount
    /// </summary>
    /// <param name="healAmount"></param>
    private void Heal(float healAmount)
    {
        // Check if the heal don't exceed the Max HP limit, if yes, set to max hp, else increment currentHP by healAmount.
        currentHealth = currentHealth + healAmount > maxHealth ? maxHealth : currentHealth + healAmount;

        HealthCheck();
        
        onHealthChange?.Invoke(currentHealth, currentMaxHealth);
    }

    
    private bool DeathCheck()
    {
        return currentHealth <= 0;
    }

    public void HealthCheck()
    {
        if (currentHealth < (currentMaxHealth * (manaFuryMaxHPGate / 100)) && !inManaFury)
        {
            inManaFury = true;
            onManaFuryEnableEvent?.RaiseEvent();
        }
        else if (currentHealth > (currentMaxHealth * (manaFuryMaxHPGate / 100)) && inManaFury)
        {
            inManaFury = false;
            onManaFuryDisableEvent?.RaiseEvent();
        }
    }

    public void CreateShield(float shieldValue)
    {
        
        if (SC_SkillManager.instance.CheckHasSkillByName("Protection Épineuse"))
        {

            damageReduction += 15;
            debuffsBuffsComponent.ApplyBuff(Enum_Buff.Thorns, 0);

        }
        
        shieldMaxHP = (shieldValue * (1 + (shieldStrength/100)));
        shieldCurrentHP = shieldMaxHP;
        
        onShieldHPChange?.Invoke(shieldCurrentHP, shieldMaxHP);
        
    }
    
    public void BreakShield()
    {

        if (SC_SkillManager.instance.CheckHasSkillByName("Protection Épineuse"))
        {

            damageReduction -= 15;
            debuffsBuffsComponent.RemoveBuff(Enum_Buff.Thorns);
            
        }
        
        shieldMaxHP = 0;
        shieldCurrentHP = 0;
        
        onShieldHPChange?.Invoke(shieldCurrentHP, shieldMaxHP);

    }
    
    public void Death()
    {
        onDeathEvent.RaiseEvent();

        if (SC_SkillManager.instance.CheckHasSkillByName("Souffle de Résurrection"))
        {
            SC_SkillManager.instance.allEquippedSkills.Remove(SC_SkillManager.instance.FindSkillByName("Souffle de Résurrection"));
            return;
        }

        SC_GameManager.instance.ChangeState(GameState.DEFEAT);
    }
    
    private void onEnemyKilled()
    {

        print("Enemy Killed");
        
        if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_3_Berserk") && debuffsBuffsComponent.CheckHasBuff(Enum_Buff.SecondChance))
        {

            if(Random.Range(0, 2) == 1) return;
            
            Heal(Mathf.Round(currentMaxHealth * (float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_3_Berserk").buffsParentEffect["healAmount"])/100)));

        }

        if (SC_SkillManager.instance.CheckHasSkillByName("Surcharge d'Essence"))
        {
            GainManaOverloadStack();

            if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_2_Berserk") && manaOverloadStack >= 2)
            {
                
                Heal(Mathf.Round(currentMaxHealth * (float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_2_Berserk").buffsParentEffect["healAmount"])/100)));
                
            }
            
        }
        
        
    }

    private void GainManaOverloadStack()
    {
        
        if(manaOverloadStack >= manaOverloadMaxStack) return;

        manaOverloadStack++;
        
        if (inManaOverload && manaOverload != null)
        {
            StopCoroutine(manaOverload);
            damageBonus -= (manaOverloadDamageBoost * (manaOverloadStack - 1));
            manaOverload = null;
        }
        
        manaOverload = StartCoroutine(ManaOverloadBoost());

    }

    private IEnumerator ManaOverloadBoost()
    {
        inManaOverload = true;
        
        var duration = manaOverloadDuration;
        var tempStacks = manaOverloadStack;
        
        damageBonus += (manaOverloadDamageBoost * tempStacks);

        while (duration > 0)
        {
            TakeDamage(currentMaxHealth * (manaOverloadDamage/100));
            
            yield return new WaitForSeconds(manaOverloadTick);
            duration -= manaOverloadTick;
        }
        
        damageBonus -= (manaOverloadDamageBoost * tempStacks);
        inManaOverload = false;
    }
    
    public void ResetModifiers()
    {
        maxHealthModifier = 0;
        speedModifier = 0;
        atkModifier = 0;
        defModifier = 0;
        
        bonusCritDMG = 0;
        bonusCritRate = 0;
        
        currentHealth = currentMaxHealth;
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
