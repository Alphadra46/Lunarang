
using Enum;
using UnityEngine;

/// <summary>
/// Interface for all Damageable Entities.
/// </summary>
public interface IDamageable
{

    void TakeDamage(float rawDamage, bool isCrit, GameObject attacker, bool trueDamage = false);

    void TakeDoTDamage(float rawDamage, bool isCrit, Enum_Debuff dotType);

}
