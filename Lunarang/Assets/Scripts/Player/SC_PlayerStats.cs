using System;
using System.Collections;
using System.Collections.Generic;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class SC_PlayerStats : MonoBehaviour
{

    public static SC_PlayerStats instance;
    
    #region Health

    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    [TabGroup("Stats", "HP", SdfIconType.HeartFill, TextColor = "green"), 
     ProgressBar(0, "maxHealthEffective", r: 0, g: 1, b: 0, Height = 20), ReadOnly] 
    public float currentHealth;
    
    [TabGroup("Stats", "HP")]
    public int maxHealth = 30;
    [TabGroup("Stats", "HP")]
    public float maxHealthModifier = 0f;
    [TabGroup("Stats", "HP")]
    public float maxHealthEffective => maxHealth * (1 + maxHealthModifier);
    

    #endregion

    #region Mana

    [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
    [TabGroup("Stats", "MP", SdfIconType.StarFill, TextColor = "blue"),
     ProgressBar(0, "maxManaEffective", r: 0, g: 0.75f, b: 1, Height = 20), ReadOnly]
    public float currentMana;
    
    [TabGroup("Stats", "MP")]
    public int maxMana = 10;
    [TabGroup("Stats", "MP")]
    public float maxManaModifier = 0f;
    [TabGroup("Stats", "MP")]
    public float maxManaEffective => maxMana * (1 + maxManaModifier);
    

    #endregion

    #region ATK

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "ATK",TextColor = "red"), ShowInInspector, ReadOnly] public float currentAttack => atkBase * (1 + atkModifier);
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "ATK"), ShowInInspector] private int atkBase = 5;
    [TabGroup("Stats", "ATK")] public float atkModifier;

    #endregion

    #region SPD

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector] public float currentSpeed => baseSpeed * (1 + speedModifier);
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "SPD"), ShowInInspector] private int baseSpeed;
    [TabGroup("Stats", "SPD")] public float speedModifier;

    #endregion

    #region Crit

    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "Crit",TextColor = "darkred")]
    [Range(0, 100)]
    public float critRate = 5;
    
    [TabGroup("Stats", "Crit")]
    public float critDmg = 1.5f;
    
    [PropertySpace(SpaceBefore = 10)]
    [TabGroup("Stats", "Crit"), ShowInInspector, ReadOnly]
    public float critValue => critDmg * critRate;

    #endregion
    

    #region Status
    
    [TabGroup("Status", "Debuffs")]
    public List<Enum_Debuff> currentDebuffs;

    #endregion
    
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    #region Status

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
    
    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage < 0 ? 0 : currentHealth - damage;
    }

    public void Heal(int healAmount)
    {
        currentHealth = currentHealth + healAmount > maxHealth ? maxHealth : currentHealth + healAmount;
    }
    
}
