using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Middle-Men/Resources Inventory")]
public class SO_SkillInventory : ScriptableObject
{
    public List<SC_Constellation> ongoingConstellations = new List<SC_Constellation>();
    public List<SC_Constellation> completedConstellations = new List<SC_Constellation>();
    public List<SO_BaseSkill> skillsOwned = new List<SO_BaseSkill>();

    private List<SO_BaseSkill> preSelectedSkills = new List<SO_BaseSkill>();

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

    public SC_Constellation FindConstellationByName(string constellationName)
    {
        return Resources.LoadAll<SC_Constellation>("Constellations").ToList().FirstOrDefault(constellation => constellation.name == constellationName);
    }

    /// <summary>
    /// Add a skill to the inventory
    /// </summary>
    /// <param name="skill">The skill to add</param>
    public void AddSkill(SO_BaseSkill skill)
    {
        
        Debug.Log(skill.skillName);
        var temp = skill as SO_LunarSkill;
        if (temp != null) goto lunarSkill;

        if (skillsOwned.Contains(skill))
            return;
        
        skillsOwned.Add(skill);
        if(skill.constellationSC != null)
            AddConstellation(skill.constellationSC);
        skill.Init();

        foreach (var constellation in ongoingConstellations)
        {
            if (constellation.IsConstellationCompleted(skillsOwned))
            {
                ongoingConstellations.Remove(constellation);
                completedConstellations.Add(constellation);
                return;
            }
        }
        return;
        
        lunarSkill:
            if (skillsOwned.Contains(skill))
            {
                temp.IncrementLevel(1);
                return;
            }
            skillsOwned.Add(skill);
            skill.Init();
        
        

    }

    public void RemoveSkill(SO_BaseSkill skill)
    {
        if (!skillsOwned.Contains(skill))
            return;

        skillsOwned.Remove(skill);

        foreach (var constellation in completedConstellations)
        {
            if (!constellation.IsConstellationCompleted(skillsOwned))
            {
                completedConstellations.Remove(constellation);
                ongoingConstellations.Add(constellation);
                return;
            }
        }
        return;
    }
    
    
    
    /// <summary>
    /// Clear the player's inventory
    /// </summary>
    [Button]
    public void ClearInventory()
    
    {
        completedConstellations.Clear();
        ongoingConstellations.Clear();
        skillsOwned.Clear();
    }
    
    public bool CheckHasSkillByName(string skillName)
    {
        return skillsOwned.Contains(FindSkillByName(skillName));
    }
    
    public bool CheckHasSkill(SO_BaseSkill skill)
    {
        return skillsOwned.Contains(skill);
    }
    
    public SO_BaseSkill FindSkillByName(string skillName)
    {
        return SC_GameManager.instance.allSkills.FirstOrDefault(skill => skill.skillName == skillName);
    }
    
    public SO_ChildSkill FindChildSkillByName(string childSkillName)
    {
        return (SO_ChildSkill) SC_GameManager.instance.allSkills.FirstOrDefault(skill => skill.skillName == childSkillName);
    }

    public List<SO_ParentSkill> GetAllParentSkillsByConstellation(SC_Constellation constellation)
    {

        return constellation.skills.Keys.ToList();
        
    }
    
    public List<SO_ChildSkill> GetAllChildSkillsByConstellation(SC_Constellation constellation, SO_ParentSkill parentSkill)
    {

        return constellation.skills[parentSkill].ToList();
        
    }

    public void SavePreSelectedSkills()
    {
        preSelectedSkills = skillsOwned.ToList();
    }

    public void ResetSkills()
    {
        ClearInventory();

        foreach (var skill in preSelectedSkills)
        {
            AddSkill(skill);
        }
        
        preSelectedSkills.Clear();
    }
    
    public void ReloadSkills()
    {

        foreach (var skill in skillsOwned)
        {
            skill.Init();
        }
        
    }
    
}
