using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
    
    
public class AttackAnimatorState : StateMachineBehaviour
{
    public float exitTime;

    private Coroutine coroutine;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerController = animator.transform.parent.GetComponent<SC_PlayerController>();
        playerController.FreezeMovement(true);
        coroutine = playerController.StartCoroutine(ExitTime(exitTime, animator,stateInfo));
        SC_ComboController.instance.canPerformCombo = false;
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerController = animator.transform.parent.GetComponent<SC_PlayerController>();
        playerController.FreezeMovement(false);
        SC_ComboController.instance.canPerformCombo = true;
        playerController.StopCoroutine(coroutine);
        coroutine = null;
        foreach (var animatorControllerParameter in animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Trigger).ToList())
        {
            animator.ResetTrigger(animatorControllerParameter.name);
        }
    }

    public IEnumerator ExitTime(float exitTime, Animator animator, AnimatorStateInfo infos)
    {
        var clip = infos;
        yield return new WaitForSeconds(clip.length/clip.speed*exitTime);
        animator.SetTrigger("canSwitchState");

    }
    
    
    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
