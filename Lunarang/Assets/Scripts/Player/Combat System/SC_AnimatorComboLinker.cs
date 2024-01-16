using System;
using System.Collections;
using System.Collections.Generic;
using BgTools.CastVisualizer;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_AnimatorComboLinker : MonoBehaviour
{

    public SC_ComboController pComboController;
    public SC_PlayerController pController;
    public SC_PlayerStats pStats;

    // All functions here are just to link the Animator from the Mesh to the ComboController
    
    public void ActivateInputBuffering()
    {
        pComboController.ActivateInputBuffering();
    }

    public void DeactivateInputBuffering()
    {
        pComboController.DeactivateInputBuffering();
    }

    public void ResetCombo()
    {
        pComboController.ResetCombo();
    }

    public void CanPerformCombo()
    {
        pComboController.CanPerformCombo();
    }
    
    public void CantPerformCombo()
    {
        pComboController.CantPerformCombo();
    }

    
    /// <summary>
    /// Called to create a hitbox at a certain timing in an animation.
    /// </summary>
    /// <param name="hb"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void CreateHitBox(SO_HitBox hb)
    {
        var hits = hb.type switch
        {
            HitBoxType.Box => Physics.OverlapBox((transform.parent.GetChild(1).position) + hb.center, hb.halfExtents,
                transform.parent.rotation, hb.layer),
            HitBoxType.Sphere => Physics.OverlapSphere(hb.pos, hb.radiusSphere, hb.layer),
            HitBoxType.Capsule => Physics.OverlapCapsule(hb.point0, hb.point1, hb.radiusCapsule, hb.layer),
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var entity in hits)
        {
            
            var isCritical = Random.Range(0, 100) < pStats.critRate ? true : false;
            
            var currentMV = pComboController.currentWeapon.MovesValues[pComboController.comboCounter-1];
            
            var rawDamage = currentMV * pStats.currentATK;
            var effDamage = rawDamage * (1 + (pStats.damageBonus/100));
            var effCrit = effDamage * (1 + (pStats.critDMG/100));
            
            print(isCritical ? "CRIIIIIT "+ effCrit : effDamage);
            entity.GetComponent<IDamageable>().TakeDamage(isCritical ? effCrit : effDamage, pComboController.currentWeapon.type, isCritical);
            
        }
    }

    public void FreezeMovement()
    {
        pController.FreezeMovement(true);
        pController.FreezeDash(true);
    }
    public void UnfreezeMovement()
    {
        pController.FreezeMovement(false);
        pController.FreezeDash(false);
    }
    
}


