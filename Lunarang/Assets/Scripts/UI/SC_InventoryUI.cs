using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
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
    
    public RectTransform leftCharacterSide;

    [PropertySpace(SpaceBefore = 15f)] public TextMeshProUGUI timeTMP;
    [PropertySpace(SpaceBefore = 5f)] public TextMeshProUGUI killsTMP;
    [PropertySpace(SpaceBefore = 5f)] public TextMeshProUGUI fragmentsDayTMP;
    [PropertySpace(SpaceBefore = 5f)] public TextMeshProUGUI fragmentsNightTMP;
    
    [PropertySpace(SpaceBefore = 15f)] public Transform leftStatsContent;
    private List<GameObject> leftStatsGOList = new List<GameObject>();
    [PropertySpace(SpaceBefore = 5f)] public Transform rightStatsContent;
    private List<GameObject> rightStatsGOList = new List<GameObject>();
    [PropertySpace(SpaceBefore = 5f)] public GameObject statTemplatePrefab;

    #region Skills

    [PropertySpace(SpaceBefore = 15f)]
    [ShowInInspector, ] private InventoryCategories category = InventoryCategories.All;
    [ShowInInspector] private List<GameObject> skillsGO = new List<GameObject>();
    
    [ShowInInspector] private List<SO_BaseSkill> skillsVisible = new List<SO_BaseSkill>();

    [ShowInInspector] private List<SO_ParentSkill> _parentSkills = new List<SO_ParentSkill>();
    
    [ShowInInspector] private List<SO_BaseSkill> _dotSkills = new List<SO_BaseSkill>();
    [ShowInInspector] private List<SO_BaseSkill> _berserkSkills = new List<SO_BaseSkill>();
    [ShowInInspector] private List<SO_BaseSkill> _tankSkills = new List<SO_BaseSkill>();
    [ShowInInspector] private List<SO_BaseSkill> _freezeSkills = new List<SO_BaseSkill>();

    #endregion

    private void Awake()
    {
        
        Init();

    }

    private void Init()
    {
       
        InitStats();
        
    }

    private void Update()
    {
        RefreshUI();
    }

    private void InitStats()
    {

        ClearStats();
        
        var statsToShowLeft = new List<string>() { "PV", "ATK", "DEF", "SPD" };
        var statsToShowRight = new List<string>() { "DMG", "TC", "DC", "CA" };

        foreach (var stat in statsToShowLeft)
        {

            var statName = stat switch
            {
                "PV" => "POINTS DE VIE",
                "ATK" => "ATTAQUE",
                "DEF" => "DÉFENSE",
                "SPD" => "VITESSE",

                _ => "-"
            };
            
            var statValue = stat switch
            {

                "PV" => SC_PlayerStats.instance.currentStats.currentMaxHealth.ToString(),
                "ATK" => SC_PlayerStats.instance.currentStats.currentATK.ToString(),
                "DEF" => SC_PlayerStats.instance.currentStats.currentDEF.ToString(),
                "SPD" => SC_PlayerStats.instance.currentStats.currentSpeed.ToString(),
                
                _ => "-"
            };
            
            var statGO = Instantiate(statTemplatePrefab, leftStatsContent);
            if(statGO.TryGetComponent(out SC_InventoryStatTemplate statTemplate)) statTemplate.Init(stat, statName, statValue, false); 

        }
        
        foreach (var stat in statsToShowRight)
        {

            var statName = stat switch
            {
                "DMG" => "BONUS DE DÉGÂTS",
                "TC" => "TAUX CRITIQUE",
                "DC" => "DÉGÂTS CRITIQUE",
                "CA" => "CHANCE D'APPLIQUER",
                
                _ => "-"
                
            };
            
            var statValue = stat switch
            {

                "TC" => SC_PlayerStats.instance.currentStats.critRate + "%",
                "DC" => SC_PlayerStats.instance.currentStats.critDMG + "%",
                "CA" => "-",
                "DMG" => SC_PlayerStats.instance.currentStats.damageBonus + "%",

                _ => "-"
            };
            
            var statGO = Instantiate(statTemplatePrefab, rightStatsContent);
            if(statGO.TryGetComponent(out SC_InventoryStatTemplate statTemplate)) statTemplate.Init(stat, statName, statValue, true); 

        }

    }

    private void ClearStats()
    {
        
        if (leftStatsGOList.Count < 0) return;
        foreach (var go in leftStatsGOList)
        {
            Destroy(go);
        }
        leftStatsGOList.Clear();
        
        if (rightStatsGOList.Count < 0) return;
        foreach (var go in rightStatsGOList)
        {
            Destroy(go);
        }
        rightStatsGOList.Clear();
        
    }
    
    private void RefreshUI()
    {
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftCharacterSide);
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftStatsContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightStatsContent.GetComponent<RectTransform>());
        
    }
    
}
