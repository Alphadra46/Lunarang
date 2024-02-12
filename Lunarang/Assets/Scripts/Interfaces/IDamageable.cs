
using Enum;

/// <summary>
/// Interface for all Damageable Entities.
/// </summary>
public interface IDamageable
{

    void TakeDamage(float rawDamage, bool trueDamage = false);

    void TakeDamage(float rawDamage, WeaponType pWeaponType, bool isCrit);

    void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType);

}
