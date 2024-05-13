using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

public class SC_Healpack : MonoBehaviour
{
    #region Variables

    [Title("Setup")]
    [SerializeField]
    private float healAmount;
    [SerializeField]
    private int playerLayerMask;
    private bool canInteract;
    [SerializeField]
    public GameObject uiInteract;

    [Title("Animations")]
    public Animator animator;
    public String startTrigger;
    public String endTrigger;
    [Title("Sounds")]
    public AudioClip activateSound;
    [Title("VFX")]
    public ParticleSystem activateParticule;

    #endregion

    #region Player in range
    //trigger if the player is in range
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerInRange();
        }
    }

    //trigger if the player get out of range
    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerOutOfRange();
        }   
    }
    
    private void PlayerInRange()
    {
        StartAnim();
    }

    private void PlayerOutOfRange()
    {
        EndAnim();
    }

#endregion
    
    public void ActivateHealpack()
    {
        SC_PlayerStats.instance.Heal(healAmount);
        canInteract = false;
        uiInteract.SetActive(false);
        PlayActivateSound();
        PlayActivateVFX();
    }

    #region Animations et FX
    public void StartAnim()
    {
        animator.SetTrigger(startTrigger);
    }

    public void EndAnim()
    {
        animator.SetTrigger(endTrigger);
    }

    public void PlayActivateSound()
    {
        //ajouter le déclenchement du son d'activation
    }

    public void PlayActivateVFX()
    {
        //ajouter le déclenchement des VFX d'activation
    }


#endregion
}
