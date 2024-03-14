using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_RewardManager : MonoBehaviour
{
    public static SC_RewardManager instance;
    
    [Header("Chest room reward parameters")] 
    [SerializeField] private int numberOfNormalSkill;
    [SerializeField] private int numberOfLunarSkill;
    [SerializeField] private int numberOfMoonFragment;

    private SO_SkillInventory playerInventory;
    private List<SC_Constellation> constellations = new List<SC_Constellation>();
    [HideInInspector] public List<SO_BaseSkill> selectedSkills = new List<SO_BaseSkill>();
    //TODO - List for Lunar Skills

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        playerInventory = Resources.Load<SO_SkillInventory>("SkillInventory");
        constellations = Resources.LoadAll<SC_Constellation>("Constellations").ToList();
    }

    /// <summary>
    /// Regroup all the rewards that have been selected
    /// </summary>
    /// <param name="rewardUI">The UI that display rewards</param>
    public void RewardSelection(SC_RewardUI rewardUI)
    {
        for (int i = 0; i < numberOfNormalSkill; i++) //Normal skill selection
        {
            selectedSkills.Add(NormalSkillSelection());
        }
        
        //TODO - Lunar Skills
        
        //TODO - Moon fragments

        rewardUI.rewardSkills = selectedSkills.ToList();
        rewardUI.Init();
    }

    /// <summary>
    /// Select a skill based a constellation
    /// </summary>
    /// <returns>The chosen skill</returns>
    private SO_BaseSkill NormalSkillSelection()
    {
        var c = constellations.ToList(); 
        c = c.Where(constel => !playerInventory.completedConstellations.Contains(constel)).ToList(); //Remove from the list the constellation that are already completed
        
        if (playerInventory.ongoingConstellations.Count==0) //Parent skill selected at random between every constellation that is not completed
        {
            SC_Constellation selectedConstellation = c[Random.Range(0, c.Count)];

            return selectedConstellation.GetRandomParentSkill(playerInventory.skillsOwned);
        }
        else
        {
            SC_Constellation selectedConstellation = playerInventory.ongoingConstellations[Random.Range(0, playerInventory.ongoingConstellations.Count)];
            float p = 0f; //The probability to get a skill from an ongoing constellation
            
            switch (playerInventory.ongoingConstellations.Count)
            {
                case 1:
                    p = 50f;
                    break;
                case 2:
                    p = 80f;
                    break;
                case 3:
                    p = 95f;
                    break;
                default:
                    p = 100f;
                    break;
            }

            if (Random.Range(1,101) > p) //Random constellation skill from a constellation that the player have not started yet
            {
                foreach (var constellation in playerInventory.ongoingConstellations)
                {
                    c.Remove(constellation);
                }
                selectedConstellation = c[Random.Range(0, c.Count)];
                return selectedConstellation.GetRandomParentSkill(playerInventory.skillsOwned);
            }
            else //Ongoing constellation skill
            {
                if (Random.Range(1,101) > 70) //Get a child skill from a already owned parent skill and ongoing constellation
                {
                    return selectedConstellation.GetRandomChildSkill(playerInventory.skillsOwned);
                }
                else //Get a new parent skill from an ongoing constellation
                {
                    return selectedConstellation.GetRandomParentSkill(playerInventory.skillsOwned);
                }
            }
        }
        return null;
    }

}
