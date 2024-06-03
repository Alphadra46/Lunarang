using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SC_RewardManager : MonoBehaviour //TODO - Need to do a out of range check if there is no more skills to get / one left
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

    
    
    [HideInInspector] public SC_Resource essenceFragment;
    private List<SC_Resource> ressources = new List<SC_Resource>();
    private List<SC_Constellation> constellations = new List<SC_Constellation>();
    [HideInInspector] public List<SO_BaseSkill> selectedSkills = new List<SO_BaseSkill>();
    private List<SO_BaseSkill> lunarSkills = new List<SO_BaseSkill>();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        constellations = Resources.LoadAll<SC_Constellation>("Constellations").ToList();
        essenceFragment = Resources.Load<SC_Resource>("Ressources/EssenceFragment");
        ressources = Resources.LoadAll<SC_Resource>("Ressources/Base").ToList();
        lunarSkills = Resources.LoadAll<SO_BaseSkill>("Skills").ToList();
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
            if (selectedSkills[Mathf.Clamp(i,0,selectedSkills.Count)] == null)
                selectedSkills.Remove(selectedSkills[Mathf.Clamp(i,0,selectedSkills.Count)]);
        }
        
        if(lunarSkills.Count>0)
            selectedSkills.Add(lunarSkills[Random.Range(0, lunarSkills.Count)]);
        
        rewardUI.rewardRessources.Add(essenceFragment);
        rewardUI.rewardSkills = selectedSkills.ToList();
        rewardUI.Show();
        
        selectedSkills.Clear();
    }

    /// <summary>
    /// Select a skill based a constellation
    /// </summary>
    /// <returns>The chosen skill</returns>
    private SO_BaseSkill NormalSkillSelection()
    {
        SO_BaseSkill randomSkill;
        var c = constellations.ToList();
        c = c.Where(constel => !SC_GameManager.instance.playerSkillInventory.completedConstellations.Contains(constel)).ToList(); //Remove from the list the constellation that are already completed

        if (c.Count <= 0)
            return null;
        
        if (SC_GameManager.instance.playerSkillInventory.ongoingConstellations.Count==0) //Parent skill selected at random between every constellation that is not completed
        {
            SC_Constellation selectedConstellation = c[Random.Range(0, c.Count)];

            randomSkill = selectedConstellation.GetRandomParentSkill(SC_GameManager.instance.playerSkillInventory.skillsOwned, selectedSkills);
        }
        else
        {
            SC_Constellation selectedConstellation = SC_GameManager.instance.playerSkillInventory.ongoingConstellations[Random.Range(0, SC_GameManager.instance.playerSkillInventory.ongoingConstellations.Count)];
            float p = 0f; //The probability to get a skill from an ongoing constellation
            
            switch (SC_GameManager.instance.playerSkillInventory.ongoingConstellations.Count)
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
                foreach (var constellation in SC_GameManager.instance.playerSkillInventory.ongoingConstellations)
                {
                    c.Remove(constellation);
                }
                selectedConstellation = c[Random.Range(0, c.Count)];
                randomSkill = selectedConstellation.GetRandomParentSkill(SC_GameManager.instance.playerSkillInventory.skillsOwned, selectedSkills);
            }
            else //Ongoing constellation skill
            {
                var r = Random.Range(1, 101);
                var i = selectedConstellation.skills.Keys.ToList().Count;

                foreach (var parentSkill in selectedConstellation.skills.Keys.ToList())
                {
                    if (SC_GameManager.instance.playerSkillInventory.CheckHasSkill(parentSkill)) i--;
                }

                if (i <= 0) r = 100;
                
                if (r > 70) //Get a child skill from a already owned parent skill and ongoing constellation
                {
                    randomSkill = selectedConstellation.GetRandomChildSkill(SC_GameManager.instance.playerSkillInventory.skillsOwned, selectedSkills);
                }
                else //Get a new parent skill from an ongoing constellation
                {
                    randomSkill = selectedConstellation.GetRandomParentSkill(SC_GameManager.instance.playerSkillInventory.skillsOwned, selectedSkills);
                }
            }
        }
        return randomSkill;
    }


    public void ResourceDropSelection(string source,out int a)
    {
        a = 0;
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
            SC_GameManager.instance.playerResourceInventory.AddResource(essenceFragment,1); 
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
                mediumRarityDropRate += lowRarityDropRate / 2;
                highRarityDropRate += lowRarityDropRate / 2;
                lowRarityDropRate = 0f;
            }
            else if (templeLevel+1 >6) //If the level of the temple is at max
            {
                mediumRarityDropRate += highRarityDropRate / 2;
                lowRarityDropRate += highRarityDropRate / 2;
                highRarityDropRate = 0f;
            }

            int r = Random.Range(1, 101);
            int resourceLevel = r <= highRarityDropRate ? templeLevel + 1 :
                (r > highRarityDropRate && r <= highRarityDropRate + mediumRarityDropRate) ? templeLevel :
                templeLevel - 1;

            dropRange = r <= highRarityDropRate ? highRarityDropRange :
                (r > highRarityDropRate && r <= highRarityDropRate + mediumRarityDropRate) ? mediumRarityDropRange :
                lowRarityDropRange;
            int amount = Mathf.RoundToInt(Random.Range(dropRange.x, dropRange.y) * quantityMultiplier);
            SC_GameManager.instance.playerResourceInventory.AddResource(ressources.First(ressource => ressource.rarityLevel==resourceLevel),amount);
            a = amount;
        }
    }
    
}
