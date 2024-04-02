using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities
{
    [Serializable]
    public struct Stats
    {
        
        #region Health

        [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
        [TabGroup("Stats", "HP", SdfIconType.HeartFill, TextColor = "green"), 
         ProgressBar(0, "currentMaxHealth", r: 0, g: 1, b: 0, Height = 20), ReadOnly] 
        public float currentHealth;
        
        [TabGroup("Stats", "HP")]
        public int maxHealth;
        [TabGroup("Stats", "HP")]
        public float maxHealthModifier;
        [TabGroup("Stats", "HP")] public float currentMaxHealth => maxHealth * (1 + (maxHealthModifier/100));
        
        #endregion

        #region DEF
        
        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "DEF",SdfIconType.ShieldFill, TextColor = "blue"), ShowInInspector, ReadOnly]
        public float currentDEF => defBase * (1 + (defModifier/100) + (steelBodyDEFModifier/100));

        [TabGroup("Stats", "DEF")] 
        public int defBase;
        [TabGroup("Stats", "DEF")]
        public float defModifier;
        
        [TabGroup("Stats", "DEF")]
        [Tooltip("DEF Stat used to reduce damage taken"), ShowInInspector, ReadOnly]
        public float defMultiplier => (100 / (100 + currentDEF));

        #endregion

        #region ATK

        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "ATK",TextColor = "red"), ShowInInspector, ReadOnly]
        public float currentATK => atkBase * (1 + (atkModifier/100));
        
        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "ATK"), ShowInInspector] public int atkBase;
        [TabGroup("Stats", "ATK")] public float atkModifier;

        #endregion

        #region Crit

        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "Crit",TextColor = "darkred")]
        [FoldoutGroup("Stats/Crit/Base Rate")]
        [Range(0, 100)]
        public float baseCritRate;
        [PropertySpace(SpaceBefore = 5)]
        [TabGroup("Stats", "Crit",TextColor = "darkred")]
        [FoldoutGroup("Stats/Crit/Base Rate")]
        public float bonusCritRate;
        
        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "Crit",TextColor = "darkred"), ShowInInspector, ReadOnly]
        public float critRate => baseCritRate + bonusCritRate;
        
        
        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "Crit")]
        [FoldoutGroup("Stats/Crit/Base DMG")]
        [Range(50, 1000)]
        public float baseCritDMG;
        [PropertySpace(SpaceBefore = 5)]
        [TabGroup("Stats", "Crit")]
        [FoldoutGroup("Stats/Crit/Base DMG")]
        public float bonusCritDMG;
        
        
        [TabGroup("Stats", "Crit",TextColor = "darkred"), ShowInInspector, ReadOnly]
        public float critDMG => (baseCritDMG + bonusCritDMG);
        
        
        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "Crit"), ShowInInspector, ReadOnly]
        public float critValue => critDMG + (critRate * 2);

        #endregion

        #region DMG

        [TabGroup("Stats", "DMG"), PropertySpace(SpaceAfter = 5f)]
        public float damageReduction;
        
            #region Taken
            
            [TabGroup("Stats", "DMG",TextColor = "darkred")]
            [FoldoutGroup("Stats/DMG/Taken")]
            public float damageTaken;
            
            [TabGroup("Stats", "DMG")]
            [FoldoutGroup("Stats/DMG/Taken")]
            public float dotDamageTaken;

            #endregion
        
            #region Bonus

            [TabGroup("Stats", "DMG")]
            [FoldoutGroup("Stats/DMG/Bonus")]
            public float damageBonus;

            [TabGroup("Stats", "DMG")]
            [FoldoutGroup("Stats/DMG/Bonus")]
            public float mhDamageBonus;
            
            [TabGroup("Stats", "DMG")]
            [FoldoutGroup("Stats/DMG/Bonus")]
            public float aoeDamageBonus;
            
            [TabGroup("Stats", "DMG")]
            [FoldoutGroup("Stats/DMG/Bonus")]
            public float projectileDamageBonus;
            
            [TabGroup("Stats", "DMG")]
            [FoldoutGroup("Stats/DMG/Bonus")]
            public float dotDamageBonus;

            #endregion
        
        #endregion

        #region Hit Rates

        [TabGroup("Stats", "Hit Rates"), MaxValue(100f), MinValue(0f)]
        public float poisonHitRate;
        
        [TabGroup("Stats", "Hit Rates"), MaxValue(100f), MinValue(0f)]
        public float freezeHitRate;
        
        [TabGroup("Stats", "Hit Rates"), MaxValue(100f), MinValue(0f)]
        public float burnHitRate;
        
        [TabGroup("Stats", "Hit Rates"), MaxValue(100f), MinValue(0f)]
        public float bleedHitRate;
        
        #endregion
    
        #region SPD

        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector]
        public float currentSpeed => baseSpeed * (1 + (speedModifier/100));
        
        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "SPD"), SerializeField] public int baseSpeed;
        [TabGroup("Stats", "SPD")] public float speedModifier;

        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "SPD", SdfIconType.Speedometer, TextColor = "purple"), ShowInInspector]
        public float currentATKSpeed => baseATKSpeed * (1 + (atkSpeedModifier/100));
        
        [PropertySpace(SpaceBefore = 10)]
        [TabGroup("Stats", "SPD"), SerializeField] public int baseATKSpeed;
        [TabGroup("Stats", "SPD")] public float atkSpeedModifier;
        
        #endregion

        #region Others

        [TabGroup("Stats", "Others")]
        public float healingBonus;

        [TabGroup("Stats", "Others")]
        public float dishesEffectBonus;
        
            #region Shield

            [TabGroup("Stats", "Shield")]
            public float shieldMaxHP;
            [TabGroup("Stats", "Shield")]
            public float shieldCurrentHP;
            [TabGroup("Stats", "Shield")]
            public float shieldStrength;

            #endregion

            #region Mana Overload

            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
            [ReadOnly, ShowInInspector] public int manaOverloadStack;
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
            public int manaOverloadMaxStack;
            
            [PropertySpace(SpaceBefore = 10f)]
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
            public float manaOverloadDamageBoost;
            
            [PropertySpace(SpaceBefore = 10f)]
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
            public float manaOverloadTick;
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
            public float manaOverloadDuration;
            [PropertySpace(SpaceBefore = 10f)]
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload")]
            public float manaOverloadDamage;
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Overload"),ShowInInspector]
            public bool inManaOverload;

            public Coroutine manaOverload;

            #endregion

            #region Mana Fury

            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Fury")]
            public float manaFuryMaxHPGate;
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Mana Fury")]
            public bool inManaFury;

            #endregion

            #region Steel Body
            
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Steel Body")]
            [ReadOnly, ShowInInspector] public int steelBodyStackCount;
            
            [PropertySpace(SpaceBefore = 10f)]
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Steel Body")]
            public float steelBodyHPPercentNeeded;
            
            [PropertySpace(SpaceBefore = 10f)]
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Steel Body")]
            public float steelBodyDEFPerStack;
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Steel Body")]
            [ReadOnly, ShowInInspector] public float steelBodyDEFModifier;
            
            [PropertySpace(SpaceBefore = 10f)]
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Steel Body")]
            public float steelBodyDMGReductionModifier;
            
            [PropertySpace(SpaceBefore = 10f)]
            [TabGroup("Stats", "Others"),FoldoutGroup("Stats/Others/Steel Body")]
            public float steelBodyShieldStrengthModifier;

            #endregion

        #endregion
        
    }
    
    public class SC_EntityBase : MonoBehaviour
    {

        public Stats baseStats = new Stats();
        [PropertySpace(SpaceBefore = 10f)]
        public Stats currentStats = new Stats();

        private delegate float ModificationDelegate(float a, float b);

        private static float PercentageModification(float baseValue, float percentageValue)
        {
            return baseValue * (1 + (percentageValue / 100));
        }
        
        private static float NumericalModification(float baseValue, float numericalValue)
        {
            return baseValue + numericalValue;
        }
        
        private static float AbosluteModification(float baseValue, float absoluteValue)
        {
            return absoluteValue;
        }

        public Stats ModifyStats(in Stats baseStats, out Stats modifiedStats, in SC_StatModification statMod)
        {

            ModificationDelegate modOperation = statMod.ModificationType switch
            {
                StatModificationType.Percentage => PercentageModification,
                StatModificationType.Numerical => NumericalModification,
                StatModificationType.Absolute => AbosluteModification,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            modifiedStats = baseStats;

            if (!TryGetComponent(out SC_DebuffsBuffsComponent debuffsBuffsComponent)) return modifiedStats;

            switch (statMod.StatToModify)
            {
                case StatTypes.HPMAX:
                    modifiedStats.maxHealthModifier =
                        modOperation(baseStats.maxHealthModifier, statMod.ModificationValue);
                    break;
                
                case StatTypes.Healing:
                    modifiedStats.healingBonus =
                        modOperation(baseStats.healingBonus, statMod.ModificationValue);
                    break;
                case StatTypes.DishesEffect:
                    modifiedStats.dishesEffectBonus =
                        modOperation(baseStats.dishesEffectBonus, statMod.ModificationValue);
                    break;
                
                case StatTypes.ATK:
                    modifiedStats.atkModifier =
                        modOperation(baseStats.atkModifier, statMod.ModificationValue);
                    break;
                case StatTypes.DEF:
                    modifiedStats.defModifier =
                        modOperation(baseStats.defModifier, statMod.ModificationValue);
                    break;
                
                case StatTypes.DMGReduction:
                    modifiedStats.damageReduction =
                        modOperation(baseStats.damageReduction, statMod.ModificationValue);
                    break;
                case StatTypes.DMGTaken:
                    modifiedStats.damageTaken =
                        modOperation(baseStats.damageTaken, statMod.ModificationValue);
                    break;
                case StatTypes.DoTDMGTaken:
                    modifiedStats.dotDamageTaken =
                        modOperation(baseStats.dotDamageTaken, statMod.ModificationValue);
                    break;
                
                case StatTypes.ATKSPD:
                    modifiedStats.atkSpeedModifier =
                        modOperation(baseStats.atkSpeedModifier, statMod.ModificationValue);
                    break;
                case StatTypes.MovementSPD:
                    modifiedStats.speedModifier =
                        modOperation(baseStats.speedModifier, statMod.ModificationValue);
                    break;
                
                case StatTypes.DMG:
                    modifiedStats.damageBonus =
                        modOperation(baseStats.damageBonus, statMod.ModificationValue);
                    break;
                case StatTypes.MultiHitDMG:
                    modifiedStats.mhDamageBonus =
                        modOperation(baseStats.mhDamageBonus, statMod.ModificationValue);
                    break;
                case StatTypes.AoEDMG:
                    modifiedStats.aoeDamageBonus =
                        modOperation(baseStats.aoeDamageBonus, statMod.ModificationValue);
                    break;
                case StatTypes.ProjectileDMG:
                    modifiedStats.projectileDamageBonus =
                        modOperation(baseStats.projectileDamageBonus, statMod.ModificationValue);
                    break;
                
                case StatTypes.DoTDMG:
                    modifiedStats.dotDamageBonus =
                        modOperation(baseStats.dotDamageBonus, statMod.ModificationValue);
                    break;
                
                case StatTypes.DoTCritRate:
                    debuffsBuffsComponent.dotCritRate =
                        modOperation(debuffsBuffsComponent.dotCritRate, statMod.ModificationValue);
                    break;
                case StatTypes.DoTCritDMG:
                    debuffsBuffsComponent.dotCritRate =
                        modOperation(debuffsBuffsComponent.dotCritRate, statMod.ModificationValue);
                    break;
                case StatTypes.DoTDurationBonus:
                    debuffsBuffsComponent.dotCritRate =
                        modOperation(debuffsBuffsComponent.dotCritRate, statMod.ModificationValue);
                    break;
                
                case StatTypes.PoisonHitRate:
                    modifiedStats.poisonHitRate = modOperation(baseStats.poisonHitRate, statMod.ModificationValue);
                    break;
                case StatTypes.PoisonStackByHit:
                    debuffsBuffsComponent.poisonStackByHit =
                        (int)modOperation(debuffsBuffsComponent.poisonStackByHit, statMod.ModificationValue);
                    break;
                case StatTypes.PoisonMaxStack:
                    debuffsBuffsComponent.poisonMaxStack =
                        (int)modOperation(debuffsBuffsComponent.poisonMaxStack, statMod.ModificationValue);
                    break;
                case StatTypes.PoisonTick:
                    debuffsBuffsComponent.poisonTick =
                        modOperation(debuffsBuffsComponent.poisonTick, statMod.ModificationValue);
                    break;
                case StatTypes.PoisonDuration:
                    debuffsBuffsComponent.poisonDuration =
                        modOperation(debuffsBuffsComponent.poisonDuration, statMod.ModificationValue);
                    break;
                case StatTypes.PoisonDMG:
                    debuffsBuffsComponent.poisonDMGBonus =
                        modOperation(debuffsBuffsComponent.poisonDMGBonus, statMod.ModificationValue);
                    break;
                
                case StatTypes.BleedHitRate:
                    
                    break;
                case StatTypes.BleedStackByHit:
                    break;
                case StatTypes.BleedMaxStack:
                    break;
                case StatTypes.BleedTick:
                    break;
                case StatTypes.BleedDuration:
                    break;
                case StatTypes.BleedDMG:
                    break;
                
                case StatTypes.BurnHitRate:
                    modifiedStats.burnHitRate =
                        modOperation(baseStats.burnHitRate, statMod.ModificationValue);
                    break;
                case StatTypes.BurnAoESize:
                    debuffsBuffsComponent.burnAoESize =
                        modOperation(debuffsBuffsComponent.burnAoESize, statMod.ModificationValue);
                    break;
                case StatTypes.BurnTick:
                    debuffsBuffsComponent.burnTick =
                        modOperation(debuffsBuffsComponent.burnTick, statMod.ModificationValue);
                    break;
                case StatTypes.BurnMaxStack:
                    debuffsBuffsComponent.burnMaxStack =
                        (int) modOperation(debuffsBuffsComponent.burnMaxStack, statMod.ModificationValue);
                    break;
                case StatTypes.BurnAoEHitRate:
                    debuffsBuffsComponent.burnAoEHitRate =
                        modOperation(debuffsBuffsComponent.burnAoEHitRate, statMod.ModificationValue);
                    break;
                case StatTypes.BurnDMG:
                    debuffsBuffsComponent.burnDMGBonus =
                        modOperation(debuffsBuffsComponent.burnDMGBonus, statMod.ModificationValue);
                    break;
                
                case StatTypes.FreezeHitRate:
                    modifiedStats.freezeHitRate = modOperation(baseStats.freezeHitRate, statMod.ModificationValue);
                    break;
                case StatTypes.FreezeDuration:
                    debuffsBuffsComponent.freezeDuration =
                        modOperation(debuffsBuffsComponent.freezeDuration, statMod.ModificationValue);
                    break;
                case StatTypes.FreezeDurationBonus:
                    debuffsBuffsComponent.freezeDurationBonus =
                        modOperation(debuffsBuffsComponent.freezeDurationBonus, statMod.ModificationValue);
                    break;
                case StatTypes.UnfreezeAoESize:
                    debuffsBuffsComponent.unfreezeAoESize =
                        modOperation(debuffsBuffsComponent.unfreezeAoESize, statMod.ModificationValue);
                    break;
                case StatTypes.UnfreezeAoEMV:
                    debuffsBuffsComponent.unfreezeAoEMV =
                        modOperation(debuffsBuffsComponent.unfreezeAoEMV, statMod.ModificationValue);
                    break;
                
                case StatTypes.ManaOverloadMaxStack:

                    modifiedStats.manaOverloadMaxStack = (int) modOperation(baseStats.manaOverloadMaxStack, statMod.ModificationValue);
                        
                    break;
                case StatTypes.ManaOverloadDamageTick:
                    modifiedStats.manaOverloadDamage = (int) modOperation(baseStats.manaOverloadDamage, statMod.ModificationValue);
                    break;
                case StatTypes.ManaOverloadDuration:
                    modifiedStats.manaOverloadDuration = (int) modOperation(baseStats.manaOverloadDuration, statMod.ModificationValue);
                    break;
                
                case StatTypes.ManaFuryMaxHP:
                    modifiedStats.manaFuryMaxHPGate = (int) modOperation(baseStats.manaFuryMaxHPGate, statMod.ModificationValue);
                    break;
                    
                case StatTypes.CritRate:
                    modifiedStats.bonusCritRate = modOperation(baseStats.bonusCritRate, statMod.ModificationValue);
                    break;
                case StatTypes.CritDamage:
                    modifiedStats.bonusCritDMG = modOperation(baseStats.bonusCritDMG, statMod.ModificationValue);
                    break;
            }

            return modifiedStats;

        }

        protected Quaternion GetCurrentForwardVector(Quaternion orientation)
        {

            Vector3 forward = transform.forward;

            forward.y = 0;

            forward.Normalize();

            Quaternion rotation = Quaternion.LookRotation(forward);

            return rotation;

        }
        
    }
    
}