﻿using UnityEngine;

public class SC_Projectile_Chakram : SC_Projectile
{
    /// <summary>
    /// Detect collision and if collide with Enemy, apply damage to Enemy.
    /// </summary>
    /// <param name="col"></param>
    public override void OnTriggerEnter(Collider col)
    {
        
        if (!col.TryGetComponent(out IDamageable damageable)) return;
        if (col.gameObject == sender) return;

        if (isAoE)
        {
            var ennemiesInAoE =
                Physics.OverlapSphere(transform.position, areaSize, LayerMask.GetMask("IA")); //TODO : Replace Pos by Weapon Hit Pos

            foreach (var e in ennemiesInAoE)
            {
                print(e.name);
                
                if(e.CompareTag("Player")) return;
                
                if (!e.TryGetComponent(out IDamageable aoeHitted)) continue;
                aoeHitted.TakeDamage(damage, isCrit, sender);
                SC_ComboController.instance.CheckBurnHit(col, true);

                if (additionalHits >= 1) continue;
                
                for (var i = 0; i < additionalHits; i++)
                {
                    aoeHitted.TakeDamage(damage, isCrit, sender);
                    SC_ComboController.instance.CheckBurnHit(col, true);
                }

            }
        }
        
        else
        {
            for (var i = 0; i < additionalHits+1; i++)
            {
                if (!col.CompareTag("Entity")) continue;
                
                damageable.TakeDamage(damage, isCrit, sender);
                SC_ComboController.instance.CheckBurnHit(col, true);

            }
        }
        
    }
    
}
