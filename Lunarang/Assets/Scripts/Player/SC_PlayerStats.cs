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
    [TabGroup("Stats", "HP")] public float currentMaxHealth => maxHealth * (1 + maxHealthModifier);
    
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

    #region Hit Rates

    [TabGroup("Stats", "Hit Rates"), MaxValue(100f), MinValue(0f)]
    public float poisonHitRate = 0f;
    
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
    
    [TabGroup("Status", "Debugs")]
    public bool isGod;
    
    [TabGroup("Stats", "Others")]
    public float healingBonus = 0f;

    [TabGroup("Stats", "Others")]
    public float dishesEffectBonus = 0f;
    
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
        currentHealth = currentMaxHealth;
        NotifyObservers(currentHealth, currentMaxHealth);
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
        
        NotifyObservers(currentHealth, currentMaxHealth);
    }

    public void TakeDamage(float rawDamage, WeaponType weaponType, bool isCrit){}
    public void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType)
    {
        if(_controller.isDashing || isGod) return;
            
        var finalDamage = rawDamage - currentDEF;
        
        currentHealth = currentHealth - finalDamage < 0 ? 0 : currentHealth - finalDamage;
        if (DeathCheck())
        {
            Death();
        }
        
        NotifyObservers(currentHealth, currentMaxHealth);
    }

    /// <summary>
    /// Heal the player by a certain amount
    /// </summary>
    /// <param name="healAmount"></param>
    public void Heal(int healAmount)
    {
        // Check if the heal don't exceed the Max HP limit, if yes, set to max hp, else increment currentHP by healAmount.
        currentHealth = currentHealth + healAmount > maxHealth ? maxHealth : currentHealth + healAmount;
        
        NotifyObservers(currentHealth, currentMaxHealth);
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
