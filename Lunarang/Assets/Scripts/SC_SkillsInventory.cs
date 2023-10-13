using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Skill Inventory")]
public class SC_SkillsInventory : ScriptableObject
{
    public List<SC_Skill> skillInventory = new List<SC_Skill>();
    
    public void AddSkill(SC_Skill skillToAdd)
    {
        skillInventory.Add(skillToAdd);

        if (skillToAdd.skillType == SC_Skill.SkillTypeEnum.Passive)
        {
            var passiveSkill = (SC_PassiveSkills)skillToAdd;
            passiveSkill.ApplyPassiveSkillEffect();
        }
        else
        {
            Debug.Log("TODO - Active Skills");
        }

    }
}
