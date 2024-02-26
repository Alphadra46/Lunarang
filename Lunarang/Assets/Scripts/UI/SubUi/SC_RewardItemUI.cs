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

    public Image background;
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
        transform.localScale = new Vector3(1.05f,1.05f,1.05f);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f,1f,1f);
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1.05f,1.05f,1.05f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1f,1f,1f);
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

    public void SetColor(ConstellationName constellationName)
    {
        switch (constellationName)
        {
            case ConstellationName.Lunar:
                background.color = new Color32(113, 52, 235, 255);
                break;
            case ConstellationName.DoT:
                background.color = new Color32(75, 204, 78, 255);
                break;
            case ConstellationName.Berserker:
                background.color = new Color32(204, 75, 75, 255);
                break;
            case ConstellationName.Tank:
                background.color = new Color32(82, 79, 77, 255);
                break;
            case ConstellationName.Freeze:
                break;
            default:
                background.color = new Color32(100, 100, 100, 255);
                break;
        }
    }
 
    IEnumerator DelayInput()
    {
        
        yield return new WaitForEndOfFrame();

        button.interactable = true;

    }
}
