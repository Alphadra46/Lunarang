using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class SC_SkillButton : MonoBehaviour
{
    [HideInInspector] public GameObject tooltip;

    [SerializeField] private Sprite unknownSkill;
    [SerializeField] private Sprite usableSkill;
    [SerializeField] private Sprite equippedSkill;

    private SO_SkillInventory inventory;
    private Image image;

    private SO_BaseSkill skill;
    private SC_Constellation constellation;
    [HideInInspector] public bool isParentSkill;
    
    private bool isSkillKnown;
    private SC_SkillBranchUI branch;
    
    private void Start()
    {
        tooltip = transform.GetChild(0).gameObject;
        image = gameObject.GetComponent<Image>();
        SC_InputManager.instance.develop.started += ToggleTooltip;
        inventory = Resources.Load<SO_SkillInventory>("SkillInventory");
    }

    public void InitTooltip(SO_BaseSkill skill, SC_Constellation constellation, SC_SkillBranchUI branch)
    {
        var nameText = tooltip.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        var descText = tooltip.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        var costText = tooltip.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

        nameText.text = skill.skillName;
        descText.text = skill.shortDescription;
        costText.text = skill.spCost.ToString();

        this.skill = skill;
        isSkillKnown = skill.isKnown;

        this.constellation = constellation;

        this.branch = branch;
        SkillIcon();
    }

    public void OnPress()
    {
        if (!isSkillKnown)
            return;
        
        Debug.Log("Pressed");
        
        if (inventory.CheckHasSkill(skill))
        {
            //Remove
            if (skill as SO_ParentSkill != null)
            {
                var s = (SO_ParentSkill)skill;
                foreach (var c in s.childrenSkills)
                {
                    if (inventory.skillsOwned.Contains(c))
                    {
                        inventory.RemoveSkill(c);
                    }
                }
            }
            
            inventory.RemoveSkill(skill);
            
        }
        else
        {
            //Add
            if (skill as SO_ChildSkill != null) //if child skill
            {
                var s = (SO_ChildSkill)skill;
                inventory.AddSkill(s.parentSkill);
            }
            inventory.AddSkill(skill);
        }

        branch.updateIcons?.Invoke(this,null);
    }
    
    public void SkillIcon()
    {
        image.sprite = !isSkillKnown ? unknownSkill :
            !inventory.CheckHasSkill(skill) ? usableSkill : equippedSkill;
    }
    
    private void ToggleTooltip(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
            return;
        
        if (tooltip.activeSelf)
            HideTooltip();
        else
            ShowTooltip();
    } 
    
    public void ShowTooltip()
    {
        tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
    
}
