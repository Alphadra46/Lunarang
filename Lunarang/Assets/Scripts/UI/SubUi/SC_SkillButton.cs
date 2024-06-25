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
    private GameObject selectionCircle;

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
        
    }

    private void OnEnable()
    {
        selectionCircle = transform.GetChild(1).gameObject;
        tooltip = transform.GetChild(0).gameObject;
        image = gameObject.GetComponent<Image>();
        inventory = Resources.Load<SO_SkillInventory>("SkillInventory");
    }

    private void OnDisable()
    {
        HideTooltip();
    }

    public void InitTooltip(SO_BaseSkill skill, SC_Constellation constellation, SC_SkillBranchUI branch)
    {
        if (tooltip==null)
        {
            tooltip = transform.GetChild(0).gameObject;
        }
        
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
                        SC_SkillTreeUI.updateSP?.Invoke(s.spCost);
                        print("1");
                    }
                }
            }
            inventory.RemoveSkill(skill);
            SC_SkillTreeUI.updateSP?.Invoke(skill.spCost);
        }
        else
        {
            if (SC_SkillTreeUI.instance.currentSPLeft-skill.spCost>=0)
            {
                //Add
                if (skill as SO_ChildSkill != null) //if child skill
                {
                    var s = (SO_ChildSkill)skill;
                    if (!inventory.skillsOwned.Contains(s.parentSkill))
                    {
                        inventory.AddSkill(s.parentSkill);
                        SC_SkillTreeUI.updateSP?.Invoke(-s.spCost);
                        print("2");
                    }
                }
                inventory.AddSkill(skill);
                SC_SkillTreeUI.updateSP?.Invoke(-skill.spCost);
            }
        }

        branch.updateIcons?.Invoke(this,null);
    }
    
    public void SkillIcon()
    {
        image.sprite = !isSkillKnown ? unknownSkill :
            !inventory.CheckHasSkill(skill) ? usableSkill : equippedSkill;
    }
    
    
    public void ShowTooltip()
    {
        selectionCircle.SetActive(true);
        if (!isSkillKnown)
            return;
        
        tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        selectionCircle.SetActive(false);
        if (!isSkillKnown)
            return;
        
        tooltip.SetActive(false);
    }
    
}
