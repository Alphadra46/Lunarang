using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SC_InventoryLunarPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    
    public void OnEnable()
    {
        
        
        
    }

    public void ResizeAllSkill(bool value)
    {

        for (var i = 2; i < transform.childCount; i++)
        {

            
            if(!transform.GetChild(i).TryGetComponent(out RectTransform rect)) return;
            
            rect.sizeDelta = new Vector2(85, value ? 100 : 50);

            if(!transform.GetChild(i).TryGetComponent(out SC_InventoryLunarSkillTemplate sc)) return;
            
            if(!sc.iconImage.gameObject.TryGetComponent(out RectTransform icon)) return;

            icon.sizeDelta = new Vector2(value ? 64 : 40, value ? 64 : 40);
            
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ResizeAllSkill(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResizeAllSkill(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        ResizeAllSkill(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ResizeAllSkill(false);
    }
}
