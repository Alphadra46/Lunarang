
/// <summary>
/// Interface for all Damageable Entities.
/// </summary>
public interface IDamageable
{

    void TakeDamage(float rawDamage);

    void TakeDamage(float rawDamage, WeaponType pWeaponType);

}
