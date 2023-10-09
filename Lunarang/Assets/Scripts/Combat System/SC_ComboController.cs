using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SC_ComboController : MonoBehaviour
{
    #region Variables
    
    [Header("Settings")]
    [SerializeField] private int comboMaxLength = 3;
    
    [Header("Debug")]
    public int comboCounter = 0;
    public WeaponType weaponType = WeaponType.Null;

    public List<WeaponType> currentComboWeaponTypes = new List<WeaponType>();
    
    public WeaponType nextType = WeaponType.Null;
    private bool canPerformCombo = true;
    
    private Animator _animator;
    
    #endregion

    #region Init

    private void Awake()
    {
        if (!TryGetComponent(out _animator)) return;
    }

    // Start is called before the first frame update
    private void Start()
    {
        AttachInputToAttack();
    }
    
    private void AttachInputToAttack()
    {
        SC_InputManager.instance.weaponA.performed += _ => Attack(WeaponType.WeaponA);
        SC_InputManager.instance.weaponB.performed += _ => Attack(WeaponType.WeaponB);
        SC_InputManager.instance.weaponC.performed += _ => Attack(WeaponType.WeaponC);
    }

    #endregion

    #region Functions

    private void Attack(WeaponType type)
    {
        
        if (canPerformCombo)
        {
            IncrementCombo(type);
            UpdateAnimator();
        }
        
    }

    private void UpdateAnimator()
    {
        _animator.SetInteger("Combo", comboCounter);
        _animator.SetInteger("Type", (int)weaponType);
    }
    
    /// <summary>
    /// Check if the current combo reach its max length.
    /// Else increment combo, switch the weapon type to current type and add this to a list.
    /// </summary>
    /// <param name="type"></param>
    private void IncrementCombo(WeaponType type)
    {
        
        // Reset Combo after reach its max length.
        if (comboCounter+1 > comboMaxLength)
        {
            ResetCombo();
        }
        
        // Increment combo, switch the weapon type to current type and add this to a list.
        comboCounter++;
        weaponType = type;
        currentComboWeaponTypes.Add(weaponType);
        
        // Debug Side
        print("Combo : " + comboCounter + " / Type : " + weaponType);
        foreach (var lasttype in currentComboWeaponTypes)
        {
            print(lasttype);
        }
        
    }

    private void ResetCombo()
    {
        comboCounter = 0;
        currentComboWeaponTypes.Clear();
    }
    
    // private void InputBuffering(WeaponType type)
    // {
    //     if (nextType == WeaponType.Null)
    //     {
    //         nextType = type;
    //     }
    // }
    //
    // private void LaunchInputBuffered()
    // {
    //     Attack(nextType);
    // }
    
    #endregion
    
}

public enum WeaponType
{
    
    Null,
    WeaponA,
    WeaponB,
    WeaponC

}
