using System;
using System.Collections;
using System.Collections.Generic;
using BgTools.CastVisualizer;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class SC_AnimatorComboLinker : MonoBehaviour
{

    public SC_ComboController pComboController;
    public SC_PlayerController pController;
    public SC_FinalATK_Builder pFABuilder;

    // All functions here are just to link the Animator from the Mesh to the ComboController

    public void ResetCombo()
    {
        pComboController.ResetCombo();
    }

    public void FinalAttack()
    {
        pFABuilder.GetInfosFromLastAttacks(pComboController.currentComboWeapons, pComboController);
    }
    
    /// <summary>
    /// Called to create a hitbox at a certain timing in an animation.
    /// </summary>
    /// <param name="hb"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void CreateHitBox(SO_HitBox hb)
    {
        if(hb == null) return;
        
        pComboController.CreateHitBox(hb);
    }

    public void CreateMultihit(int additionalHits)
    {
        // print(pComboController.currentEnemiesHitted.Length);
        pComboController.Multihit(additionalHits);
        
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

    public void EnableDash()
    {
        SC_ComboController.instance.EnableDash();
    }

    public void DisableDash()
    {
        SC_ComboController.instance.DisableDash();
    }

    public void ShowWeapon()
    {
        
        pComboController.equippedWeaponsGO[pComboController.currentWeapon.id].transform.parent.gameObject.SetActive(true);
        
    }
    
    public void HideWeapon()
    {
        
        pComboController.equippedWeaponsGO[pComboController.currentWeapon.id].transform.parent.gameObject.SetActive(false);
        
    }
    
    public void PlayVFX(VisualEffect vfx)
    {
        
        vfx.Play();
        
    }

    public void PlaySFX(AudioClip sfx)
    {
        
        if(sfx != null)
            SC_PlayerStats.instance.sfxPlayer.PlayClip(sfx);
        
    }
    
}


