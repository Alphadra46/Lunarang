using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

public class SC_ComboController : MonoBehaviour
{
    
    #region Variables

    public static SC_ComboController instance;
    
    [Title("Settings")]

    #region Combos

    [TabGroup("Settings", "Combo")]
    [SerializeField] private int comboMaxLength = 3;
    
    [TabGroup("Settings", "Combo")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
    public int comboCounter = 0;

    #endregion


    #region Weapons
    
    [TabGroup("Settings", "Weapon")]
    public List<SC_Weapon> equippedWeapons;
    [PropertySpace(SpaceAfter = 5)]
    [TabGroup("Settings", "Weapon"), ShowInInspector, ReadOnly]
    public SC_Weapon currentWeapon;

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
    
    [TabGroup("Settings", "Weapon")]
    [PropertySpace(SpaceAfter = 5), ReadOnly]
    public List<SC_Weapon> currentComboWeapons;

    #endregion


    #region Input Buffering

    [TabGroup("Settings", "Combo")]
    [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
    public SC_Weapon inputBufferedWeapon;
    
    [TabGroup("Settings", "Combo")]
    [SerializeField]private bool canPerformCombo = true;
    private bool isInputBufferingOn = false;

    #endregion
    
    public Animator _animator;
    private SC_PlayerController _controller;
    private SC_FinalATK_Builder _finalBuilder;
    [SerializeField] private List<VisualEffect> vfxParameterList = new List<VisualEffect>();

    public bool canAttack = true;
    
    #endregion

    #region Init

    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
        
        if(!TryGetComponent(out _controller)) return;
        if(!TryGetComponent(out _finalBuilder)) return;
    }

    private void Start()
    {
        AttachInputToAttack();
    }
    
    /// <summary>
    /// Attach inputs to functions
    /// </summary>
    public void AttachInputToAttack()
    {
        SC_InputManager.instance.weaponA.performed += _ => Attack(equippedWeapons[0]);
        SC_InputManager.instance.weaponB.performed += _ => Attack(equippedWeapons[1]);
        SC_InputManager.instance.weaponC.performed += _ => Attack(equippedWeapons[2]);
    }
    
    public void DettachInputToAttack()
    {
        SC_InputManager.instance.weaponA.performed -= _ => Attack(equippedWeapons[0]);
        SC_InputManager.instance.weaponB.performed -= _ => Attack(equippedWeapons[1]);
        SC_InputManager.instance.weaponC.performed -= _ => Attack(equippedWeapons[2]);
    }

    #endregion

    #region Functions
    
    /// <summary>
    /// Perform an attack and stack it in a combo counter.
    /// Update the animator and play the animation.
    /// Stock an input if already performing an attack.
    /// </summary>
    /// <param name="usedWeapon">Weapon used in this attack</param>
    private void Attack(SC_Weapon usedWeapon)
    {
        
        if(SC_GameManager.instance.isPause || !canAttack) return;
        
        
        if (canPerformCombo)
        {
            canAttack = false;
            IncrementCombo(usedWeapon);
            UpdateAnimator();
            
            _controller.FreezeMovement(true);

        }
        // else if(isInputBufferingOn)
        // {
        //     InputBuffering(currentWeapon);
        // }
        
        
        
    }

    /// <summary>
    /// Resend all values to the animator.
    /// </summary>
    private void UpdateAnimator()
    {
        
        _animator.SetInteger("Combo", comboCounter);
        if (currentWeapon != null)
            _animator.SetTrigger(currentWeapon.id);
        
        if (currentComboParameters.Count > 0)
        {
            _animator.SetInteger("Parameter_1", (int) currentComboParameters[0]);
        }
        else if (currentComboParameters.Count > 1)
        {
            _animator.SetInteger("Parameter_1", (int) currentComboParameters[0]);
            _animator.SetInteger("Parameter_2", (int) currentComboParameters[1]);
        }
        
    }

    #region Combo Part
    
    /// <summary>
    /// Set combo to performable.
    /// </summary>
    public void CanPerformCombo()
    {
        canAttack = true;
        canPerformCombo = true;

        // if (inputBufferedWeapon == null) return;
        //
        // Attack(inputBufferedWeapon);
        // inputBufferedWeapon = null;

    }
    
    /// <summary>
    /// Set combo to not performable.
    /// </summary>
    public void CantPerformCombo()
    {
        canPerformCombo = false;
    }

    /// <summary>
    /// Check if the current combo reach its max length.
    /// Else increment combo, switch the weapon type to current type and add this to a list.
    /// Add parameters of the current combo to a list.
    /// </summary>
    /// <param name="newWeapon">New weapon to add to the current combo list</param>
    private void IncrementCombo(SC_Weapon newWeapon)
    {
        
        // Reset Combo after reach its max length.
        if (comboCounter+1 > comboMaxLength)
        {
            ResetCombo();
            // _finalBuilder.Reset();
        }
        
        // Increment combo, switch the weapon type to current type and add this to a list.
        comboCounter++;
        currentWeapon = newWeapon;
        currentType = newWeapon.type;
        currentComboWeaponTypes.Add(currentWeapon.type);
        currentComboParameters.Add(currentWeapon.parameter);
        currentComboWeapons.Add(currentWeapon);
        
        //Only spawn 1 VFX depending on the parameter of the hit
        if (comboCounter<=2)
        {
            switch (newWeapon.parameter)
            {
                case ParameterType.MultiHit:
                    vfxParameterList[0].Play();
                    break;
                case ParameterType.AreaOfEffect:
                    vfxParameterList[1].Play();
                    break;
                case ParameterType.Projectile:
                    vfxParameterList[2].Play();
                    break;
                default:
                    break;
            }
        }
        else //Spawn VFX depending on the current combo parameters for the finisher
        {
            foreach (var comboParameter in currentComboParameters)
            {
                switch (comboParameter)
                {
                    case ParameterType.MultiHit:
                        vfxParameterList[0].Play();
                        break;
                    case ParameterType.AreaOfEffect:
                        vfxParameterList[1].Play();
                        break;
                    case ParameterType.Projectile:
                        vfxParameterList[2].Play();
                        break;
                    default:
                        break;
                }
            }

        }
        
        // if(comboCounter == 3) _finalBuilder.GetInfosFromLastAttacks(currentComboWeapons);
        
        
        // Debug Side
        print("Combo : " + comboCounter + " / Type : " + currentWeapon.type);
        foreach (var lasttype in currentComboWeaponTypes)
        {
            print(lasttype);
        }
        
    }

    /// <summary>
    /// Reset the current combo and its parameters.
    /// </summary>
    public void ResetCombo()
    {
        comboCounter = 0;
        currentWeapon = null;
        currentType = WeaponType.Null;
        currentComboWeaponTypes.Clear();
        currentComboParameters.Clear();
        currentComboWeapons.Clear();
        UpdateAnimator();
    }

    #endregion

    #region Input Buffering
    
    /// <summary>
    /// Activate the possibility to do stock an input.
    /// </summary>
    public void ActivateInputBuffering()
    {
        isInputBufferingOn = true;
        print("Buffering On");
    }
    
    /// <summary>
    /// Deactivate the possibility to do stock an input.
    /// </summary>
    public void DeactivateInputBuffering()
    {
        isInputBufferingOn = false;
        print("Buffering Off");
    }
    
    /// <summary>
    /// Do the stocked input.
    /// </summary>
    private void InputBuffering(SC_Weapon nextWeapon)
    {
        if (inputBufferedWeapon == null) return;
        
        inputBufferedWeapon = nextWeapon;
        print("Buffered : " + inputBufferedWeapon);
    }
    
    #endregion
    
    #endregion
    
}


