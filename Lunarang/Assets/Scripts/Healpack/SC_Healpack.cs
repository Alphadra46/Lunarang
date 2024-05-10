using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Healpack : MonoBehaviour
{
#region Variables
    [SerializeField]
    private float healAmount; 
    [SerializeField]
    private int playerLayerMask;
    [SerializeField]
    private bool canInteract;
    [SerializeField]
    private Animator animator;
    public String startTrigger;
    public String endTrigger;
#endregion

#region Détection de collision
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
#endregion
    private void PlayerInRange()
    {
        canInteract = true;
        Debug.Log("Loü est à portée");
        StartAnim();
    }

    private void PlayerOutOfRange()
    {
        Debug.Log("Rien à portée");
        canInteract = false;
        EndAnim();
    }

    private void ActivateHealpack()
    {
        SC_PlayerStats.instance.Heal(healAmount);
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

#endregion
}
