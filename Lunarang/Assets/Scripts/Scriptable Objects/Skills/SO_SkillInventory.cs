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

    public void AddConstellation(SC_Constellation constellation)
    {
        if (ongoingConstellations.Contains(constellation))
            return;
        
        ongoingConstellations.Add(constellation);
    }

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
