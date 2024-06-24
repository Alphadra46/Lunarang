using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
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

    private List<GameObject> upgradeCostsGO = new List<GameObject>();
    
    [PropertySpace(SpaceBefore = 25f)] public GameObject upgradeCostPrefab;
    
    [PropertySpace(SpaceBefore = 10f)] public Transform upgradeCostTransform;
    
    [PropertySpace(SpaceBefore = 25f)] public SC_InputPrompt upgradePrompt;
    [PropertySpace(SpaceBefore = 25f)] public SC_InputPrompt interactPrompt;

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
        
        SC_Lobby.currentBuilding?.Invoke(this, value);

        outlineImage.DOColor(value ? colors.highlightedColor : colors.normalColor, 0.25f);
        
        tooltip.SetActive(value);

        interactPrompt.SetInteractable(interactable);
        
        if(building.currentLevel != building.maxLevel)
            upgradePrompt.SetInteractable(SC_GameManager.instance.playerResourceInventory.CheckHasResources(
                building.levelUpCosts[building.currentLevel + 1]));
        else
        {
            upgradePrompt.SetInteractable(false);
        }
        
        UpdateUpgradeCosts();
        RefreshUI();

    }

    public void UpdateUpgradeCosts()
    {

        if(building.currentLevel == building.maxLevel)
        {
            upgradeCostTransform.gameObject.SetActive(false);
            return;
        }
        
        ClearAllCosts();
        
        foreach (var cost in building.levelUpCosts[building.currentLevel+1])
        {
            var costGO = Instantiate(upgradeCostPrefab, upgradeCostTransform);
            if(costGO.TryGetComponent(out SC_UpgradeCostText sc))
                sc.Init(cost.Key, cost.Value);
            
            upgradeCostsGO.Add(costGO);
        }
        
        RefreshUI();
        
    }

    private void ClearAllCosts()
    {

        foreach (var costGO in upgradeCostsGO)
        {
            Destroy(costGO);
        }
        
        upgradeCostsGO.Clear();
        
        RefreshUI();
        
    }


    private void RefreshUI()
    {
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(upgradeCostTransform.GetComponent<RectTransform>());
        
    }
}
