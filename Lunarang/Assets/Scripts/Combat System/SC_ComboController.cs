using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SC_ComboController : MonoBehaviour
{
    #region Variables
    
    [Header("Settings")]
    [SerializeField] private int comboMaxLength = 3;
    
    [Header("Debug")]
    public int comboCounter = 0;
    public WeaponType weaponType = WeaponType.WeaponA;

    public List<WeaponType> currentComboWeaponTypes = new List<WeaponType>();

    
    
    #endregion

    #region Init
    
    // Start is called before the first frame update
    private void Start()
    {
        SC_InputManager.instance.weaponA.performed += _ => IncrementCombo(WeaponType.WeaponA);
        SC_InputManager.instance.weaponB.performed += _ => IncrementCombo(WeaponType.WeaponB);
        SC_InputManager.instance.weaponC.performed += _ => IncrementCombo(WeaponType.WeaponC);
    }

    #endregion

    #region Functions
    
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

    #endregion
    
}

public enum WeaponType
{
    
    WeaponA,
    WeaponB,
    WeaponC

}
