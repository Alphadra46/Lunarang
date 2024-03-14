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


    [Header("Resource drop parameters")]
    [SerializeField, Range(1, 6)] private int templeLevel;
    [SerializeField] private float highRarityDropRate;
    [SerializeField] private Vector2 highRarityDropRange;
    [SerializeField] private float mediumRarityDropRate;
    [SerializeField] private Vector2 mediumRarityDropRange;
    [SerializeField] private float lowRarityDropRate;
    [SerializeField] private Vector2 lowRarityDropRange;

    [SerializeField] private float eliteQuantityMultiplier;
    [SerializeField] private Vector3 eliteDropRate;
    [SerializeField, Range(0,100)] private int moonFragmentEliteDropRate = 3;
    private float totalEliteDropRate => eliteDropRate.x + eliteDropRate.y + eliteDropRate.z;
    
    [SerializeField] private float baseQuantityMultiplier;
    [SerializeField] private Vector3 baseDropRate;
    private float totalBaseDropRate => baseDropRate.x + baseDropRate.y + baseDropRate.z;
    
    [SerializeField] private float chestQuantityMultiplier;
    [SerializeField] private Vector3 chestDropRate;
    private float totalChestDropRate => chestDropRate.x + chestDropRate.y + chestDropRate.z;

    
    
    private SC_Ressource moonFragment;
    private SO_SkillInventory playerSkillInventory;
    private SC_ResourcesInventory playerResourceInventory;
    private List<SC_Ressource> ressources = new List<SC_Ressource>();
    private List<SC_Constellation> constellations = new List<SC_Constellation>();
    [HideInInspector] public List<SO_BaseSkill> selectedSkills = new List<SO_BaseSkill>();
    //TODO - List for Lunar Skills

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        playerSkillInventory = Resources.Load<SO_SkillInventory>("SkillInventory");
        playerResourceInventory = Resources.Load<SC_ResourcesInventory>("ResourceInventory");
        constellations = Resources.LoadAll<SC_Constellation>("Constellations").ToList();
        moonFragment = Resources.Load<SC_Ressource>("Ressources/MoonFragment");
        ressources = Resources.LoadAll<SC_Ressource>("Ressources/Base").ToList();
    }

    /// <summary>
    /// Regroup all the rewards that have been selected
    /// </summary>
    /// <param name="rewardUI">The UI that display rewards</param>
    public void ChestRewardSelection(SC_RewardUI rewardUI)
    {
        for (int i = 0; i < numberOfNormalSkill; i++) //Normal skill selection
        {
            selectedSkills.Add(NormalSkillSelection());
        }
        
        //TODO - Lunar Skills
        
        rewardUI.rewardRessources.Add(moonFragment);
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
        c = c.Where(constel => !playerSkillInventory.completedConstellations.Contains(constel)).ToList(); //Remove from the list the constellation that are already completed
        
        if (playerSkillInventory.ongoingConstellations.Count==0) //Parent skill selected at random between every constellation that is not completed
        {
            SC_Constellation selectedConstellation = c[Random.Range(0, c.Count)];

            return selectedConstellation.GetRandomParentSkill(playerSkillInventory.skillsOwned);
        }
        else
        {
            SC_Constellation selectedConstellation = playerSkillInventory.ongoingConstellations[Random.Range(0, playerSkillInventory.ongoingConstellations.Count)];
            float p = 0f; //The probability to get a skill from an ongoing constellation
            
            switch (playerSkillInventory.ongoingConstellations.Count)
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
                foreach (var constellation in playerSkillInventory.ongoingConstellations)
                {
                    c.Remove(constellation);
                }
                selectedConstellation = c[Random.Range(0, c.Count)];
                return selectedConstellation.GetRandomParentSkill(playerSkillInventory.skillsOwned);
            }
            else //Ongoing constellation skill
            {
                if (Random.Range(1,101) > 70) //Get a child skill from a already owned parent skill and ongoing constellation
                {
                    return selectedConstellation.GetRandomChildSkill(playerSkillInventory.skillsOwned);
                }
                else //Get a new parent skill from an ongoing constellation
                {
                    return selectedConstellation.GetRandomParentSkill(playerSkillInventory.skillsOwned);
                }
            }
        }
        return null;
    }


    public void ResourceDropSelection(string source)
    {
        Vector3 dropRates = Vector3.zero;
        Vector2 dropRange = Vector2.zero;
        float totalDropRate = 0f;
        bool canDropMoonFragment = false;
        float quantityMultiplier = 1f;

        switch (source)
        {
            case "Elite":
                quantityMultiplier = eliteQuantityMultiplier;
                canDropMoonFragment = true;
                dropRates = eliteDropRate;
                totalDropRate = totalEliteDropRate;
                break;
            case "Base":
                quantityMultiplier = baseQuantityMultiplier;
                dropRates = baseDropRate;
                totalDropRate = totalBaseDropRate;
                break;
            case "Chest":
                quantityMultiplier = chestQuantityMultiplier;
                dropRates = chestDropRate;
                totalDropRate = totalChestDropRate;
                break;
            default:
                break;
        }

        if (canDropMoonFragment && Random.Range(1, 101) <= moonFragmentEliteDropRate)
        {
            playerResourceInventory.AddResource(moonFragment,1); 
            //TODO - Display resource
        }

        int dropChance = Random.Range(1, 101);
        int numberOfDropRoll = dropChance > totalDropRate ? 0 :
            dropChance <= dropRates.x ? 1 :
            (dropChance > dropRates.x && dropChance <= dropRates.x + dropRates.y) ? 2 : 3;

        if (numberOfDropRoll<=0) //No Need to go further if there is no item to drop
            return;
        
        for (int i = 0; i < numberOfDropRoll; i++)
        {
            float highRarityDropRate = this.highRarityDropRate;
            float mediumRarityDropRate = this.mediumRarityDropRate;
            float lowRarityDropRate = this.lowRarityDropRate;

            if (templeLevel-1 >=0) //If the level of the temple is at the minimum
            {
                mediumRarityDropRate = lowRarityDropRate / 2;
                highRarityDropRate = lowRarityDropRate / 2;
                lowRarityDropRate = 0f;
            }
            else if (templeLevel+1 >6) //If the level of the temple is at max
            {
                mediumRarityDropRate = highRarityDropRate / 2;
                lowRarityDropRate = highRarityDropRate / 2;
                highRarityDropRate = 0f;
            }

            int r = Random.Range(1, 101);
            int resourceLevel = r <= highRarityDropRate ? templeLevel + 1 :
                (r > highRarityDropRate && r <= highRarityDropRate + mediumRarityDropRate) ? templeLevel :
                templeLevel - 1;

            playerResourceInventory.AddResource(ressources.First(ressource => ressource.rarityLevel==resourceLevel),Mathf.RoundToInt(Random.Range(dropRange.x,dropRange.y)*quantityMultiplier));
            //TODO - Display resource
        }
    }
    
}
