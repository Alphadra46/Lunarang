using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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


    private InventoryCategories category = InventoryCategories.All;
    private List<GameObject> SkillsGO = new List<GameObject>();
    
    private List<SO_BaseSkill> skillsVisible = new List<SO_BaseSkill>();

    private List<SO_ParentSkill> _parentSkills = new List<SO_ParentSkill>();
    
    private List<SO_BaseSkill> _dotSkills = new List<SO_BaseSkill>();
    private List<SO_BaseSkill> _berserkSkills = new List<SO_BaseSkill>();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if(SC_SkillManager.instance.allEquippedSkills.Count < 1) return;
        
        foreach (var skill in SC_SkillManager.instance.allEquippedSkills.Where(skill => skill.GetType() == typeof(SO_ParentSkill)))
        {
            _parentSkills.Add((SO_ParentSkill) skill);
        }

        foreach (var dotSkill in SC_SkillManager.instance.allEquippedSkills.Where(skill => skill.constellation == ConstellationName.DoT))
        {
            _dotSkills.Add(dotSkill);
        }
        
        foreach (var bSkill in SC_SkillManager.instance.allEquippedSkills.Where(skill => skill.constellation == ConstellationName.Berserker))
        {
            _berserkSkills.Add(bSkill);
        }
        
        
        CreateItems();
        
    }

    public void ChangeCategory(InventoryCategories newCategory)
    {
        
        switch (category)
        {
            case InventoryCategories.All:
                skillsVisible = SC_SkillManager.instance.allEquippedSkills;
                break;
            case InventoryCategories.DoT:
                skillsVisible = _dotSkills;
                break;
            case InventoryCategories.Berserker:
                skillsVisible = _berserkSkills;
                break;
            case InventoryCategories.Tank:
                break;
            case InventoryCategories.Freeze:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    private void CreateItems()
    {
        
        if(SkillsGO.Count > 0) ClearItems();
        
        foreach (var skill in skillsVisible)
        {
            var skillGO = Instantiate(ItemPrefab, contentParent).GetComponent<SC_InventoryItemUI>();
            skillGO.SetTitle(skill.skillName);
            skillGO.SetDescription(skill.shortDescription);
                
            SkillsGO.Add(skillGO.gameObject);
        }
        
    }

    private void ClearItems()
    {
        SkillsGO.Clear();
    }

}
