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
    
    private bool isSelected;

    #endregion


    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        SwitchState();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        SwitchState();
    }


    private void SwitchState()
    {

        bgImage.color = isSelected ? bgSelectedColor : bgUnselectedColor;
        outlineImage.color = isSelected ? outlineSelectedColor : outlineUnselectedColor;
        
    }

    public void OnSubmit(BaseEventData eventData)
    {
        
        isSelected = true;
        SwitchState();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = true;
        SwitchState();
    }
}
