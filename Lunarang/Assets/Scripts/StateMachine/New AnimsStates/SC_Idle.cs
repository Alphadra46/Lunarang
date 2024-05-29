using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class SC_Idle : StateMachineBehaviour
{
    private Animator _animator;

    private bool isAttackLaunched;

    private bool hasExited;
    
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasExited = false;
        SC_ComboController.instance.ExitPreviousState(this);
        if (SC_ComboController.instance.currentState == null)
            SC_ComboController.instance.currentState = this;
        SC_PlayerController.instance.FreezeMovement(false);
        isAttackLaunched = false;
        _animator = animator;

        SC_InputManager.instance.weaponA.started += ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[0]);
        SC_InputManager.instance.weaponB.started += ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[1]);
        SC_InputManager.instance.weaponC.started += ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[2]);
        //SC_ComboController.instance.StartCoroutine(DelayedInit());

    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(0.1f);
        SC_InputManager.instance.weaponA.started += ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[0]);
        SC_InputManager.instance.weaponB.started += ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[1]);
        SC_InputManager.instance.weaponC.started += ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[2]);
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Exit();
    }

    public void Exit()
    {
        if (hasExited)
            return;
        
        hasExited = true;
        SC_InputManager.instance.weaponA.started -= ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[0]);
        SC_InputManager.instance.weaponB.started -= ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[1]);
        SC_InputManager.instance.weaponC.started -= ctx => AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[2]);
    }
    
    private void AttackState(SC_Weapon weaponUsed) //TODO - Unable to attack when in pause
    {
        if (isAttackLaunched)
            return;
        
        SC_ComboController.instance.IncrementCombo(weaponUsed);
        isAttackLaunched = true;

        _animator.SetTrigger(weaponUsed.id);
        
    }
    
}
