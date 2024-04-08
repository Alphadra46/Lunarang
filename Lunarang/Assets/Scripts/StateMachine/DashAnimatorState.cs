using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAnimatorState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SC_ComboController.instance.canPerformCombo = false;
        Debug.Log(stateInfo.length/stateInfo.speed +" Dash duration");
        SC_PlayerController.instance.canDash = false;

    }
    

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SC_ComboController.instance.canPerformCombo = true;
        SC_PlayerController.instance.isDashing = false;
        SC_PlayerController.instance.canDash = true;
    }
}
