using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_InventoryChildSkillTemplate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{

    #region Variables

    [BoxGroup("References")]
    public List<Image> idChildImages = new List<Image>();
    
    [BoxGroup("References"), PropertySpace(SpaceBefore = 5f)]
    public Image iconImage;
    
    [BoxGroup("References"), PropertySpace(SpaceBefore = 5f)]
    public Image outlineImage;
    
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public GameObject tooltipGO;
    
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI tooltipName;
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI tooltipDesc;
    
    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 15f)]
    public SO_ChildSkill childSkill;

    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 5f), ShowInInspector]
    public int IDChild
    {
        get => idChild;
        set
        {

            idChild = value;
            UpdateIDImages();
            
        }
    }
    private int idChild;

    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 10f)]
    public Color isIDColor;
    [BoxGroup("Settings")]
    public Color isNotIDColor;
    
    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 10f)]
    public Color isOwnedColor;
    [BoxGroup("Settings")]
    public Color isNotOwnedIDColor;
    

    #endregion

    public void Init(SO_ChildSkill newChildSkill, int id)
    {

        childSkill = newChildSkill;
        IDChild = id;
        
        UpdateOutlineColor();
        InitTooltip();

        if (childSkill.parentSkill.crystal == null) return;
        
        iconImage.sprite = childSkill.parentSkill.skillName == "Poison d'Ozamas" ? childSkill.parentSkill.crystalIcon : childSkill.parentSkill.crystal;
        iconImage.gameObject.SetActive(true);

    }
    
    public void InitTooltip()
    {

        tooltipName.text = childSkill.skillName;
        if(PlayerPrefs.HasKey("longDesc"))
            tooltipDesc.text = PlayerPrefs.GetInt("longDesc") == 1 ? childSkill.longDescription : childSkill.shortDescription;
        else
            tooltipDesc.text = childSkill.shortDescription;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipName.transform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipDesc.transform.parent);

    }

    public void ShowTooltip(bool value)
    {
        
        tooltipGO.SetActive(value);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipName.transform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipDesc.transform.parent);
        
    }
    
    public void UpdateIDImages()
    {
        
        if(idChildImages.Count < 1) return;

        idChildImages[idChild].color = isIDColor;

        foreach (var image in idChildImages.Where(image => idChildImages.IndexOf(image) != idChild))
        {
            image.color = isNotIDColor;
        }

    }
    
    public void UpdateOutlineColor()
    {
        
        if(childSkill == null) return;

        outlineImage.color = SC_GameManager.instance.playerSkillInventory.CheckHasSkill(childSkill)
            ? isOwnedColor
            : isNotOwnedIDColor;

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
