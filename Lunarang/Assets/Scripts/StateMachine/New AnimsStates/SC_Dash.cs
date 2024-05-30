using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Dash : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SC_ComboController.instance.canPerformCombo = false;
        Debug.Log(stateInfo.length/stateInfo.speed +" Dash duration");
        SC_PlayerController.instance.canDash = false;
        var attackState = SC_ComboController.instance.currentState as SC_Attack;
        if (attackState != null) //TODO - Not always what we want, if the cancel is after the attack then no need to reset the combo
        {
            if (!attackState.hasAttackEnded)
            {
                SC_ComboController.instance.CancelAttack();
                SC_ComboController.instance.currentComboWeapons.Remove(SC_ComboController.instance.currentComboWeapons[^1]);
            }
            SC_ComboController.instance.ExitPreviousState(this); //Not sure of this
            SC_ComboController.instance.currentState = null;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SC_ComboController.instance.canPerformCombo = true;
        SC_PlayerController.instance.isDashing = false;
        SC_PlayerController.instance.canDash = true;
    }
}
