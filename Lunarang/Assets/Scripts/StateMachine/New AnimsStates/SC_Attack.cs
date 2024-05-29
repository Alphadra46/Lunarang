using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_Attack : StateMachineBehaviour
{
    [Tooltip("Between 0 and 1, 1 being 100% of the animation and 0 the very start fo the animation")]
    public float timeBeforeBuffering;
    public float exitTime;
    private bool canPerformCombo;
    private Coroutine coroutine;

    [HideInInspector] public bool hasAttackEnded;
    private bool canCancel;
    private bool hasAttacked;

    private bool hasExited;
    
    private Animator _animator;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SC_ComboController.instance.EnableDash();
        hasAttackEnded = false;
        hasExited = false;
        SC_ComboController.instance.ExitPreviousState(this);
        if (SC_ComboController.instance.currentState == null)
            SC_ComboController.instance.currentState = this;
        _animator = animator;
        canPerformCombo = false;
        hasAttacked = false;
        animator.ResetTrigger("canSwitchState");
        
        SC_InputManager.instance.weaponA.started += ctx => Attack(SC_GameManager.instance.weaponInventory.weaponsEquipped[0]);
        SC_InputManager.instance.weaponB.started += ctx => Attack(SC_GameManager.instance.weaponInventory.weaponsEquipped[1]);
        SC_InputManager.instance.weaponC.started += ctx => Attack(SC_GameManager.instance.weaponInventory.weaponsEquipped[2]);
        
        var playerController = animator.transform.parent.GetComponent<SC_PlayerController>();
        playerController.isAttacking = true;
        playerController.FreezeMovement(true);
        
        playerController.StartCoroutine(BufferingTime(timeBeforeBuffering, stateInfo));
        coroutine = playerController.StartCoroutine(ExitTime(exitTime, animator, stateInfo));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) //Bug ? - Maybe exiting multiple times
    {
        Exit();
    }

    public void Exit() //TODO - Dash cancel
    {
        if (hasExited)
            return;
        
        hasExited = true;
        canPerformCombo = false; //Just in case
        _animator.ResetTrigger("canSwitchState");
        _animator.ResetTrigger("chakram");
        _animator.ResetTrigger("rapier");
        _animator.ResetTrigger("hammer");
        SC_InputManager.instance.weaponA.started -= ctx => Attack(SC_GameManager.instance.weaponInventory.weaponsEquipped[0]);
        SC_InputManager.instance.weaponB.started -= ctx => Attack(SC_GameManager.instance.weaponInventory.weaponsEquipped[1]);
        SC_InputManager.instance.weaponC.started -= ctx => Attack(SC_GameManager.instance.weaponInventory.weaponsEquipped[2]);

        var playerController = _animator.transform.parent.GetComponent<SC_PlayerController>();
        
        if (hasAttacked && SC_ComboController.instance.comboCounter < SC_ComboController.instance.comboMaxLength)
            SC_ComboController.instance.IncrementCombo(SC_ComboController.instance.currentWeapon);


        playerController.StopCoroutine(coroutine);
        coroutine = null;
    }
    
    private IEnumerator BufferingTime(float timer, AnimatorStateInfo infos)
    {
        var clip = infos;
        yield return new WaitForSeconds(clip.length*timer);
        //canCancel = false; //TODO - Move this elsewhere like Animation event or get a timer for it
        //SC_PlayerController.instance.canDash = canCancel;
        canPerformCombo = true;
    }
    
    
    /// <summary>
    /// A coroutine that will allow to transition with the next animation after a certain amount of time
    /// </summary>
    /// <param name="exitTime">The percentage of the animation after which the transition is possible. Between 0 and 1</param>
    /// <param name="animator">The animator to act on</param>
    /// <param name="infos">Animation state infos</param>
    public IEnumerator ExitTime(float exitTime, Animator animator, AnimatorStateInfo infos)
    {
        var clip = infos;
        yield return new WaitForSeconds(clip.length*exitTime);
        hasAttackEnded = true;
        SC_ComboController.instance.EnableDash();
        animator.SetTrigger("canSwitchState");
    }
    
    private void Attack(SC_Weapon weaponUsed)
    {
        if (SC_GameManager.instance.isPause)
            return;
        
        if (!canPerformCombo)
            return;

        hasAttacked = true;
        
        //Resetting all weapon triggers
        foreach (var animatorControllerParameter in _animator.parameters.Where(p => p.type == AnimatorControllerParameterType.Trigger && p.name != "death" && p.name != "canSwitchState").ToList())
        {
            _animator.ResetTrigger(animatorControllerParameter.name);
        }

        _animator.SetTrigger(weaponUsed.id);
    }
    
    
}
