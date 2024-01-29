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
        pComboController.CreateHitBox(hb);
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


