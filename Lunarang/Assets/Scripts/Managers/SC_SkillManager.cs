using System;
using System.Collections.Generic;
using System.Linq;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_SkillManager : MonoBehaviour
{
    public static SC_SkillManager instance;
    
    [ShowInInspector] public List<SO_BaseSkill> allSkills = new List<SO_BaseSkill>();
    [ShowInInspector] public List<SO_BaseSkill> allCurrentRunSkills = new List<SO_BaseSkill>();
    public List<SO_BaseSkill> allEquippedSkills = new List<SO_BaseSkill>();

    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;

        foreach (var skill in allSkills.Where(skill => skill.GetType() == typeof(SO_ParentSkill)))
        {
            allCurrentRunSkills.Add(skill);
        }
    }

    public bool CheckHasSkillByName(string skillName)
    {
        return allEquippedSkills.Contains(FindSkillByName(skillName));
    }
    
    public bool CheckHasSkill(SO_BaseSkill skill)
    {
        return allEquippedSkills.Contains(skill);
    }
    
    public SO_BaseSkill FindSkillByName(string skillName)
    {
        return allSkills.FirstOrDefault(skill => skill.skillName == skillName);
    }
    
    public SO_ChildSkill FindChildSkillByName(string childSkillName)
    {
        return (SO_ChildSkill) allSkills.FirstOrDefault(skill => skill.skillName == childSkillName);
    }
    
    public void AddSkillsToSkillsList(SO_BaseSkill newSkill)
    {
        if(allEquippedSkills.Contains(newSkill) && newSkill.constellation != ConstellationName.Lunar) return;
        
        if (newSkill.skillName == "ChildSkill_3_4_Berserk" && SC_PlayerStats.instance.inManaFury)
        {
            SC_PlayerStats.instance.debuffsBuffsComponent.RemoveBuff(Enum_Buff.ManaFury);
            allEquippedSkills.Add(newSkill);
            newSkill.Init();
            SC_PlayerStats.instance.debuffsBuffsComponent.ApplyBuff(Enum_Buff.ManaFury);
            return;
        }
        
        allEquippedSkills.Add(newSkill);
        newSkill.Init();

       
       
    }
    
}
