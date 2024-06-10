using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_BuildingButton : Selectable
{
    
    [PropertySpace(SpaceBefore = 25f)]
    public GameObject tooltip;

    [PropertySpace(SpaceBefore = 15f)]
    public Image outlineImage;
    public Image buildingImage;

    [PropertySpace(SpaceBefore = 15f)]
    public SO_Building building;

    protected override void Awake()
    {
        base.Awake();

       UpdateSprite();
    }

    public void UpdateSprite()
    {
        
        if(building.spritesByLevel.Count > 0)
            buildingImage.sprite = building.spritesByLevel[building.currentLevel];
        if(building.outlinesByLevel.Count > 0)
            outlineImage.sprite = building.outlinesByLevel[building.currentLevel];
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();
        ShowTooltip(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        ShowTooltip(false);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        ShowTooltip(true);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        ShowTooltip(false);
    }


    private void ShowTooltip(bool value)
    {
        if(!interactable) return;
        
        SC_Lobby.currentBuilding?.Invoke(this, value);

        outlineImage.DOColor(value ? colors.highlightedColor : colors.normalColor, 0.25f);
        
        tooltip.SetActive(value);
        
    }
}
