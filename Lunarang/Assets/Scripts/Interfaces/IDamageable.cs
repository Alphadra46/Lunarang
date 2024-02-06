
using Enum;

/// <summary>
/// Interface for all Damageable Entities.
/// </summary>
public interface IDamageable
{

    void TakeDamage(float rawDamage);

    void TakeDamage(float rawDamage, WeaponType pWeaponType, bool isCrit);

    void ApplyDebuffToSelf(Enum_Debuff newDebuff);

}
