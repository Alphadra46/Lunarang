using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ParentSkills", fileName = "ParentSkill")]
public class SO_ParentSkill : SO_BaseSkill
{

    public List<SO_BaseSkill> childrenSkills;

    public override void Init()
    {
        base.Init();

        foreach (var child in childrenSkills.Where(child => !SC_SkillManager.instance.allSkills.Contains(child)))
        {
            SC_SkillManager.instance.allSkills.Add(child);
        }

        SC_SkillManager.instance.allSkills.Remove(this);
    }
    
}
