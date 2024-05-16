using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "SO/Constellation", fileName = "SO_Constellation")]
public class SC_Constellation : SerializedScriptableObject
{
    public string name;
    public string description;
    public Color32 color;
    public Sprite splashArt;
    public Dictionary<SO_ParentSkill, List<SO_ChildSkill>> skills = new Dictionary<SO_ParentSkill, List<SO_ChildSkill>>();

    /// <summary>
    /// Get a random child skill of a already owned parent skill
    /// </summary>
    /// <param name="playerInventory">The player skill inventory</param>
    /// <returns>The chosen skill</returns>
    public SO_ChildSkill GetRandomChildSkill(List<SO_BaseSkill> playerInventory, List<SO_BaseSkill> skillsToExclude)
    {
        var l = skills.Keys.ToList();
        l = l.Where(s => playerInventory.Contains(s)).ToList();
        var parentSkill = l[Random.Range(0, l.Count)];

        var lc = skills[parentSkill];
        lc = lc.Where(s => !playerInventory.Contains(s)).ToList(); //Ignore the skills that are already in the player inventory
        lc = lc.Where(s => !skillsToExclude.Contains(s)).ToList(); //Ignore the skills that are already selected 
        
        return lc[Random.Range(0,lc.Count)];
    }
    
    /// <summary>
    /// Get a random parent skill from a constellation
    /// </summary>
    /// <param name="inventory">The player skill inventory</param>
    /// <returns>The chosen skill</returns>
    public SO_ParentSkill GetRandomParentSkill(List<SO_BaseSkill> inventory, List<SO_BaseSkill> skillsToExclude)
    {
        var l = skills.Keys.ToList();
        l = l.Where(s => !inventory.Contains(s)).ToList(); //Ignore the skills that are already in the player inventory
        l = l.Where(s => !skillsToExclude.Contains(s)).ToList(); //Ignore the skills that are already selected 
        return l[Random.Range(0,l.Count)];
    }

    /// <summary>
    /// Check if all the parent and children skills from this constellation are already in the player inventory
    /// </summary>
    /// <param name="playerSkills">The player skill inventory</param>
    /// <returns>If the constellation is completely owned by the player</returns>
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
