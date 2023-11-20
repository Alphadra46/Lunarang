using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_ComboController : MonoBehaviour
{
    
    #region Variables
    
    [Title("Settings")]

    #region Combos

    [TabGroup("Settings", "Combo")]
    [SerializeField] private int comboMaxLength = 3;
    
    [TabGroup("Settings", "Combo")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
    public int comboCounter = 0;
    

    #endregion


    #region Weapons

    [TabGroup("Settings", "Weapon"), ShowInInspector, ReadOnly]
    public SC_Weapon currentWeapon;
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5)]
    public List<SC_Weapon> equippedWeapons;

    #endregion


    #region Types & Parameters

    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5), ReadOnly]
    public WeaponType currentType;
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<WeaponType> currentComboWeaponTypes = new List<WeaponType>();
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<ParameterType> currentComboParameters;

    #endregion


    #region Input Buffering

    [TabGroup("Settings", "Combo")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
    public SC_Weapon inputBufferedWeapon;
    
    [TabGroup("Settings", "Combo")]
    [SerializeField]private bool canPerformCombo = true;
    private bool isInputBufferingOn = false;

    #endregion
    
    private Animator _animator;
    private SC_PlayerController _controller;
    
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
        SC_InputManager.instance.weaponA.performed += _ => Attack(equippedWeapons[0]);
        SC_InputManager.instance.weaponB.performed += _ => Attack(equippedWeapons[1]);
        SC_InputManager.instance.weaponC.performed += _ => Attack(equippedWeapons[2]);
    }

    #endregion

    #region Functions

    private void Attack(SC_Weapon usedWeapon)
    {
        
        // if(_controller.isDashing) return;
        
        if (canPerformCombo)
        {
            IncrementCombo(usedWeapon);
            UpdateAnimator();
        }
        else if(isInputBufferingOn)
        {
            InputBuffering(currentWeapon);
        }
        
    }

    private void UpdateAnimator()
    {
        _animator.SetInteger("Combo", comboCounter);
        if(currentWeapon != null)
            _animator.SetInteger("Type", (int)currentWeapon.type);
    }

    #region Combo Part

    private void CanPerformCombo()
    {
        canPerformCombo = true;

        if (inputBufferedWeapon == null) return;
        
        Attack(inputBufferedWeapon);
        inputBufferedWeapon = null;

    }
    private void CantPerformCombo()
    {
        canPerformCombo = false;
    }

    /// <summary>
    /// Check if the current combo reach its max length.
    /// Else increment combo, switch the weapon type to current type and add this to a list.
    /// </summary>
    /// <param name="newWeapon"></param>
    private void IncrementCombo(SC_Weapon newWeapon)
    {
        
        // Reset Combo after reach its max length.
        if (comboCounter+1 > comboMaxLength)
        {
            ResetCombo();
        }
        
        // Increment combo, switch the weapon type to current type and add this to a list.
        comboCounter++;
        currentWeapon = newWeapon;
        currentType = newWeapon.type;
        currentComboWeaponTypes.Add(currentWeapon.type);
        currentComboParameters.Add(currentWeapon.parameter);
        
        // Debug Side
        print("Combo : " + comboCounter + " / Type : " + currentWeapon.type);
        foreach (var lasttype in currentComboWeaponTypes)
        {
            print(lasttype);
        }
        
    }

    private void ResetCombo()
    {
        comboCounter = 0;
        currentType = WeaponType.Null;
        currentComboWeaponTypes.Clear();
        currentComboParameters.Clear();
        UpdateAnimator();
    }

    #endregion

    #region Input Buffering
    
    public void ActivateInputBuffering()
    {
        isInputBufferingOn = true;
        print("Buffering On");
    }
    
    public void DesactivateInputBuffering()
    {
        isInputBufferingOn = false;
        print("Buffering Off");
    }
    
    private void InputBuffering(SC_Weapon nextWeapon)
    {
        if (inputBufferedWeapon == null) return;
        
        inputBufferedWeapon = nextWeapon;
        print("Buffered : " + inputBufferedWeapon);
    }
    
    #endregion
    
    #endregion
    
}


