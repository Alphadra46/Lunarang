using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Middle-Men/Resources Inventory")]
public class SO_SkillInventory : ScriptableObject
{
    public List<SC_Constellation> ongoingConstellations = new List<SC_Constellation>();
    public List<SC_Constellation> completedConstellations = new List<SC_Constellation>();
    public List<SO_BaseSkill> skillsOwned = new List<SO_BaseSkill>();

    /// <summary>
    /// Add a constellation to the ongoing constellation list
    /// </summary>
    /// <param name="constellation">The constellation to add</param>
    public void AddConstellation(SC_Constellation constellation)
    {
        if (ongoingConstellations.Contains(constellation))
            return;
        
        ongoingConstellations.Add(constellation);
    }

    /// <summary>
    /// Add a skill to the inventory
    /// </summary>
    /// <param name="skill">The skill to add</param>
    public void AddSkill(SO_BaseSkill skill)
    {
        bool constellationCompleted = false;
        
        if (skillsOwned.Contains(skill))
            return;
        
        skillsOwned.Add(skill);

        foreach (var constellation in ongoingConstellations)
        {
            if (constellation.IsConstellationCompleted(skillsOwned))
            {
                ongoingConstellations.Remove(constellation);
                completedConstellations.Add(constellation);
                return;
            }
        }
    }
}
