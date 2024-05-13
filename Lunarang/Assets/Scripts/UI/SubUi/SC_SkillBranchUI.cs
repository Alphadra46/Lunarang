using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = System.Object;

public class SC_SkillBranchUI : MonoBehaviour
{
    [HideInInspector] public SO_ParentSkill parentSkill;
    [HideInInspector] public List<SO_ChildSkill> childrenSkills = new List<SO_ChildSkill>();

    [SerializeField] private GameObject parentSkillSlot;
    [SerializeField] private List<GameObject> childrenSkillSlots = new List<GameObject>();

    private List<SC_SkillButton> skillsButtons = new List<SC_SkillButton>();
    
    private SC_ConstellationUI selectedConstellation;

    private Image background;
    
    public EventHandler updateIcons;

    public void SkillBranchSetup(SO_ParentSkill parentSkill, List<SO_ChildSkill> childrenSkills, SC_Constellation constellation)
    {
        background = GetComponent<Image>();
        this.parentSkill = parentSkill;
        this.childrenSkills = childrenSkills;
        updateIcons += UpdateBranchIcons;

        if (parentSkillSlot.TryGetComponent(out SC_SkillButton skillButton))
        {
            skillButton.InitTooltip(parentSkill, constellation, this);
            skillsButtons.Add(skillButton);
        }
        
        for (int i = 0; i < childrenSkillSlots.Count; i++)
        {
            var c = childrenSkillSlots[i].GetComponent<SC_SkillButton>();
            c.InitTooltip(childrenSkills[i], constellation, this);
            skillsButtons.Add(c);
        }
    }

    public void PanelSelection()
    {
        EventSystem.current.SetSelectedGameObject(parentSkillSlot);
        background.enabled = true;
    }

    public void PanelDeselection()
    {
        background.enabled = false;
    }
    
    private void UpdateBranchIcons(Object sender, EventArgs e)
    {
        Debug.Log("Updated");
        foreach (var skillButton in skillsButtons)
        {
            skillButton.SkillIcon();
        }
    }
    
}
