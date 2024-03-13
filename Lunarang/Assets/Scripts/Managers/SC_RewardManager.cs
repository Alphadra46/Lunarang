using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_RewardManager : MonoBehaviour
{
    public static SC_RewardManager instance;
    
    [Header("Reward parameters")] 
    [SerializeField] private int numberOfNormalSkill;
    [SerializeField] private int numberOfLunarSkill;
    [SerializeField] private int numberOfMoonFragment;
    
    
    [HideInInspector] public List<SO_BaseSkill> selectedSkills = new List<SO_BaseSkill>();
    //TODO - List for Lunar Skills

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void RewardSelection()
    {
        for (int i = 0; i < numberOfNormalSkill; i++) //Normal skill selection
        {
            selectedSkills.Add(NormalSkillSelection());
        }
        
        
    }

    private SO_BaseSkill NormalSkillSelection()
    {
        var c = Resources.LoadAll<SC_Constellation>("Constellation").ToList();
        SO_SkillInventory playerInventory = Resources.Load<SO_SkillInventory>("SO_SkillInventory");

        
        if (playerInventory.ongoingConstellations.Count==0)
        {
            //Fully random selection
        }
        else
        {
            SC_Constellation selectedConstellation = playerInventory.ongoingConstellations[Random.Range(0, playerInventory.ongoingConstellations.Count)];
            float p = 0f;
            
            
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

            if (Random.Range(1,101) > p) //Random constellation skill
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
                if (Random.Range(1,101) > 70)
                {
                    //return selectedConstellation.GetRandomChildSkill();
                }
                else
                {
                    return selectedConstellation.GetRandomParentSkill(playerInventory.skillsOwned);
                }
                //TODO Get a random child or parent skill from the selected constellation with 70% of a child from existing parent and 30% of a new parent skill
            }
        }

        return null;
    }

}
