using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        if (rewardRessources.Count > 0)
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
        
        EventSystem.current.SetSelectedGameObject(rewards[0]);
    }
    
    
}
