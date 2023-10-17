using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Skill/Passive Skill")]
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
    
    [Range(0,100), Tooltip("The amount of the augmentation in percentage for each level from 1 to 5, i.e. level 1 is 2, level 2 is 4 so you write 2 for level 1 and 2")]
    public int[] modifierPerLevel = new int[5];
    
    public SC_PassiveSkills()
    {
        skillType = SkillTypeEnum.Passive;
    }

    public void ApplyPassiveSkillEffect()
    {
        switch (affectedStat)
        {
            case AffectedStat.Attack:
                SC_PlayerStats.instance.atkModifier += LevelScaling();
                break;
            case AffectedStat.Crit:
                SC_PlayerStats.instance.critRate += LevelScaling();
                break;
            case AffectedStat.Health:
                Debug.Log("TODO - Health modifier");
                break;
            case AffectedStat.Speed:
                SC_PlayerStats.instance.speedModifier += LevelScaling();
                break;
            case AffectedStat.None:
                Debug.Log("TODO - Skill effect (boolean)");
                break;
            default:
                break;
        }
    }

    /// <summary>
    ///Return the value to add to the stats modifier in the player stats
    /// </summary>
    /// <returns></returns>
    private float LevelScaling()
    {
        return modifierPerLevel[level - 1]/100f;
    }
    
}
