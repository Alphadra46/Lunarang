using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/ChildSkill", fileName = "ChildSkill")]
public class SO_ChildSkill : SO_BaseSkill
{
    public SO_BaseSkill parentSkill;

    public Dictionary<string, string> buffsParentEffect = new Dictionary<string, string>(); 
    
    public override void Init()
    {
        base.Init();
        
        SC_SkillManager.instance.allCurrentRunSkills.Remove(this);
    }
    
}
