using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SC_RewardUI : MonoBehaviour
{
    
    public List<SO_BaseSkill> rewardSkills = new List<SO_BaseSkill>();
    public List<SC_Ressource> rewardRessources = new List<SC_Ressource>();

    public GameObject rewardItemPrefab;

    public Transform rewardParent;
    
    private List<GameObject> rewards = new List<GameObject>();


    public void SetRessourcesRewards(List<SC_Ressource> ressources)
    {
        rewardRessources = ressources;
    }
    
    public void SetSkillsRewards(List<SO_BaseSkill> skills)
    {
        rewardSkills = skills;
    }

    private void RandomSkillReward()
    {
        var i = 0;
        var length = SC_SkillManager.instance.allCurrentRunSkills.Count > 2
            ? 3
            : SC_SkillManager.instance.allCurrentRunSkills.Count;
        
        while (i < length)
        {
            var index = Random.Range(0, SC_SkillManager.instance.allCurrentRunSkills.Count);

            if (rewardSkills.Contains(SC_SkillManager.instance.allCurrentRunSkills[index]))
            {
                continue;
            }
            
            rewardSkills.Add(SC_SkillManager.instance.allCurrentRunSkills[index]);
            i++;
        }
        
        print(rewardSkills.Count);
    }

    private void Awake()
    {

        RandomSkillReward();
        Init();

        EventSystem.current.SetSelectedGameObject(rewards[0]);
    }

    public void Init()
    {
        
        if (rewardSkills.Count > 0)
        {
            
            foreach (var skill in rewardSkills)
            {
                var reward = Instantiate(rewardItemPrefab, rewardParent).GetComponent<SC_RewardItemUI>();
                reward.SetTitle(skill.skillName);
                reward.SetDescription(skill.shortDescription);
                reward.SetColor(skill.constellation);
                
                rewards.Add(reward.gameObject);
            }
            
        }
        else if (rewardRessources.Count > 0)
        {
            
            foreach (var ressource in rewardRessources)
            {
                var reward = Instantiate(rewardItemPrefab, rewardParent).GetComponent<SC_RewardItemUI>();
                reward.SetTitle(ressource.name);
                reward.SetDescription(ressource.description);

                
                rewards.Add(reward.gameObject);
            }
            
        }

        foreach (var reward in rewards)
        {
            
            var btn = reward.GetComponent<Selectable>();
            var navigation = btn.navigation;
            
            navigation.mode = Navigation.Mode.Automatic;

        }
        
        
    }
    
    
}
