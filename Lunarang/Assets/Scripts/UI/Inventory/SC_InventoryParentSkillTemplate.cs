using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_InventoryParentSkillTemplate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{

    #region Variables
    
    [BoxGroup("References"), PropertySpace(SpaceBefore = 5f)]
    public Image iconImage;
    
    [BoxGroup("References"), PropertySpace(SpaceBefore = 5f)]
    public Image outlineImage;
    
    [BoxGroup("References"), PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI nameTMP;
    
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public GameObject tooltipGO;
    
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI tooltipName;
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI tooltipDesc;
    
    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 15f)]
    public SO_ParentSkill parentSkill;
    
    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 10f)]
    public Color isOwnedColor;
    [BoxGroup("Settings")]
    public Color isNotOwnedIDColor;

    #endregion
    
    public void Init(SO_ParentSkill newParentSkill, string newName)
    {

        parentSkill = newParentSkill;

        nameTMP.text = newName;
        
        UpdateOutlineColor();
        InitTooltip();

        if (parentSkill.crystal == null) return;
        
        iconImage.sprite = parentSkill.skillName == "Poison d'Ozamas" ? parentSkill.crystalIcon : parentSkill.crystal;
        iconImage.gameObject.SetActive(true);
        
    }
    
    
    public void UpdateOutlineColor()
    {

        if (parentSkill == null) return;

        outlineImage.color = SC_GameManager.instance.playerSkillInventory.CheckHasSkill(parentSkill)
            ? isOwnedColor
            : isNotOwnedIDColor;
        

    }

    public void InitTooltip()
    {

        tooltipName.text = parentSkill.skillName;
        if(PlayerPrefs.HasKey("longDesc"))
            tooltipDesc.text = PlayerPrefs.GetInt("longDesc") == 1 ? parentSkill.longDescription : parentSkill.shortDescription;
        else
            tooltipDesc.text = parentSkill.shortDescription;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipName.transform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipDesc.transform.parent);

    }

    public void ShowTooltip(bool value)
    {
        
        tooltipGO.SetActive(value);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipName.transform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipDesc.transform.parent);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowTooltip(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShowTooltip(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ShowTooltip(false);
    }
}
