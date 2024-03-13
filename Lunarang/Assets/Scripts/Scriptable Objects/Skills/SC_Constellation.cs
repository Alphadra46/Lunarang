using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "SO/Constellation", fileName = "SO_Constellation")]
public class SC_Constellation : ScriptableObject
{
    
    public SerializedDictionary<SO_ParentSkill, List<SO_ChildSkill>> skills = new SerializedDictionary<SO_ParentSkill, List<SO_ChildSkill>>();

    public SO_ChildSkill GetRandomChildSkill(SO_ParentSkill parentSkill)
    {
        return skills[parentSkill][Random.Range(0, skills[parentSkill].Count)];
    }
    
    public SO_ParentSkill GetRandomParentSkill(List<SO_BaseSkill> inventory)
    {
        var s = skills.Keys.ToList()[Random.Range(0, skills.Keys.Count)];
        while (inventory.Contains(s))
        {
            s = skills.Keys.ToList()[Random.Range(0, skills.Keys.Count)];
        }
        return s;
    }

    public bool IsConstellationCompleted(List<SO_BaseSkill> playerSkills)
    {
        foreach (var parentSkill in skills.Keys)
        {
            if (!playerSkills.Contains(parentSkill))
                return false;
            
            foreach (var childSkill in skills[parentSkill])
            {
                if (!playerSkills.Contains(childSkill))
                    return false;
            }
        }
        return true;
    }
    
}
