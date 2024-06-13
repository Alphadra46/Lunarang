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
    
    public RectTransform leftCharacterSide;
    
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
        RefreshUI();
        
        // scrollbar.Select();
    }

    private void Init()
    {
       
        RefreshUI();
        
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

    private void RefreshUI()
    {
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftCharacterSide);
        
    }
    
}
