using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_RewardItemUI : SerializedMonoBehaviour
{
    #region Variables
    private VerticalLayoutGroup _layoutGroupComponent;

    public Dictionary<string, Color32> baseColors = new Dictionary<string, Color32>();
    
    #region References
    [BoxGroup("References")]
    public Button button;
    [BoxGroup("References")]
    public Image outline;
    [BoxGroup("References")]
    public Image crystal;
    [BoxGroup("References")]
    public Image icon;
    [BoxGroup("References")]
    public TextMeshProUGUI title;
    [BoxGroup("References")]
    public TextMeshProUGUI description;
    #endregion
    
    private SC_Ressource ressource;
    private SO_BaseSkill skill;
    
    private int amount;
    
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
        
        if(skill != null)
            SC_GameManager.instance.playerSkillInventory.AddSkill(skill);
        else
        {
            SC_GameManager.instance.playerResourceInventory.AddResource(ressource, amount);
        }
        
    }

    public void Init(SC_Ressource newRessource, int newAmount)
    {

        ressource = newRessource;

        SetTitle(ressource.name);
        SetImage(ressource.sprite);
        SetDescription(ressource.description);
        SetAmount(newAmount);
        SetDefaultColor();
        
    }
    
    public void Init(SO_BaseSkill newSkill)
    {
        
        skill = newSkill;
        
        
        SetTitle(skill.skillName);
        SetDescription(skill.shortDescription);
        SetColor(skill.constellation);
        if (skill.constellation != ConstellationName.Lunar) return;
        
        var lunarSkill = (SO_LunarSkill) newSkill;
        SetIcon(lunarSkill.lunarIcon);

    }
    
    private void SetTitle(string newTitle)
    {
        title.text = newTitle;
    }
    
    private void SetImage(Sprite newSprite)
    {
        crystal.sprite = newSprite;
    }
    
    private void SetIcon(Sprite newIcon)
    {
        icon.gameObject.SetActive(true);
        icon.sprite = newIcon;
    }
    
    private void SetDescription(string newDescription)
    {
        description.text = newDescription;
    }

    private void SetColor(ConstellationName constellationName)
    {
        switch (constellationName)
        {
            case ConstellationName.Lunar:
                outline.color = baseColors["Lunar"];
                crystal.color = baseColors["Lunar"];
                
                var crystalColor = crystal.color;
                crystalColor.a = 0.75f;
                crystal.color = crystalColor;
                break;
            case ConstellationName.DoT:
                outline.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("DoT").color;
                crystal.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("DoT").color;
                break;
            case ConstellationName.Berserker:
                outline.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("Berserk").color;
                crystal.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("Berserk").color;
                break;
            case ConstellationName.Tank:
                outline.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("Tank").color;
                crystal.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("Tank").color;
                break;
            case ConstellationName.Freeze:
                outline.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("Freeze").color;
                crystal.color = SC_GameManager.instance.playerSkillInventory.FindConstellationByName("Freeze").color;
                break;
            default:
                outline.color = baseColors["Default"];
                crystal.color = baseColors["Default"];
                break;
        }
    }
    
    private void SetDefaultColor()
    {
        outline.color = baseColors["Default"];
        crystal.color = baseColors["Default"];
    }

    private void SetAmount(int newAmount)
    {
        amount = newAmount;
    }
    
    IEnumerator DelayInput()
    {
        
        yield return new WaitForEndOfFrame();

        button.interactable = true;

    }
}
