using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_ForgeInventoryTypeButton : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler 
{

    #region Variables

    public Image bgImage;
    public TextMeshProUGUI nameTMP;
    
    [PropertySpace(SpaceBefore = 5f)]
    public Color32 bgSelectedColor;
    public Color32 bgUnselectedColor;
    [PropertySpace(SpaceBefore = 5f)]
    public Color32 nameTMPSelectedColor;
    public Color32 nameTMPUnselectedColor;

    [PropertySpace(SpaceBefore = 5f)]
    public ParameterType type;
    
    private bool isSelected = false;
    public SC_ForgeUI _forgeUI;

    #endregion

    public void OnPointerClick(PointerEventData eventData)
    {
        _forgeUI.switchInventoryType?.Invoke(this, true, type);
    }

    public void OnSelect(BaseEventData eventData)
    {
        _forgeUI.switchInventoryType?.Invoke(this, true, type);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _forgeUI.switchInventoryType?.Invoke(this, false, type);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        _forgeUI.switchInventoryType?.Invoke(this, false, type);
    }

    public void SwitchState(bool value)
    {

        isSelected = value;

        bgImage.color = isSelected ? bgSelectedColor : bgUnselectedColor;
        nameTMP.fontMaterial.color = isSelected ? nameTMPSelectedColor : nameTMPUnselectedColor;
        

    }
    
}
