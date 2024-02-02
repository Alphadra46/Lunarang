// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// public class SC_RoomRewards : MonoBehaviour
// {
//
//     public static SC_RoomRewards instance;
//     
//     public enum RewardType
//     {
//         Resource,
//         Skill
//     }
//
//     public List<RewardType> rewardList = new List<RewardType>(); //List of the different type of reward, it is used to differentiate from which loot table the drop will be selected
//
//     public List<SC_Constellation> constellationList = new List<SC_Constellation>(); //The list of every constellation, used to check which constellation is followed by the player
//     public SC_ResourceLootTable resourcesList; //The loot table of every resources in the game
//
//     [Tooltip("Must be at least 1 if at least the guaranteed skill, including the guaranteed skill")]
//     [SerializeField] private int numberOfReward; //How many reward the player will get when there is a reward (Include the guaranteed skill if a constellation is followed by the player)
//
//     // private SC_SkillsInventory skillInventoryScript; //reference to the skill inventory script
//     private SC_ResourcesInventory resourcesInventoryScript; //reference to the resource inventory script
//     
//     private SC_Constellation constellationFollowed; //The constellation that is followed by the player and from which the guaranteed skill will be selected
//     private List<SC_Skill> skillRewardList = new List<SC_Skill>(); //List of every skill that has been dropped by the player
//     private List<SC_Resources> resourceRewardList = new List<SC_Resources>(); //List of every skill that has been dropped by the player //Can be change with a simple variable instead of a List
//
//
//     private void Awake() 
//     {
//         instance = this; //This script is made a singleton to be accessible from everywhere in the scene
//     }
//
//     /// <summary>
//     /// Reset all the loot tables with their initial values (skill list)
//     /// </summary>
//     public void ResetAllLootTables()
//     {
//         foreach (var constellation in constellationList)
//         {
//             constellation.constellationLootTable.ResetLootTable();
//         }
//     }
//     
//     /// <summary>
//     /// Will simulate the rewards for cleaning a room
//     /// </summary>
//     public void SimulateReward()
//     {
//         //Clearing all the lists used during the selection of drops
//         rewardList.Clear();
//         skillRewardList.Clear();
//         resourceRewardList.Clear();
//         
//         //Checking all constellation to know which one is followed
//         foreach (var constellation in constellationList) 
//         {
//             if (constellation.isFollowingThisConstellation)
//             {
//                 constellationFollowed = constellation;
//                 Debug.Log("Constellation Followed : "+ constellationFollowed.name);
//                 break;
//             }
//         }
//
//         //Guaranteed skill of the followed Constellation, if the player doesn't 
//         if (constellationFollowed.constellationLootTable.lootTable.Count > 0) //Skip the guaranteed skill drop if there is no more skills in the pool
//             skillRewardList.Add(constellationFollowed.constellationLootTable.GetDrop()[0]);
//         
//         
//         
//         //Randomize the type of reward the player is getting (2 skills OR 1 skill and 1 resource)
//         for (int i = 0; i < (constellationFollowed.constellationLootTable.lootTable.Count>0?numberOfReward-1:numberOfReward); i++)
//         {
//             var index = rewardList.Contains(RewardType.Resource)?1:Random.Range(0, 2); //Choose at random between a resource or a skill and if a resource is already in the reward list it just draw skill
//             var rewardType = index == 0 ? RewardType.Resource : RewardType.Skill;
//             rewardList.Add(rewardType);
//         }
//         
//         SelectReward(); //Select the 2 random rewards that the player will get
//     }
//
//     /// <summary>
//     /// Select random rewards based on the reward types in the rewardList variable
//     /// </summary>
//     private void SelectReward()
//     {
//         for (int i = 0; i < (constellationFollowed.constellationLootTable.lootTable.Count>0?numberOfReward-1:numberOfReward); i++) //There is a security in case the player already have all of the skill at max level from a the constellation he follows
//         {
//             switch (rewardList[i])
//             {
//                 case RewardType.Resource: //Select a random resource
//                     var resourceDropped = resourcesList.GetDrop()[0];
//                     resourceDropped.amount = Random.Range(resourceDropped.minAmount, resourceDropped.maxAmount);
//                     resourceRewardList.Add(resourceDropped);
//                     break;
//                 case RewardType.Skill: //Random skills from random selected constellations loot tables
//                     var indexOfSelectedConstellation = Random.Range(0, constellationList.Count);
//                     if (constellationList[indexOfSelectedConstellation].constellationLootTable.lootTable.Count <= 0) //If the loot table selected is empty then re-randomize the drop to avoid null references
//                     {
//                         i--; //Need to decrease the index by 1 before continue so that there is the right amount of reward at the end
//                         continue;
//                     }
//                     
//                     var skillDropped = constellationList[indexOfSelectedConstellation].constellationLootTable.GetDrop()[0];
//                     if (skillRewardList.Contains(skillDropped)) //Prevent the player to have twice the same skill in a single reward selection
//                     {
//                         i--; //Need to decrease the index by 1 before continue so that there is the right amount of reward at the end
//                         continue;
//                     }
//                     skillRewardList.Add(skillDropped); //Getting the first drop of the lootTable and then adding it to the reward list if the reward is different from what have been already chosen
//                     break;
//                 default:
//                     break;
//             }
//         }
//
//         #region Debug
//         var line = "";
//
//
//         if (constellationFollowed.constellationLootTable.lootTable.Count > 0)
//         {
//            line = "Guaranteed skill : " + skillRewardList[0].name +
//                 "\n Random drops : " + (rewardList.Contains(RewardType.Resource) 
//                     ?(rewardList[0] == RewardType.Resource
//                         ? resourceRewardList[0].resourceName
//                         : skillRewardList[1].name) + ", "+ (rewardList[1] == RewardType.Resource
//                         ? resourceRewardList[0].resourceName
//                         : skillRewardList[1].name)
//                     :skillRewardList[1].name + ", " + skillRewardList[2].name);
//         }
//         else
//         {
//             line = "Random drops : " + (rewardList.Contains(RewardType.Resource)
//                 ? (rewardList[0] == RewardType.Resource
//                       ? resourceRewardList[0].resourceName
//                       : skillRewardList[0].name) + ", " +
//                   (rewardList[1] == RewardType.Resource
//                       ? resourceRewardList[0].resourceName
//                       : (rewardList[0] == RewardType.Resource ? skillRewardList[0].name : skillRewardList[1].name)) + ", " +
//                   (rewardList[2] == RewardType.Resource
//                       ? resourceRewardList[0].resourceName
//                       : skillRewardList[1])
//                 : skillRewardList[0].name + ", " + skillRewardList[1].name + ", " + skillRewardList[2].name);
//         }
//
//
//         Debug.Log(line);
//         #endregion
//     }
//     
//     // public void ChooseReward(int index)
//     // {
//     //     if (rewardList.Contains(RewardType.Resource))
//     //         resourcesInventoryScript = Resources.Load<SC_ResourcesInventory>("ResourceInventory");
//     //     
//     //     // skillInventoryScript = Resources.Load<SC_SkillsInventory>("SkillInventory");
//     //     // var skillInventory = skillInventoryScript.skillInventory;
//     //     var i = rewardList.Contains(RewardType.Resource) ? Mathf.Clamp(index, 0, 1) : index;
//     //     if (index > 0)
//     //     {
//     //         switch (rewardList[index-1])
//     //         {
//     //             case RewardType.Resource:
//     //                 resourcesInventoryScript.AddResource(resourceRewardList[0]);
//     //                 break;
//     //             case RewardType.Skill:
//     //                 
//     //                 if (skillInventory.Contains(skillRewardList[i]) && skillRewardList[i].level > 0)
//     //                 {
//     //                     skillInventory[skillInventory.IndexOf(skillRewardList[i])].ReinforceSkill();
//     //                     if (skillRewardList[i].level >= skillRewardList[i].maxLevel)
//     //                     {
//     //                         constellationList[SearchForSkill(skillRewardList[i], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove); //Remove skill from pool if it's already at max level
//     //                     }
//     //                 }
//     //                 else skillInventoryScript.AddSkill(skillRewardList[i]);
//     //                 break;
//     //             default:
//     //                 break;
//     //         }
//     //     }
//     //     else
//     //     {
//     //         if (skillInventory.Contains(skillRewardList[i]) && skillRewardList[i].level > 0) //Same problem as higher
//     //         {
//     //             skillInventory[skillInventory.IndexOf(skillRewardList[i])].ReinforceSkill();
//     //             if (skillRewardList[i].level >= skillRewardList[i].maxLevel)
//     //             {
//     //                 constellationList[SearchForSkill(skillRewardList[i], out Loot<SC_Skill> skillToRemove)].constellationLootTable.lootTable.Remove(skillToRemove); //Remove skill from pool if it's already at max level
//     //             }
//     //         }
//     //         else
//     //         {
//     //             skillRewardList[i].level = 1;
//     //             skillInventoryScript.AddSkill(skillRewardList[i]);
//     //         }
//     //     }
//     // }
//     
//
//     private int SearchForSkill(SC_Skill skillToSearch, out Loot<SC_Skill> skillFound)
//     {
//         for (int i = 0; i < constellationList.Count; i++)
//         {
//             for (int j = 0; j < constellationList[i].constellationLootTable.lootTable.Count; j++)
//             {
//                 if (constellationList[i].constellationLootTable.lootTable[j].Drop == skillToSearch)
//                 {
//                     skillFound = constellationList[i].constellationLootTable.lootTable[j];
//                     return i;
//                 }
//             }
//         }
//
//         skillFound = null;
//         return -1;
//     }
// }
