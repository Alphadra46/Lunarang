using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Skill/Skill Template")]
public class SC_Skill : ScriptableObject
{
    public int level = 0;
    [HideInInspector] public int maxLevel = 5;
    public enum SkillTypeEnum
    {
        Active,
        Passive
    }

    public SkillTypeEnum skillType;
    
    public void ReinforceSkill()
    {
        level++;

        if (skillType == SkillTypeEnum.Passive)
        {
            var passiveSkill = (SC_PassiveSkills)this;
            passiveSkill.ApplyPassiveSkillEffect();
        }
    }
}
