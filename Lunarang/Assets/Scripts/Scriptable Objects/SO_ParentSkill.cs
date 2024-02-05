using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ParentSkills", fileName = "ParentSkill")]
public class SO_ParentSkill : SO_BaseSkill
{

    public List<SO_BaseSkill> childrenSkills;

    public override void Init()
    {
        base.Init();
        
        // TODO Add children skills.
    }
    
}
