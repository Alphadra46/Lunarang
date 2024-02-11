using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_InventoryItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    #region Variables

    public Button button;
    private VerticalLayoutGroup _layoutGroupComponent;
    
    public Image image;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    
    #endregion

    private void Awake()
    {
        if(!TryGetComponent(out _layoutGroupComponent)) return;
        if(!TryGetComponent(out button)) return;
        
        StartCoroutine(DelayInput());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.05f,1.05f,1.05f);
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f,1f,1f);
        // description.gameObject.SetActive(false);
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1.05f,1.05f,1.05f);
        // description.gameObject.SetActive(true);
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1f,1f,1f);
        // description.gameObject.SetActive(false);
        // LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }
    
    
    public void SetTitle(string newTitle)
    {
        title.text = newTitle;
    }
    
    public void SetImage(Sprite newSprite)
    {
        image.sprite = newSprite;
    }
    
    public void SetDescription(string newDescription)
    {
        description.text = newDescription;
    }
 
    IEnumerator DelayInput()
    {
        
        yield return new WaitForEndOfFrame();

        button.interactable = true;

    }
}
