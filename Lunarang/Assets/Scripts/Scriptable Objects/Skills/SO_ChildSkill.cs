using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ChildSkill", fileName = "ChildSkill")]
public class SO_ChildSkill : SO_BaseSkill
{
    public SO_BaseSkill parentSkill;

    public Dictionary<string, string> buffsParentEffect = new Dictionary<string, string>();
    public List<SC_StatModification> buffsParentTest = new List<SC_StatModification>();
    
    public override void Init()
    {
        base.Init();
    }
    
}
