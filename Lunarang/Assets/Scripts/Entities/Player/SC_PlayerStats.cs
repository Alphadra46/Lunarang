using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_PlayerStats : SC_EntityBase, IDamageable
{

    public static SC_PlayerStats instance;

    #region Variables

    #region Others
    
    [TabGroup("Status", "Debugs")]
    public bool isGod;

    public int resurectionCounter = 1;
    
    #region Events

    public SO_Event onDeathEvent;
    public SO_Event onManaFuryEnableEvent;
    public SO_Event onManaFuryDisableEvent;

    #endregion
    
    #endregion
    
    public static Action<float, float> onHealthInit;
    public static Action<float, float> onHealthChange;
    public static Action<float, float> onShieldHPChange;
    public static Action onUpdatedStats;

    public List<SkinnedMeshRenderer> _meshRenderer;
    private SC_PlayerController _controller;
    private SC_ComboController _comboController;
    
    [HideInInspector] public SC_DebuffsBuffsComponent debuffsBuffsComponent;
    private SC_SFXPlayerComponent sfxPlayer;

    public SC_StatsDebug statsDebug = null;

    private Coroutine damageTakenCoroutine = null;

    private bool isInvincible = false;
    
    private bool isDeath = false;

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
        if(!TryGetComponent(out sfxPlayer)) return;

    }

    /// <summary>
    /// At the start, set currentHP to the maxHP
    /// </summary>
    private void Start()
    {
        currentStats.currentHealth = currentStats.currentMaxHealth;
        onHealthInit?.Invoke(currentStats.currentHealth, currentStats.currentMaxHealth);
        if (SceneManager.GetActiveScene().buildIndex == 1)
            isInvincible = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad9)) TakeDamage(5, false, null, true);
        if(Input.GetKeyDown(KeyCode.Keypad8)) Heal(10);
    }


    private void OnEnable()
    {
        SC_AIStats.onDeath += onEnemyKilled;
        onUpdatedStats += UpdateStats;
        SC_FeedbackRoomStatusUI.roomCleared += ClearRoom;
    }

    private void OnDisable()
    {
        SC_AIStats.onDeath -= onEnemyKilled;
        onUpdatedStats -= UpdateStats;
        SC_FeedbackRoomStatusUI.roomCleared -= ClearRoom;
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
        if (isInvincible) return;
        if (isDeath) return;
        
        if (damageTakenCoroutine != null) StopCoroutine(damageTakenCoroutine);
        damageTakenCoroutine = StartCoroutine(DamageTaken());
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

            var clips = new List<string>() { "SD_LouDegat_1", "SD_LouDegat_2" };
            sfxPlayer.PlayRandomClip(clips);

        }
        SC_CameraShake.instance.StopAllCoroutines();
        StartCoroutine(SC_CameraShake.instance.ShakeCamera(4f, 1, 0.3f));

        #region Thorns
        
        if (!SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Peau d'Épines")) return;
        
        if(currentStats.shieldCurrentHP <= 0) return;
        
        const float thornsMV = 0.1f;
        var rawDMG = thornsMV * currentStats.currentATK;
        var effDMG = rawDMG * (1 +
                               (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Parade Brutale")
                                && SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Parade Brutale")
                                    .buffsParentEffect.TryGetValue("thornsDMGBonus", out var value1) ?
                                   float.Parse(value1)/100
                                   : 0));
            
        attacker.GetComponent<IDamageable>().TakeDamage(MathF.Round(effDMG, MidpointRounding.AwayFromZero), false, gameObject);
        
        if (!SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Riposte Hivernale") || !SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Riposte Hivernale")
                .buffsParentEffect.TryGetValue("freezeHitRate", out var freezeHitRate)) return;
        
        if(Random.Range(1, 100) < float.Parse(freezeHitRate))
        {
            attacker.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Freeze, GetComponent<SC_DebuffsBuffsComponent>());
        }
        
        #endregion
        
    }

    private IEnumerator DamageTaken()
    {
        
        foreach (var meshRenderer in _meshRenderer)
        {
            var materials = meshRenderer.materials;
        
            foreach (var material in materials)
            {
                material.SetFloat("_DamageAmount", 0.7f);
            }
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var meshRenderer in _meshRenderer)
        {
            var materials = meshRenderer.materials;
            foreach (var material in materials)
            {
                material.SetFloat("_DamageAmount", 0f);
            }
        }

        damageTakenCoroutine = null;
    }
    
    public void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType)
    {
        if (isInvincible) return;
        if (isDeath) return;
        
        if (damageTakenCoroutine != null) StopCoroutine(damageTakenCoroutine);
        damageTakenCoroutine = StartCoroutine(DamageTaken());
        
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
    public void Heal(float healAmount)
    {
        if(isDeath) return;
        
        // Check if the heal don't exceed the Max HP limit, if yes, set to max hp, else increment currentHP by healAmount.
        currentStats.currentHealth = currentStats.currentHealth + healAmount > currentStats.currentMaxHealth ? currentStats.currentMaxHealth : currentStats.currentHealth + healAmount;

        HealthCheck();
        
        onHealthChange?.Invoke(currentStats.currentHealth, currentStats.currentMaxHealth);
    }
    
    private bool DeathCheck()
    {
        return currentStats.currentHealth <= 0;
    }

    public void HealthCheck()
    {
        
        if(SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Furie Rocheuse")) {
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

        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Soutien Naturel"))
        {
            var stackGain = SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Adaptation Primale") ? float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Adaptation Primale")
                .buffsParentEffect["stackGain"]) : 1;
            
            var maxStacks = SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Bajoues de Sablille") ? int.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Bajoues de Sablille")
                .buffsParentEffect["maxStacks"]) : 10;
            
            currentStats.steelBodyStackCount = (int)(currentStats.steelBodyStackCount >= maxStacks ? maxStacks : (Mathf.FloorToInt((100 - ((currentStats.currentHealth / currentStats.currentMaxHealth) * 100)) / currentStats.steelBodyHPPercentNeeded) * stackGain));

            
            
            currentStats.steelBodyDEFModifier = currentStats.steelBodyStackCount >= maxStacks ? (currentStats.steelBodyDEFPerStack * maxStacks) : (currentStats.steelBodyDEFPerStack * currentStats.steelBodyStackCount);

            if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Lumiscarapace"))
                currentStats.steelBodyShieldStrengthModifier = float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Lumiscarapace")
                                                      .buffsParentEffect["shieldStrengthPerStack"]) * currentStats.steelBodyStackCount;
            
            if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Résistance Evolutive"))
                currentStats.steelBodyDMGReductionModifier = float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Résistance Evolutive")
                    .buffsParentEffect["damageReductionPerStack"]) * currentStats.steelBodyStackCount;

        }
        
    }

    
    public void CreateShield(float shieldValue)
    {
        if(currentStats.shieldCurrentHP > 0 && currentStats.shieldMaxHP > 0) BreakShield();
        
        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Protection Épineuse"))
        {

            currentStats.shieldStrength += (15 + 
                                             (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Renforcement Défensif") 
                                              && SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Renforcement Défensif").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
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

        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Protection Épineuse"))
        {

            currentStats.shieldStrength -= (15 + 
                                             (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Renforcement Défensif") 
                                              && SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Renforcement Défensif").buffsParentEffect.TryGetValue("effectBonus", out var value1) 
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
        
        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Souffle de Résurrection") && resurectionCounter > 0)
        {
            
            resurectionCounter--;
            SC_FeedbackRoomStatusUI.resurected?.Invoke();

            StartCoroutine(Invicibility(1.5f));
            
            return;
            
        }
        
        if (isDeath) return;
        
        isDeath = true;
        SC_GameManager.instance.ChangeState(GameState.DEFEAT);
        
    }
    
    private void onEnemyKilled(SC_AIStats enemy)
    {

        print("Enemy Killed");

        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Blindage Spirituel"))
        {
            if(debuffsBuffsComponent.CheckHasBuff(Enum_Buff.Armor)) debuffsBuffsComponent.RemoveBuff(Enum_Buff.Armor);
            
            debuffsBuffsComponent.ApplyBuff(Enum_Buff.Armor, 5);
        }
        
        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Lithothérapie") && debuffsBuffsComponent.CheckHasBuff(Enum_Buff.SecondChance))
        {

            if(Random.Range(0, 2) == 1) return;
            
            Heal(Mathf.Round(currentStats.currentMaxHealth * (float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Lithothérapie").buffsParentEffect["healAmount"])/100)));

        }

        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Surcharge d'Essence"))
        {
            GainManaOverloadStack();

            if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Plénitude Acharnée") && currentStats.manaOverloadStack >= 1)
            {
                
                Heal(Mathf.Round(currentStats.currentMaxHealth * (float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Plénitude Acharnée").buffsParentEffect["healAmount"])/100)));
                
            }
            
        }
        
        
        
    }
    
    private void ClearRoom()
    {
        
        if (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Régénération"))
        {
            print("ALLER");
            Heal(2);
        }
        
    }

    private void GainManaOverloadStack()
    {
        
        var maxStack = SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Super-Héraut") ? 
            int.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Super-Héraut").buffsParentEffect["maxStack"])
            : currentStats.manaOverloadMaxStack;
        
        if(currentStats.manaOverloadStack >= maxStack) return;
        
        currentStats.manaOverloadStack++;

        var manaOverloadDMG = (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Elan Vital") ? 
            float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Elan Vital").buffsParentEffect["overloadDMG"])
            : currentStats.manaOverloadDamage)/100;
        
        TakeDamage(manaOverloadDMG, false, null);
        
        currentStats.inManaOverload = true;
        
        if (currentStats.manaOverload != null)
        {
            StopCoroutine(currentStats.manaOverload);
            currentStats.damageBonus -= (currentStats.manaOverloadDamageBoost * (currentStats.manaOverloadStack - 1));
            currentStats.manaOverload = null;
        }
        
        currentStats.manaOverload = StartCoroutine(ManaOverloadBoost());

    }

    private IEnumerator ManaOverloadBoost()
    {
        
        var tempStacks = currentStats.manaOverloadStack;
        var duration = currentStats.manaOverloadDuration * (1 + (SC_GameManager.instance.playerSkillInventory.CheckHasSkillByName("Potentiel Intemporel") ? 
            float.Parse(SC_GameManager.instance.playerSkillInventory.FindChildSkillByName("Potentiel Intemporel").buffsParentEffect["durationMultiplier"])
            : 0));
        
        currentStats.damageBonus += (currentStats.manaOverloadDamageBoost * tempStacks);
        
        yield return new WaitForSeconds(duration);
        
        currentStats.damageBonus -= (currentStats.manaOverloadDamageBoost * tempStacks);
        
        currentStats.manaOverloadStack = 0;
        currentStats.inManaOverload = false;
        
    }
    
    private IEnumerator Invicibility(float duration)
    {

        isInvincible = true;
        
        yield return new WaitForSeconds(duration);

        isInvincible = false;

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

    public void UpdateStats()
    {
     
        print("Update");
        onHealthChange?.Invoke(currentStats.currentHealth, currentStats.currentMaxHealth);
        
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
