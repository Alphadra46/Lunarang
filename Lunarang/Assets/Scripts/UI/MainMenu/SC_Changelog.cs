using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SC_Changelog : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
{

    public Color32 selectColor;
    public Color32 normalColor;

    public TextMeshProUGUI versionText;

    public GameObject prompt;

    public bool isInteractable;


    public void SwitchMode(bool value)
    {
        
        prompt.SetActive(value);
        versionText.color = value ? selectColor : normalColor;

    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!isInteractable) return;
        SwitchMode(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!isInteractable) return;
        SwitchMode(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if(!isInteractable) return;
        SwitchMode(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if(!isInteractable) return;
        SwitchMode(false);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if(!isInteractable) return;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!isInteractable) return;
    }
}
