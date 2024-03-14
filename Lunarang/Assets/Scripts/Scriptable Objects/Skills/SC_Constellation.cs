using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "SO/Constellation", fileName = "SO_Constellation"), Serializable]
public class SC_Constellation : ScriptableObject
{
    public string name;
    public List<SO_ParentSkill> skills = new List<SO_ParentSkill>();

    /// <summary>
    /// Get a random child skill of a already owned parent skill
    /// </summary>
    /// <param name="playerInventory">The player skill inventory</param>
    /// <returns>The chosen skill</returns>
    public SO_ChildSkill GetRandomChildSkill(List<SO_BaseSkill> playerInventory)
    {
        var l = skills.ToList();
        l = l.Where(s => playerInventory.Contains(s)).ToList();
        var parentSkill = l[Random.Range(0, l.Count)];

        var lc = skills[skills.IndexOf(parentSkill)].childrenSkills.ToList();
        lc = lc.Where(s => !playerInventory.Contains(s)).ToList();
        
        //return lc[Random.Range(0,lc.Count)];
        return new SO_ChildSkill();
    }
    
    /// <summary>
    /// Get a random parent skill from a constellation
    /// </summary>
    /// <param name="inventory">The player skill inventory</param>
    /// <returns>The chosen skill</returns>
    public SO_ParentSkill GetRandomParentSkill(List<SO_BaseSkill> inventory)
    {
        var l = skills.ToList();
        l = l.Where(s => !inventory.Contains(s)).ToList();
        return l[Random.Range(0,l.Count)];
    }

    /// <summary>
    /// Check if all the parent and children skills from this constellation are already in the player inventory
    /// </summary>
    /// <param name="playerSkills">The player skill inventory</param>
    /// <returns>If the constellation is completely owned by the player</returns>
    public bool IsConstellationCompleted(List<SO_BaseSkill> playerSkills)
    {
        foreach (var parentSkill in skills)
        {
            if (!playerSkills.Contains(parentSkill))
                return false;
            
            foreach (var childSkill in skills[skills.IndexOf(parentSkill)].childrenSkills)
            {
                if (!playerSkills.Contains(childSkill))
                    return false;
            }
        }
        return true;
    }
    
}
