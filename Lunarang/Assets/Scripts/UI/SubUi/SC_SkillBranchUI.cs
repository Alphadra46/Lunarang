using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SC_SkillBranchUI : MonoBehaviour
{
    [HideInInspector] public SO_ParentSkill parentSkill;
    [HideInInspector] public List<SO_ChildSkill> childrenSkills = new List<SO_ChildSkill>();

    [SerializeField] private GameObject parentSkillSlot;
    [SerializeField] private List<GameObject> childrenSkillSlots = new List<GameObject>();

    private SC_ConstellationUI selectedConstellation;

    public void SkillBranchSetup(SO_ParentSkill parentSkill, List<SO_ChildSkill> childrenSkills)
    {
        this.parentSkill = parentSkill;
        this.childrenSkills = childrenSkills;

        if (parentSkillSlot.TryGetComponent(out SC_SkillButton skillButton))
            skillButton.InitTooltip(parentSkill);
        
        for (int i = 0; i < childrenSkillSlots.Count; i++)
        {
            childrenSkillSlots[i].GetComponent<SC_SkillButton>().InitTooltip(childrenSkills[i]);
        }
    }

}
