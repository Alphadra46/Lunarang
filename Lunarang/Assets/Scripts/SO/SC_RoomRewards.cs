using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_RoomRewards : MonoBehaviour
{
    
    public enum RewardType
    {
        Resource,
        Skill
    }

    public List<RewardType> rewardList = new List<RewardType>();

    public List<SC_Constellation> constellationList = new List<SC_Constellation>();
    public List<SC_Resources> resourcesList = new List<SC_Resources>();

    [Tooltip("Must be at least 1 if at least the guaranteed skill, including the guaranteed skill")]
    [SerializeField] private int numberOfReward;

    private SC_SkillsInventory skillInventoryScript;
    private SC_ResourcesInventory resourcesInventoryScript;
    
    private SC_Constellation constellationFollowed;
    //Temp -> GameObject will be changed to the type of the Skill class
    private List<SC_Skill> skillRewardList = new List<SC_Skill>();
    private List<SC_Resources> resourceRewardList = new List<SC_Resources>(); //Can be change with a simple variable instead of a List

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
        rewardList.Clear();
        skillRewardList.Clear();
        resourceRewardList.Clear();
        
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
        
        //Randomize the type of reward the player is getting (2 skills or 1 skill and 1 resource)
        for (int i = 0; i < numberOfReward-1; i++)
        {
            var index = rewardList.Contains(RewardType.Resource)?1:Random.Range(0, 2); //Choose at random between a resource or a skill and if a resource is already in the reward list it just draw skill
            var rewardType = index == 0 ? RewardType.Resource : RewardType.Skill;
            rewardList.Add(rewardType);
        }
        
        SelectReward();
        
        //Random skills from random selected constellations loot tables
        // for (int i = 0; i < numberOfReward-1; i++) //numberOfReward - 1 is because the number of reward is including the guaranteed skill so we deduce 1 to have the wanted number at the end
        // {
        //     //Debug.Log("Remaining reward : "+ (numberOfReward-1 - i) );
        //     var indexOfSelectedConstellation = Random.Range(0, constellationList.Count);
        //     var skillDropped = constellationList[indexOfSelectedConstellation].constellationLootTable.GetDrop()[0];
        //     if (skillRewardList.Contains(skillDropped))
        //     {
        //         i--;
        //         continue;
        //     }
        //     
        //     skillRewardList.Add(skillDropped); //Getting the first drop of the lootTable and then adding it to the reward list if the reward is different from what have been already chosen
        // }

        // var line = "Guaranteed skill : " + skillRewardList[0].name + 
        //            "\n Random skills : " + skillRewardList[1].name + ", " + skillRewardList[2].name;
        //
        // Debug.Log(line);
    }

    private void SelectReward()
    {
        for (int i = 0; i < numberOfReward-1; i++)
        {
            switch (rewardList[i])
            {
                case RewardType.Resource:
                    var indexofSelectedResource = Random.Range(0,resourcesList.Count);
                    var resourceDropped = resourcesList[indexofSelectedResource];
                    resourceDropped.amount = Random.Range(1, 50); //Change the min and max HERE
                    resourceRewardList.Add(resourceDropped);
                    break;
                case RewardType.Skill: //Random skills from random selected constellations loot tables
                    var indexOfSelectedConstellation = Random.Range(0, constellationList.Count);
                    var skillDropped = constellationList[indexOfSelectedConstellation].constellationLootTable.GetDrop()[0];
                    if (skillRewardList.Contains(skillDropped))
                    {
                        i--;
                        continue;
                    }
                    skillRewardList.Add(skillDropped); //Getting the first drop of the lootTable and then adding it to the reward list if the reward is different from what have been already chosen
                    break;
                default:
                    break;
            }
        }
    }
    
    public void ChooseReward(int index)//TODO - Resource section
    {
        if (rewardList.Contains(RewardType.Resource))
            resourcesInventoryScript = Resources.Load<SC_ResourcesInventory>("ResourceInventory");
        //var resourceInventory = resourcesInventoryScript.resourceInventory; //Maybe not needed
        
        skillInventoryScript = Resources.Load<SC_SkillsInventory>("SkillInventory");
        var skillInventory = skillInventoryScript.skillInventory;
        if (index > 0)
        {
            switch (rewardList[index-1])
            {
                case RewardType.Resource:
                    resourcesInventoryScript.AddResource(resourceRewardList[0]);
                    break;
                case RewardType.Skill:
                    if (skillInventory.Contains(skillRewardList[index]) && !skillRewardList[index].isReinforced)
                    {
                        skillInventory[skillInventory.IndexOf(skillRewardList[index])].ReinforceSkill();
                        constellationList[SearchForSkill(skillRewardList[index], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove); //Remove skill from pool if taken another time
                    }
                    else skillInventoryScript.AddSkill(skillRewardList[index]);
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (skillInventory.Contains(skillRewardList[index]) && !skillRewardList[index].isReinforced)
            {
                skillInventory[skillInventory.IndexOf(skillRewardList[index])].ReinforceSkill();
                constellationList[SearchForSkill(skillRewardList[index], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove); //Remove skill from pool if taken another time
            }
            else skillInventoryScript.AddSkill(skillRewardList[index]);
        }
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
