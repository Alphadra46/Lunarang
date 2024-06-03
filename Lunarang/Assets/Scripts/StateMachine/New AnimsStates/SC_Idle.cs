using System.Collections;
using System.Collections.Generic;
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

        SC_InputManager.instance.weaponA.started += AttackWeaponCheck;
        SC_InputManager.instance.weaponB.started += AttackWeaponCheck;
        SC_InputManager.instance.weaponC.started += AttackWeaponCheck;
        //SC_ComboController.instance.StartCoroutine(DelayedInit());

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
        SC_InputManager.instance.weaponA.started -= AttackWeaponCheck;
        SC_InputManager.instance.weaponB.started -= AttackWeaponCheck;
        SC_InputManager.instance.weaponC.started -= AttackWeaponCheck;
    }

    private void AttackWeaponCheck(InputAction.CallbackContext context)
    {
        var weapon = context.action.name;
        
        switch (weapon)
        {
            case "Weapon A":
                AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[0]);
                break;
            case "Weapon B":
                AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[1]);
                break;
            case "Weapon C":
                AttackState(SC_GameManager.instance.weaponInventory.weaponsEquipped[2]);
                break;
            default:
                break;
        }
    }
    
    private void AttackState(SC_Weapon weaponUsed) //TODO - Unable to attack when in pause
    {
        if (SC_GameManager.instance.isPause)
            return;
        
        if (isAttackLaunched)
            return;
        
        SC_ComboController.instance.IncrementCombo(weaponUsed);
        isAttackLaunched = true;

        _animator.SetTrigger(weaponUsed.id);
        
    }
    
}
