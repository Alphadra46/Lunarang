using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_RoomRewards : MonoBehaviour
{

    public static SC_RoomRewards instance;
    
    public enum RewardType
    {
        Resource,
        Skill
    }

    public List<RewardType> rewardList = new List<RewardType>();

    public List<SC_Constellation> constellationList = new List<SC_Constellation>();
    public SC_ResourceLootTable resourcesList;

    [Tooltip("Must be at least 1 if at least the guaranteed skill, including the guaranteed skill")]
    [SerializeField] private int numberOfReward;

    private SC_SkillsInventory skillInventoryScript;
    private SC_ResourcesInventory resourcesInventoryScript;
    
    private SC_Constellation constellationFollowed;
    private List<SC_Skill> skillRewardList = new List<SC_Skill>();
    private List<SC_Resources> resourceRewardList = new List<SC_Resources>(); //Can be change with a simple variable instead of a List


    private void Awake()
    {
        instance = this;
    }

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
                Debug.Log("Constellation Followed : "+ constellationFollowed.name);
                break;
            }
        }

        //Guaranteed skill of the followed Constellation
        if (constellationFollowed.constellationLootTable.lootTable.Count > 0) //Skip the guaranteed skill drop if there is no more skills in the pool
            skillRewardList.Add(constellationFollowed.constellationLootTable.GetDrop()[0]);
        
        
        
        //Randomize the type of reward the player is getting (2 skills OR 1 skill and 1 resource)
        for (int i = 0; i < (constellationFollowed.constellationLootTable.lootTable.Count>0?numberOfReward-1:numberOfReward); i++)
        {
            var index = rewardList.Contains(RewardType.Resource)?1:Random.Range(0, 2); //Choose at random between a resource or a skill and if a resource is already in the reward list it just draw skill
            var rewardType = index == 0 ? RewardType.Resource : RewardType.Skill;
            rewardList.Add(rewardType);
        }
        
        SelectReward(); //Select the 2 random rewards that the player will get
    }

    private void SelectReward()
    {
        for (int i = 0; i < (constellationFollowed.constellationLootTable.lootTable.Count>0?numberOfReward-1:numberOfReward); i++)
        {
            switch (rewardList[i])
            {
                case RewardType.Resource:
                    //var indexofSelectedResource = Random.Range(0,resourcesList.lootTable.Count);
                    var resourceDropped = resourcesList.GetDrop()[0];
                    resourceDropped.amount = Random.Range(resourceDropped.minAmount, resourceDropped.maxAmount);
                    resourceRewardList.Add(resourceDropped);
                    break;
                case RewardType.Skill: //Random skills from random selected constellations loot tables
                    var indexOfSelectedConstellation = Random.Range(0, constellationList.Count);
                    if (constellationList[indexOfSelectedConstellation].constellationLootTable.lootTable.Count <= 0) //If the loot table selected is empty then re-randomize the drop
                    {
                        i--;
                        continue;
                    }
                    
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

        var line = "";


        if (constellationFollowed.constellationLootTable.lootTable.Count > 0)
        {
           line = "Guaranteed skill : " + skillRewardList[0].name +
                "\n Random drops : " + (rewardList.Contains(RewardType.Resource) 
                    ?(rewardList[0] == RewardType.Resource
                        ? resourceRewardList[0].resourceName
                        : skillRewardList[1].name) + ", "+ (rewardList[1] == RewardType.Resource
                        ? resourceRewardList[0].resourceName
                        : skillRewardList[1].name)
                    :skillRewardList[1].name + ", " + skillRewardList[2].name);
        }
        else
        {
            line = "Random drops : " + (rewardList.Contains(RewardType.Resource)
                ? (rewardList[0] == RewardType.Resource
                      ? resourceRewardList[0].resourceName
                      : skillRewardList[0].name) + ", " +
                  (rewardList[1] == RewardType.Resource
                      ? resourceRewardList[0].resourceName
                      : (rewardList[0] == RewardType.Resource ? skillRewardList[0].name : skillRewardList[1].name)) + ", " +
                  (rewardList[2] == RewardType.Resource
                      ? resourceRewardList[0].resourceName
                      : skillRewardList[1])
                : skillRewardList[0].name + ", " + skillRewardList[1].name + ", " + skillRewardList[2].name);
        }


        Debug.Log(line);
        
    }
    
    public void ChooseReward(int index)
    {
        if (rewardList.Contains(RewardType.Resource))
            resourcesInventoryScript = Resources.Load<SC_ResourcesInventory>("ResourceInventory");
        
        skillInventoryScript = Resources.Load<SC_SkillsInventory>("SkillInventory");
        var skillInventory = skillInventoryScript.skillInventory;
        var i = rewardList.Contains(RewardType.Resource) ? Mathf.Clamp(index, 0, 1) : index;
        if (index > 0)
        {
            switch (rewardList[index-1])
            {
                case RewardType.Resource:
                    resourcesInventoryScript.AddResource(resourceRewardList[0]);
                    break;
                case RewardType.Skill:
                    
                    if (skillInventory.Contains(skillRewardList[i]) && skillRewardList[i].level > 0)
                    {
                        skillInventory[skillInventory.IndexOf(skillRewardList[i])].ReinforceSkill();
                        if (skillRewardList[i].level >= skillRewardList[i].maxLevel)
                        {
                            constellationList[SearchForSkill(skillRewardList[i], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove); //Remove skill from pool if it's already at max level
                        }
                    }
                    else skillInventoryScript.AddSkill(skillRewardList[i]);
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (skillInventory.Contains(skillRewardList[i]) && skillRewardList[i].level > 0) //Same problem as higher
            {
                skillInventory[skillInventory.IndexOf(skillRewardList[i])].ReinforceSkill();
                if (skillRewardList[i].level >= skillRewardList[i].maxLevel)
                {
                    constellationList[SearchForSkill(skillRewardList[i], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove); //Remove skill from pool if it's already at max level
                }
            }
            else
            {
                skillRewardList[i].level = 1;
                skillInventoryScript.AddSkill(skillRewardList[i]);
            }
        }
    }
    

    private int SearchForSkill(SC_Skill skillToSearch, out Loot<SC_Skill> skillFound)
    {
        for (int i = 0; i < constellationList.Count; i++)
        {
            for (int j = 0; j < constellationList[i].constellationLootTable.lootTable.Count; j++)
            {
                if (constellationList[i].constellationLootTable.lootTable[j].Drop == skillToSearch)
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
