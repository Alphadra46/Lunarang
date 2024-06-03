using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_RewardUI : MonoBehaviour
{
    
    public List<SO_BaseSkill> rewardSkills = new List<SO_BaseSkill>();
    public List<SC_Resource> rewardRessources = new List<SC_Resource>();

    public GameObject rewardItemPrefab;

    public Transform rewardParent;
    public Transform rewardStart;
    
    [ShowInInspector, ReadOnly] private List<GameObject> rewards = new List<GameObject>();
    
    public List<SC_Selectable> rewardsSelectables = new List<SC_Selectable>();


    public void Show()
    {
        
        if(!TryGetComponent(out Animator _animator)) return;
        
        _animator.SetTrigger("Show");
        
    }
    
    public void Init()
    {
        var index = 0;
        
        if (rewardSkills.Count > 0)
        {
            
            foreach (var skill in rewardSkills)
            {
                var reward = Instantiate(rewardItemPrefab, rewardParent).GetComponent<SC_RewardItemUI>();
                reward.gameObject.gameObject.name = "Reward_" + index;
                reward.Init(skill);
                
                rewards.Add(reward.gameObject);
                index++;
            }

        }
        
        if (rewardRessources.Count > 0)
        {
            foreach (var ressource in rewardRessources)
            {
                var reward = Instantiate(rewardItemPrefab, rewardParent).GetComponent<SC_RewardItemUI>();
                reward.gameObject.gameObject.name = "Reward_" + index;
                reward.Init(ressource, 1);
                
                rewards.Add(reward.gameObject);
            }
            
        }

        foreach (var reward in rewards)
        {
            rewardsSelectables.Add(reward.GetComponent<SC_Selectable>());
            
            var btn = reward.GetComponent<Selectable>();
            var navigation = btn.navigation;
            
            navigation.mode = Navigation.Mode.Automatic;

            reward.GetComponent<RectTransform>().anchoredPosition =
                rewardStart.GetComponent<RectTransform>().anchoredPosition;
        }

        StartCoroutine(DelayBeforeCards(0.5f));
        
    }

    public void ShowAnimation()
    {
        var lastX = 225f;
        
        for (var i = 0; i < rewards.Count; i++)
        {
        
            var pos = rewards[i].GetComponent<RectTransform>().anchoredPosition;
            print(pos);
        
            pos.x = (i > 0 ? lastX : 225) + (i > 0 ? 12.5f : 0) + (i > 0 ? 450 : 0);
            lastX = pos.x;
            
            pos.y = 350;
        
            rewards[i].GetComponent<RectTransform>().DOAnchorPos(pos, (0.25f * (i+1))).SetUpdate(true);
            print(pos);
        
        }

        EventSystem.current.SetSelectedGameObject(rewards[0]);

    }
    public void DiscardOtherRewards(SC_Selectable selectedReward)
    {
        rewardsSelectables.Remove(selectedReward);
        foreach (var r in rewardsSelectables)
        {
            r.Discard();
        }
    }
    
    public IEnumerator DelayBeforeCards(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        ShowAnimation();

    }


    private void OnDestroy()
    {
        foreach (var reward in rewards)
        {
            Destroy(reward);
        }
        
        rewards.Clear();
        
        rewardSkills.Clear();
        rewardRessources.Clear();
    }
}
