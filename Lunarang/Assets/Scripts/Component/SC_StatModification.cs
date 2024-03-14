using UnityEngine;

public class SC_StatModification
{

    public StatTypes StatToModify;
    public StatModificationType ModificationType;
    public float ModificationValue;
    public float timer;
    
}

public enum StatTypes
{
    HP,
    HPMAX,
    Healing,
    DishesEffect,
    
    ATK,
    
    DEF,
    DMGReduction,
    
    DMGTaken,
    DoTDMGTaken,
    
    ATKSPD,
    MovementSPD,
    DashSPD,
    
    DMG,
    MultiHitDMG,
    AoEDMG,
    ProjectileDMG,
    
    DoTDMG,
    
    DoTCritRate,
    DoTCritDMG,
    DoTDurationBonus,
    
    PoisonHitRate,
    PoisonStackByHit,
    PoisonMaxStack,
    PoisonTick,
    PoisonDuration,
    PoisonDMG,
    
    BleedHitRate,
    BleedStackByHit,
    BleedMaxStack,
    BleedTick,
    BleedDuration,
    BleedDMG,
    
    BurnHitRate,
    BurnAoESize,
    BurnTick,
    BurnMaxStack,
    BurnAoEHitRate,
    BurnDMG,
    
    FreezeHitRate,
    FreezeDuration,
    FreezeDurationBonus,
    UnfreezeAoESize,
    UnfreezeAoEMV,
    UnfreezeAoEHitRate,
    
    ManaOverloadMaxStack,
    ManaOverloadDamageTick,
    ManaOverloadDuration,
    
    ManaFuryMaxHP,
    
    CritRate,
    CritDamage
    
}

public enum StatModificationType
{
    
    Percentage,
    Numerical,
    Absolute,
    None
    
}