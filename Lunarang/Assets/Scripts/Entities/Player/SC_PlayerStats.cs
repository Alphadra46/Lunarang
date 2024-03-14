using System;
using System.Collections;
using Entities;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_PlayerStats : SC_EntityBase, IDamageable
{

    public static SC_PlayerStats instance;

    #region Variables

    #region Others
    
    [TabGroup("Status", "Debugs")]
    public bool isGod;
    
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

        currentStats = baseStats;
        
        if(!TryGetComponent(out _controller)) return;
        if(!TryGetComponent(out _comboController)) return;
        if(!TryGetComponent(out debuffsBuffsComponent)) return;
    }

    /// <summary>
    /// At the start, set currentHP to the maxHP
    /// </summary>
    private void Start()
    {
        currentStats.currentHealth = currentStats.currentMaxHealth;
        onHealthChange?.Invoke(currentStats.currentHealth, currentStats.currentMaxHealth);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad9)) TakeDamage(5, false, null, false);
        if(Input.GetKeyDown(KeyCode.Keypad8)) Heal(10);
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {

            SC_SkillManager.instance.AddSkillsToSkillsList(SC_SkillManager.instance.FindSkillByName("Glouton"));

        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            if(SC_SkillManager.instance.CheckHasSkillByName("Glouton"))
                SC_SkillManager.instance.AddLevelToLunarSkill("Glouton", 1);

        }
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
    /// <param name="isCrit"></param>
    /// <param name="attacker"></param>
    /// <param name="trueDamage"></param>
    public void TakeDamage(float rawDamage, bool isCrit, GameObject attacker, bool trueDamage = false)
    {
        
        if(_controller.isDashing || isGod) return;

        var damageTakenMultiplier = (1 + (currentStats.damageTaken/100));
        var damageReductionMultiplier = (1 - ((currentStats.damageReduction/100) + (currentStats.steelBodyDMGReductionModifier/100)));
        
        if (currentStats.shieldCurrentHP > 0)
        {
            var shieldDamage = Mathf.Round(rawDamage * damageTakenMultiplier * damageReductionMultiplier);
            
            currentStats.shieldCurrentHP = currentStats.shieldCurrentHP - shieldDamage < 0 ? 0 : currentStats.shieldCurrentHP - shieldDamage;
            
            onShieldHPChange?.Invoke(currentStats.shieldCurrentHP, currentStats.shieldMaxHP);
            
        }
        else
        {
            var finalDamage = !trueDamage ? Mathf.Round((rawDamage * currentStats.defMultiplier) * damageTakenMultiplier * damageReductionMultiplier) : rawDamage;
            
            currentStats.currentHealth = currentStats.currentHealth - finalDamage < 0 ? 0 : currentStats.currentHealth - finalDamage;

            HealthCheck();
            
            if (DeathCheck())
            {
                Death();
            }
            
            onHealthChange?.Invoke(currentStats.currentHealth, currentStats.currentMaxHealth);
            
        }

        #region Thorns
        
        if (!SC_SkillManager.instance.CheckHasSkillByName("Protection Épineuse")) return;
        
        if(currentStats.shieldCurrentHP <= 0) return;
        
        const float thornsMV = 0.1f;
        var rawDMG = thornsMV * currentStats.currentATK;
        var effDMG = rawDMG * (1 +
                               (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_2_Tank")
                                && SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_2_Tank")
                                    .buffsParentEffect.TryGetValue("thornsDMGBonus", out var value1) ?
                                   float.Parse(value1)/100
                                   : 0));
            
        attacker.GetComponent<IDamageable>().TakeDamage(MathF.Round(effDMG, MidpointRounding.AwayFromZero), false, gameObject);
        
        if (!SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_4_Tank") || !SC_SkillManager.instance
                .FindChildSkillByName("ChildSkill_1_4_Tank")
                .buffsParentEffect.TryGetValue("freezeHitRate", out var freezeHitRate)) return;
        
        if(Random.Range(1, 100) < float.Parse(freezeHitRate))
        {
            attacker.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Freeze, GetComponent<SC_DebuffsBuffsComponent>());
        }
        
        #endregion
        
    }
    
    public void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType)
    {
        if(_controller.isDashing || isGod) return;
            
        var damageTakenMultiplier = (1 * (1 + (currentStats.dotDamageTaken/100)));
        
        var damageReductionMultiplier = (1 * (1 - (currentStats.damageReduction/100)));
        
        var finalDamage = (rawDamage * currentStats.defMultiplier) * damageTakenMultiplier * damageReductionMultiplier;
        
        currentStats.currentHealth = currentStats.currentHealth - finalDamage < 0 ? 0 : currentStats.currentHealth - finalDamage;
        
        HealthCheck();
        
        if (DeathCheck())
        {
            Death();
        }
        
        onHealthChange?.Invoke(currentStats.currentHealth, currentStats.currentMaxHealth);
    }

    /// <summary>
    /// Heal the player by a certain amount
    /// </summary>
    /// <param name="healAmount"></param>
    private void Heal(float healAmount)
    {
        // Check if the heal don't exceed the Max HP limit, if yes, set to max hp, else increment currentHP by healAmount.
        currentStats.currentHealth = currentStats.currentHealth + healAmount > currentStats.maxHealth ? currentStats.maxHealth : currentStats.currentHealth + healAmount;

        HealthCheck();
        
        onHealthChange?.Invoke(currentStats.currentHealth, currentStats.currentMaxHealth);
    }

    
    private bool DeathCheck()
    {
        return currentStats.currentHealth <= 0;
    }

    public void HealthCheck()
    {
        
        if(SC_SkillManager.instance.CheckHasSkillByName("Furie de l'Essence")) {
            if (currentStats.currentHealth < (currentStats.currentMaxHealth * (currentStats.manaFuryMaxHPGate / 100)) && !currentStats.inManaFury)
            {
                currentStats.inManaFury = true;
                onManaFuryEnableEvent?.RaiseEvent();
            }
            else if (currentStats.currentHealth > (currentStats.currentMaxHealth * (currentStats.manaFuryMaxHPGate / 100)) && currentStats.inManaFury)
            {
                currentStats.inManaFury = false;
                onManaFuryDisableEvent?.RaiseEvent();
            }
        }

        if (SC_SkillManager.instance.CheckHasSkillByName("Corps d'Acier"))
        {
            var stackGain = SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_3_Tank") ? float.Parse(SC_SkillManager.instance
                .FindChildSkillByName("ChildSkill_2_3_Tank")
                .buffsParentEffect["stackGain"]) : 1;
            
            var maxStacks = SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_2_Tank") ? int.Parse(SC_SkillManager.instance
                .FindChildSkillByName("ChildSkill_2_2_Tank")
                .buffsParentEffect["maxStacks"]) : 10;
            
            currentStats.steelBodyStackCount = (int)(currentStats.steelBodyStackCount >= maxStacks ? maxStacks : (Mathf.FloorToInt((100 - ((currentStats.currentHealth / currentStats.currentMaxHealth) * 100)) / currentStats.steelBodyHPPercentNeeded) * stackGain));

            
            
            currentStats.steelBodyDEFModifier = currentStats.steelBodyStackCount >= maxStacks ? (currentStats.steelBodyDEFPerStack * maxStacks) : (currentStats.steelBodyDEFPerStack * currentStats.steelBodyStackCount);

            if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_1_Tank"))
                currentStats.steelBodyShieldStrengthModifier = float.Parse(SC_SkillManager.instance
                                                      .FindChildSkillByName("ChildSkill_2_1_Tank")
                                                      .buffsParentEffect["shieldStrengthPerStack"]) * currentStats.steelBodyStackCount;
            
            if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_4_Tank"))
                currentStats.steelBodyDMGReductionModifier = float.Parse(SC_SkillManager.instance
                    .FindChildSkillByName("ChildSkill_2_4_Tank")
                    .buffsParentEffect["damageReductionPerStack"]) * currentStats.steelBodyStackCount;

        }
        
    }

    
    public void CreateShield(float shieldValue)
    {
        if(currentStats.shieldCurrentHP > 0 && currentStats.shieldMaxHP > 0) BreakShield();
        
        if (SC_SkillManager.instance.CheckHasSkillByName("Protection Épineuse"))
        {

            currentStats.damageReduction += (15 + 
                                             (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_1_Tank") 
                                              && SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_1_Tank").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                                                 ? float.Parse(value1) : 0));
            debuffsBuffsComponent.ApplyBuff(Enum_Buff.Thorns, 0);
            
        }
        
        currentStats.shieldMaxHP = shieldValue * (1 + (currentStats.shieldStrength/100) + (currentStats.steelBodyShieldStrengthModifier/100));
        currentStats.shieldCurrentHP = currentStats.shieldMaxHP;
        
        onShieldHPChange?.Invoke(currentStats.shieldCurrentHP, currentStats.shieldMaxHP);
        
    }

    [TabGroup("Stats", "Shield"), Button]
    public void EditorShield()
    {
        
        CreateShield(0.2f * currentStats.currentMaxHealth);
        
    }
    
    public void BreakShield()
    {

        if (SC_SkillManager.instance.CheckHasSkillByName("Protection Épineuse"))
        {

            currentStats.damageReduction -= (15 + 
                                             (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_1_Tank") 
                                              && SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_1_Tank").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
                                                 ? float.Parse(value1) : 0));
            debuffsBuffsComponent.RemoveBuff(Enum_Buff.Thorns);
            
        }
        
        currentStats.shieldMaxHP = 0;
        currentStats.shieldCurrentHP = 0;
        
        onShieldHPChange?.Invoke(currentStats.shieldCurrentHP, currentStats.shieldMaxHP);

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
            
            Heal(Mathf.Round(currentStats.currentMaxHealth * (float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_3_Berserk").buffsParentEffect["healAmount"])/100)));

        }

        if (SC_SkillManager.instance.CheckHasSkillByName("Surcharge d'Essence"))
        {
            GainManaOverloadStack();

            if (SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_2_Berserk") && currentStats.manaOverloadStack >= 2)
            {
                
                Heal(Mathf.Round(currentStats.currentMaxHealth * (float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_2_Berserk").buffsParentEffect["healAmount"])/100)));
                
            }
            
        }
        
        
    }

    private void GainManaOverloadStack()
    {
        
        if(currentStats.manaOverloadStack >= currentStats.manaOverloadMaxStack) return;

        currentStats.manaOverloadStack++;
        
        if (currentStats.inManaOverload && currentStats.manaOverload != null)
        {
            StopCoroutine(currentStats.manaOverload);
            currentStats.damageBonus -= (currentStats.manaOverloadDamageBoost * (currentStats.manaOverloadStack - 1));
            currentStats.manaOverload = null;
        }
        
        currentStats.manaOverload = StartCoroutine(ManaOverloadBoost());

    }

    private IEnumerator ManaOverloadBoost()
    {
        currentStats.inManaOverload = true;
        
        var duration = currentStats.manaOverloadDuration;
        var tempStacks = currentStats.manaOverloadStack;
        
        currentStats.damageBonus += (currentStats.manaOverloadDamageBoost * tempStacks);

        while (duration > 0)
        {
            TakeDamage(currentStats.currentMaxHealth * (currentStats.manaOverloadDamage/100), false, gameObject);
            
            yield return new WaitForSeconds(currentStats.manaOverloadTick);
            duration -= currentStats.manaOverloadTick;
        }
        
        currentStats.damageBonus -= (currentStats.manaOverloadDamageBoost * tempStacks);
        currentStats.inManaOverload = false;
    }
    
    public void ResetModifiers()
    {
        currentStats.maxHealthModifier = 0;
        currentStats.speedModifier = 0;
        currentStats.atkModifier = 0;
        currentStats.defModifier = 0;
        
        currentStats.bonusCritDMG = 0;
        currentStats.bonusCritRate = 0;
        
        currentStats.currentHealth = currentStats.currentMaxHealth;
    }
    
    
    /// <summary>
    /// Detect Hurtbox collision, set up taking damage.
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter(Collider col)
    {
        
        if (!col.CompareTag("HurtBox_AI")) return;
        
        if(!col.transform.parent.parent.TryGetComponent(out SC_AIStats aiStats)) return;
        
        var aiCurrentAtk = aiStats.currentStats.currentATK;
        var aiCurrentMV = aiStats.moveValues[aiStats.moveValueIndex];

        var rawDamage = Mathf.Round(aiCurrentMV * aiCurrentAtk);
        
        TakeDamage(rawDamage, false, aiStats.gameObject);

    }
    
}
