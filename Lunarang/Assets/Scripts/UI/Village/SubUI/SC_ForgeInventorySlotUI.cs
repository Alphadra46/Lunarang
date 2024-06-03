using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_ForgeInventorySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
{

    #region Variables
    
    public Color32 bgSelectedColor;
    public Color32 bgUnselectedColor;
    [PropertySpace(SpaceBefore = 5f)]
    public Color32 outlineSelectedColor;
    public Color32 outlineUnselectedColor;
    
    [PropertySpace(SpaceBefore = 20f)]
    public SC_Weapon weapon;

    [PropertySpace(SpaceBefore = 20f)]
    public Image bgImage;
    public Image outlineImage;
    public Image iconImage;
    
    private bool isSelected;
    private SC_ForgeUI _forgeUI;

    #endregion


    public void Init(SC_ForgeUI forgeUI, SC_Weapon newWeapon)
    {

        _forgeUI = forgeUI;
        weapon = newWeapon;
        
        if(weapon != null)
            iconImage.sprite = weapon.icon;

    }

    public void OnSelect(BaseEventData eventData)
    {
        _forgeUI.switchSelectedInventory?.Invoke(this, true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _forgeUI.switchSelectedInventory?.Invoke(this, false);
    }


    public void SwitchState(bool value)
    {
        
        isSelected = value;
        
        bgImage.color = isSelected ? bgSelectedColor : bgUnselectedColor;
        outlineImage.color = isSelected ? outlineSelectedColor : outlineUnselectedColor;
        
    }

    public void OnSubmit(BaseEventData eventData)
    {
        
        // _forgeUI.switchSelectedInventory?.Invoke(this, true);
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // _forgeUI.switchSelectedInventory?.Invoke(this, true);
    }
}
