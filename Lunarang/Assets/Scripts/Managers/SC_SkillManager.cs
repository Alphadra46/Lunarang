using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_SkillManager : MonoBehaviour
{
    
    [ShowInInspector] public List<SO_BaseSkill> allSkills = new List<SO_BaseSkill>();
    public List<SO_BaseSkill> allEquippedSkills = new List<SO_BaseSkill>();


    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddSkillsToSkillsList(FindSkillByName("Viper's Bite"));
        }
        
    }

    public SO_BaseSkill FindSkillByName(string skillName)
    {
        return allSkills.FirstOrDefault(skill => skill.skillName == skillName);
    }
    
    public void AddSkillsToSkillsList(SO_BaseSkill newSkill)
    {
        
       allEquippedSkills.Add(newSkill);
       newSkill.Init();
        
    }
    
}
