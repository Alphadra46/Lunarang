using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "SO/Constellation", fileName = "SO_Constellation")]
public class SC_Constellation : SerializedScriptableObject
{
    public string name;
    public SerializedDictionary<SO_ParentSkill, List<SO_ChildSkill>> skills = new SerializedDictionary<SO_ParentSkill, List<SO_ChildSkill>>();

    public SO_ChildSkill GetRandomChildSkill(List<SO_BaseSkill> playerInventory)
    {
        var l = skills.Keys.ToList();
        l = l.Where(s => playerInventory.Contains(s)).ToList();
        var parentSkill = l[Random.Range(0, l.Count)];

        var lc = skills[parentSkill];
        lc = lc.Where(s => !playerInventory.Contains(s)).ToList();
        
        return lc[Random.Range(0,lc.Count)];
    }
    
    public SO_ParentSkill GetRandomParentSkill(List<SO_BaseSkill> inventory)
    {
        var l = skills.Keys.ToList();
        l = l.Where(s => !inventory.Contains(s)).ToList();
        return l[Random.Range(0,l.Count)];
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
