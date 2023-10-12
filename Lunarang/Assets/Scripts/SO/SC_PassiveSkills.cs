using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Passive Skill")]
public class SC_PassiveSkills : SC_Skill
{
    public enum AffectedStat //The player stats that is affeted by the effect of this skill "None" if there is a special effect
    {
        Attack,
        Health,
        Crit,
        Speed,
        None
    }

    public AffectedStat affectedStat;
    
    [Range(0,1), Tooltip("The amount of the augmentation (multiply by 100 to get a percentage)")]
    public float modifierAmount;
    
    public SC_PassiveSkills()
    {
        skillType = SkillTypeEnum.Passive;
    }
}
