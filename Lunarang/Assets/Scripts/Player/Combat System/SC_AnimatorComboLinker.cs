using System;
using System.Collections;
using System.Collections.Generic;
using BgTools.CastVisualizer;
using UnityEngine;

public class SC_AnimatorComboLinker : MonoBehaviour
{

    public SC_ComboController comboController;
    public SC_PlayerController movementController;

    // All functions here are just to link the Animator from the Mesh to the ComboController
    
    public void ActivateInputBuffering()
    {
        comboController.ActivateInputBuffering();
    }

    public void DeactivateInputBuffering()
    {
        comboController.DeactivateInputBuffering();
    }

    public void ResetCombo()
    {
        comboController.ResetCombo();
    }

    public void CanPerformCombo()
    {
        comboController.CanPerformCombo();
    }
    
    public void CantPerformCombo()
    {
        comboController.CantPerformCombo();
    }

    
    /// <summary>
    /// Called to create a hitbox at a certain timing in an animation.
    /// </summary>
    /// <param name="hb"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void CreateHitBox(SO_HitBox hb)
    {
        Collider[] hits;
        switch (hb.type)
        {
            
            case HitBoxType.Box:
                hits = Physics.OverlapBox((transform.parent.GetChild(1).position)+hb.center, hb.halfExtents, transform.parent.rotation, hb.layer);
                
                break;
            case HitBoxType.Sphere:
                hits = Physics.OverlapSphere(hb.pos, hb.radiusSphere, hb.layer);
                
                break;
            case HitBoxType.Capsule:
                hits = Physics.OverlapCapsule(hb.point0, hb.point1, hb.radiusCapsule, hb.layer);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        foreach (var entity in hits)
        {
                    
            entity.GetComponent<IDamageable>().TakeDamage(5);
                    
        }
        
    }

    public void FreezeMovement()
    {
        movementController.FreezeMovement(true);
    }
    public void UnfreezeMovement()
    {
        movementController.FreezeMovement(false);
    }
    
}


