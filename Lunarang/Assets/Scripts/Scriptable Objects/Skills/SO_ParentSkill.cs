using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ParentSkills", fileName = "ParentSkill")]
public class SO_ParentSkill : SO_BaseSkill
{

    public List<SO_BaseSkill> childrenSkills;

    public override void Init()
    {
        base.Init();
    }
    
}
