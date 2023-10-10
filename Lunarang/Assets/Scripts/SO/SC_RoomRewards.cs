using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_RoomRewards : MonoBehaviour
{
    
    public List<SC_Constellation> constellationList = new List<SC_Constellation>();

    [Tooltip("Must be at least 1 if at least the guaranteed skill, including the guaranteed skill")]
    [SerializeField] private int numberOfReward;

    private List<SC_Skill> skillInventory;
    
    private SC_Constellation constellationFollowed;
    //Temp -> GameObject will be changed to the type of the Skill class
    private List<SC_Skill> skillRewardList = new List<SC_Skill>();

    public void ResetAllLootTables()
    {
        foreach (var constellation in constellationList)
        {
            constellation.constellationLootTable.ResetLootTable();
        }
    }
    
    /// <summary>
    /// Will simulate the rewards for cleaning a room
    /// </summary>
    public void SimulateReward()
    {
        skillRewardList.Clear();
        
        foreach (var constellation in constellationList) //Checking all constellation to know which one is followed
        {
            if (constellation.isFollowingThisConstellation)
            {
                constellationFollowed = constellation;
                Debug.Log("Constellation Followed : "+constellationFollowed.name);
                break;
            }
        }
        
        //Guaranteed skill of the followed Constellation
        skillRewardList.Add(constellationFollowed.constellationLootTable.GetDrop()[0]);
        
        //Random skills from random selected constellations loot tables
        for (int i = 0; i < numberOfReward-1; i++) //numberOfReward - 1 is because the number of reward is including the guaranteed skill so we deduce 1 to have the wanted number at the end
        {
            //Debug.Log("Remaining reward : "+ (numberOfReward-1 - i) );
            var indexOfSelectedConstellation = Random.Range(0, constellationList.Count);
            var skillDropped = constellationList[indexOfSelectedConstellation].constellationLootTable.GetDrop()[0];
            if (skillRewardList.Contains(skillDropped))
            {
                i--;
                continue;
            }
            
            skillRewardList.Add(skillDropped); //Getting the first drop of the lootTable and then adding it to the reward list if the reward is different from what have been already choosed
        }

        var line = "Guaranteed skill : " + skillRewardList[0].name + 
                   "\n Random skills : " + skillRewardList[1].name + ", " + skillRewardList[2].name;
        
        Debug.Log(line);
    }

    public void ChooseReward1()
    {
        skillInventory = Resources.Load<SC_SkillsInventory>("SkillInventory").skillInventory;

        if (skillInventory.Contains(skillRewardList[0]) && !skillRewardList[0].isReinforced)
        {
            skillInventory[skillInventory.IndexOf(skillRewardList[0])].ReinforceSkill();
            constellationList[SearchForSkill(skillRewardList[0], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove);
        }
        
        else skillInventory.Add(skillRewardList[0]);
    }
    
    public void ChooseReward2()
    {
        skillInventory = Resources.Load<SC_SkillsInventory>("SkillInventory").skillInventory;

        if (skillInventory.Contains(skillRewardList[1]) && !skillRewardList[1].isReinforced)
        {
            skillInventory[skillInventory.IndexOf(skillRewardList[1])].ReinforceSkill();
            constellationList[SearchForSkill(skillRewardList[1], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove);
        }
        
        else skillInventory.Add(skillRewardList[1]);
    }
    
    public void ChooseReward3()
    {
        skillInventory = Resources.Load<SC_SkillsInventory>("SkillInventory").skillInventory;
        if (skillInventory.Contains(skillRewardList[2]) && !skillRewardList[2].isReinforced)
        {
            skillInventory[skillInventory.IndexOf(skillRewardList[2])].ReinforceSkill();
            constellationList[SearchForSkill(skillRewardList[2], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove); //Remove skill from pool if taken another time
        }
        
        else skillInventory.Add(skillRewardList[2]);
    }

    private int SearchForSkill(SC_Skill skillToSearch, out Loot<SC_Skill> skillFound)
    {
        var skill = new Loot<GameObject>();
        for (int i = 0; i < constellationList.Count; i++)
        {
            for (int j = 0; j < constellationList[i].constellationLootTable.lootTable.Count; j++)
            {
                if (constellationList[i].constellationLootTable.lootTable[j].Skill == skillToSearch)
                {
                    skillFound = constellationList[i].constellationLootTable.lootTable[j];
                    return i;
                }
            }
        }

        skillFound = null;
        return -1;
    }
}
