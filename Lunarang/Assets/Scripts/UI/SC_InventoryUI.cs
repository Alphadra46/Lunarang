using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryCategories
{
    
    All,
    DoT,
    Berserker,
    Tank,
    Freeze
    
}


public class SC_InventoryUI : MonoBehaviour
{
    public GameObject ItemPrefab;
    public Transform contentParent;
    
    public Scrollbar scrollbar;
    private GridLayout _gridLayout;


    [ShowInInspector] private InventoryCategories category = InventoryCategories.All;
    [ShowInInspector] private List<GameObject> skillsGO = new List<GameObject>();
    
    [ShowInInspector] private List<SO_BaseSkill> skillsVisible = new List<SO_BaseSkill>();

    [ShowInInspector] private List<SO_ParentSkill> _parentSkills = new List<SO_ParentSkill>();
    
    [ShowInInspector] private List<SO_BaseSkill> _dotSkills = new List<SO_BaseSkill>();
    [ShowInInspector] private List<SO_BaseSkill> _berserkSkills = new List<SO_BaseSkill>();
    [ShowInInspector] private List<SO_BaseSkill> _tankSkills = new List<SO_BaseSkill>();
    [ShowInInspector] private List<SO_BaseSkill> _freezeSkills = new List<SO_BaseSkill>();

    private void Awake()
    {
        Init();
        
        scrollbar.Select();
    }

    private void Init()
    {
        if(SC_GameManager.instance.playerSkillInventory.skillsOwned.Count < 1) return;
        
        foreach (var skill in SC_GameManager.instance.playerSkillInventory.skillsOwned.Where(skill => skill.GetType() == typeof(SO_ParentSkill)))
        {
            _parentSkills.Add((SO_ParentSkill) skill);
        }

        foreach (var dotSkill in SC_GameManager.instance.playerSkillInventory.skillsOwned.Where(skill => skill.constellation == ConstellationName.DoT))
        {
            _dotSkills.Add(dotSkill);
        }
        
        foreach (var bSkill in SC_GameManager.instance.playerSkillInventory.skillsOwned.Where(skill => skill.constellation == ConstellationName.Berserker))
        {
            _berserkSkills.Add(bSkill);
        }
        
        foreach (var tSkill in SC_GameManager.instance.playerSkillInventory.skillsOwned.Where(skill => skill.constellation == ConstellationName.Tank))
        {
            _tankSkills.Add(tSkill);
        }
        
        foreach (var fSkill in SC_GameManager.instance.playerSkillInventory.skillsOwned.Where(skill => skill.constellation == ConstellationName.Freeze))
        {
            _freezeSkills.Add(fSkill);
        }
        
        
        CreateItems();
        ChangeCategory((int) InventoryCategories.All);
        
    }

    public void ChangeCategory(int newCategory)
    {
        skillsVisible.Clear();
        
        category = (InventoryCategories)newCategory;
        
        switch (category)
        {
            case InventoryCategories.All:
                skillsVisible = SC_GameManager.instance.playerSkillInventory.skillsOwned.ToList();
                break;
            case InventoryCategories.DoT:
                skillsVisible = _dotSkills.ToList();
                break;
            case InventoryCategories.Berserker:
                skillsVisible = _berserkSkills.ToList();
                break;
            case InventoryCategories.Tank:
                skillsVisible = _tankSkills.ToList();
                break;
            case InventoryCategories.Freeze:
                skillsVisible = _freezeSkills.ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        CreateItems();
    }

    private void CreateItems()
    {
        
        if(skillsGO.Count > 0) ClearItems();
        
        foreach (var skill in skillsVisible)
        {
            var skillGO = Instantiate(ItemPrefab, contentParent).GetComponent<SC_RewardItemUI>();
            skillGO.Init(skill, false);
                
            skillsGO.Add(skillGO.gameObject);
        }
        
    }

    private void ClearItems()
    {
        foreach (var go in skillsGO)
        {
            Destroy(go);
        }
        
        skillsGO.Clear();
    }

}
