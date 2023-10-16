using System;
using System.Collections;
using System.Collections.Generic;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_PlayerStats : MonoBehaviour
{

    public static SC_PlayerStats instance;
    
    [Header("Player statistics")]
    
    public int maxHealth;
    public float maxHealthModifier = 0f;
    public float maxHealthEffective => maxHealth * (1 + maxHealthModifier);
    [HideInInspector] public float currentHealth;

    public int maxMana;
    public float maxManaModifier = 0f;
    public float maxManaEffective => maxMana * (1 + maxManaModifier);
    [HideInInspector] public float currentMana;
    
    [SerializeField] private int weaponAttack;
    [HideInInspector] public float attackModifier;
    [SerializeField] public float currentAttack => weaponAttack * (1 + attackModifier);
    
    
    private int baseSpeed;
    [HideInInspector] public float speedModifier;
    [ShowInInspector] public float currentSpeed => baseSpeed * (1 + speedModifier);
    
    public float critRate;

    //DEBUG ONLY
    public float curATK;
    public float curSPD;

    #region Status

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

    // Update is called once per frame
    void Update()
    {
        curATK = currentAttack;
        curSPD = currentSpeed;
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
