using System;
using System.Collections;
using System.Collections.Generic;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
    public float currentATK => atkBase * (1 + atkModifier);
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "ATK"), ShowInInspector] private int atkBase = 5;
    [TabGroup("Stats", "ATK")] public float atkModifier;

    #endregion

    #region SPD

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector]
    public float currentSpeed => baseSpeed * (1 + speedModifier);
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD"), SerializeField] private int baseSpeed = 7;
    [TabGroup("Stats", "SPD")] public float speedModifier;

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
    public float critDMG => (baseCritDMG + bonusCritDMG)/100;
    
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "Crit"), ShowInInspector, ReadOnly]
    public float critValue => critDMG + (critRate * 2);

    #endregion
    

    #region Status
    
    [TabGroup("Status", "Debuffs")]
    public List<Enum_Debuff> currentDebuffs;
    [TabGroup("Status", "Debugs")]
    public bool isGod;
    
    
    #endregion

    private SC_PlayerController _controller;
    public GameObject mainHUD;
    public SC_UI_HealthBar hpBar;

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
    }
    
    /// <summary>
    /// At the start, set currentHP to the maxHP
    /// </summary>
    private void Start()
    {
        currentHealth = maxHealth;
        hpBar.HealthUpdate(currentHealth, maxHealth);
    }

    private void Update()
    {
    }
    
    #endregion

    #region Status
    
    /// <summary>
    /// Apply a debuff to self with a certain type, a certain activation cooldown and a duration.
    /// </summary>
    /// <param name="newDebuff">Debuff to apply</param>
    /// <param name="tick">Cooldown between to activation</param>
    /// <param name="duration">Duration before debuff expire</param>
    private void ApplyDebuffToSelf(Enum_Debuff newDebuff, float tick=1, float duration=5)
    {
        
        currentDebuffs.Add(newDebuff);

        switch (newDebuff)
        {
            case Enum_Debuff.Poison:
                StartCoroutine(PoisonDoT((maxHealthEffective * 0.1f), tick, duration));
                break;
        }
        
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
            currentHealth -= finalDamage;
            duration -= tick;
            
            // Debug Part
            print("Dummy : -" + finalDamage + " HP");
            print((duration+1) + " seconds reamining");
            
            // _renderer.UpdateHealthBar(currentHealth, maxHealthEffective);
            // _renderer.DebugDamage(finalDamage);
            
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
        
        //hpBar.HealthUpdate(currentHealth, maxHealth);
        NotifyObservers(currentHealth, maxHealth);
    }

    public void TakeDamage(float rawDamage, WeaponType weaponType)
    {
    }

    /// <summary>
    /// Heal the player by a certain amount
    /// </summary>
    /// <param name="healAmount"></param>
    public void Heal(int healAmount)
    {
        // Check if the heal don't exceed the Max HP limit, if yes, set to max hp, else increment currentHP by healAmount.
        currentHealth = currentHealth + healAmount > maxHealth ? maxHealth : currentHealth + healAmount;
        
        hpBar.HealthUpdate(currentHealth, maxHealth);
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
