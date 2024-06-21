using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_InventoryLunarSkillTemplate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{

    #region Variables
    
    
    [BoxGroup("References"), PropertySpace(SpaceBefore = 5f)]
    public Image iconImage;
    
    [BoxGroup("References"), PropertySpace(SpaceBefore = 5f)]
    public Image backgroundImage;
    
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public GameObject tooltipGO;
    
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI tooltipName;
    [BoxGroup("References/Tooltip"), PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI tooltipDesc;
    
    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 15f)]
    public SO_LunarSkill lunarSkill;
    
    
    [BoxGroup("Settings"), PropertySpace(SpaceBefore = 10f)]
    public Color isOwnedColor;
    [BoxGroup("Settings")]
    public Color isNotOwnedIDColor;
    

    #endregion

    public void Init(SO_LunarSkill newLunarSkill)
    {

        lunarSkill = newLunarSkill;
        
        UpdateBackgoundColor();
        InitTooltip();

        if (lunarSkill.lunarIcon == null) return;
        
        iconImage.sprite = lunarSkill.lunarIcon;
        iconImage.gameObject.SetActive(true);

    }
    
    public void InitTooltip()
    {

        tooltipName.text = lunarSkill.skillName;
        if(PlayerPrefs.HasKey("longDesc"))
            tooltipDesc.text = PlayerPrefs.GetInt("longDesc") == 1 ? lunarSkill.longDescription : lunarSkill.shortDescription;
        else
            tooltipDesc.text = lunarSkill.shortDescription;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipName.transform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipDesc.transform.parent);

    }

    public void ShowTooltip(bool value)
    {
        
        tooltipGO.SetActive(value);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipName.transform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) tooltipDesc.transform.parent);
        
    }

    public void ResizeSkill(bool value)
    {
        
        

    }
    
    public void UpdateBackgoundColor()
    {
        
        if(lunarSkill == null) return;

        backgroundImage.color = SC_GameManager.instance.playerSkillInventory.CheckHasSkill(lunarSkill)
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
