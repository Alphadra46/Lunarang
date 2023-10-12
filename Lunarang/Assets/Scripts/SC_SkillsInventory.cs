using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Skill Inventory")]
public class SC_SkillsInventory : ScriptableObject
{
    //Temp - GameObject will be replaced by the skill class
    public List<SC_Skill> skillInventory = new List<SC_Skill>();

    //TODO when skill class is created
    public void AddSkill(SC_Skill skillToAdd)
    {
        skillInventory.Add(skillToAdd);

        if (skillToAdd.skillType == SC_Skill.SkillTypeEnum.Passive)
            ApplyPassiveSkillEffect((SC_PassiveSkills)skillToAdd);
        else
        {
            Debug.Log("TODO - Active Skills");
        }
        
    }

    private void ApplyPassiveSkillEffect(SC_PassiveSkills skill)
    {
        switch (skill.affectedStat)
        {
            case SC_PassiveSkills.AffectedStat.Attack:
                SC_PlayerStats.instance.attackModifier += skill.modifierAmount;
                break;
            case SC_PassiveSkills.AffectedStat.Crit:
                SC_PlayerStats.instance.critRate += skill.modifierAmount;
                break;
            case SC_PassiveSkills.AffectedStat.Health:
                Debug.Log("TODO - Health modifier");
                break;
            case SC_PassiveSkills.AffectedStat.Speed:
                SC_PlayerStats.instance.speedModifier += skill.modifierAmount;
                break;
            case SC_PassiveSkills.AffectedStat.None:
                Debug.Log("TODO - Skill effect (boolean)");
                break;
            default:
                break;
        }
        
        
    }
    
}
