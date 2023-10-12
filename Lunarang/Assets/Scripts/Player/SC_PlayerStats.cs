using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PlayerStats : MonoBehaviour
{

    public static SC_PlayerStats instance;
    
    [Header("Player statistics")]
    
    public int maxHealth;
    [HideInInspector] public int currentHealth;

    public int maxMana;
    [HideInInspector] public int currentMana;
    
    [SerializeField] private int weaponAttack;
    [HideInInspector] public float attackModifier;
    [SerializeField] public float currentAttack => weaponAttack * (1 + attackModifier);
    private int baseSpeed;
    [HideInInspector] public float speedModifier;
    [SerializeField] public float currentSpeed => baseSpeed * (1 + speedModifier);
    public float critRate;

    //DEBUG ONLY
    public float curATK;
    public float curSPD;
    
    
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


    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage < 0 ? 0 : currentHealth - damage;
    }

    public void Heal(int healAmount)
    {
        currentHealth = currentHealth + healAmount > maxHealth ? maxHealth : currentHealth + healAmount;
    }
    
}
