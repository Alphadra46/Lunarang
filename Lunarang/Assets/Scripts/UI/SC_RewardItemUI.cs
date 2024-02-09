using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_RewardItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
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

    public void OnClick()
    {
        
        SC_UIManager.instance.ShowRewardMenu();
        SC_GameManager.instance.SetPause();
        
        SC_SkillManager.instance.AddSkillsToSkillsList(SC_SkillManager.instance.FindSkillByName(title.text));
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        title.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        title.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
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

    public void OnSelect(BaseEventData eventData)
    {
        title.gameObject.SetActive(true);
        description.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        title.gameObject.SetActive(false);
        description.gameObject.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
    }
}
