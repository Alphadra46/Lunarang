using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    #region Variables

    public GameObject characterPage;
    public GameObject skillsPage;
    public GameObject mapPage;
    
    [PropertySpace(SpaceBefore = 15f)]
    public List<GameObject> buttonsPanel = new List<GameObject>();

    public Color normalColor;
    public Color selectedColor;
    public Color textSelectedColor;

    private int currentPageIndex;
    
    [PropertySpace(SpaceBefore = 15f)]
    public RectTransform leftCharacterSide;

    [PropertySpace(SpaceBefore = 15f)] public TextMeshProUGUI timeTMP;
    [PropertySpace(SpaceBefore = 5f)] public TextMeshProUGUI killsTMP;
    [PropertySpace(SpaceBefore = 5f)] public TextMeshProUGUI fragmentsDayTMP;
    [PropertySpace(SpaceBefore = 5f)] public TextMeshProUGUI fragmentsNightTMP;

    [PropertySpace(SpaceBefore = 15f)] public List<Image> WeaponSlots;

    #region Stats

    [PropertySpace(SpaceBefore = 15f)] public Transform leftStatsContent;
    [ShowInInspector] private List<GameObject> leftStatsGOList = new List<GameObject>();
    [PropertySpace(SpaceBefore = 5f)] public Transform rightStatsContent;
    private List<GameObject> rightStatsGOList = new List<GameObject>();
    [PropertySpace(SpaceBefore = 5f)] public GameObject statTemplatePrefab;

    #endregion

    #region Skills

    [PropertySpace(SpaceBefore = 15f)]
    public GameObject constellationTemplate;
    public Transform ConstellationTransform;
    public List<GameObject> constellationsGO;

    [PropertySpace(SpaceBefore = 15f)]
    public GameObject lunarTemplate;
    public Transform lunarTransform;

#endregion

    public List<Selectable> resourcesSelectables = new List<Selectable>();

    #endregion

    private void Awake()
    {
        
        Init();

    }

    private void Init()
    {
       
        InitStats();
        InitLog();
        InitWeapons();
        InitConstellationSkills();
        InitLunarSkills();
        
        InitNavigation();
        
        UpdatePageButtons();
        
    }

    private void Update()
    {
        RefreshUI();
    }

    public void ChangePage(int newIndex)
    {

        currentPageIndex = newIndex;

        switch (currentPageIndex)
        {
            
            case 0:
                characterPage.SetActive(true);
                skillsPage.SetActive(false);
                mapPage.SetActive(false);
                break;
            case 1:
                characterPage.SetActive(false);
                skillsPage.SetActive(true);
                mapPage.SetActive(false);
                break;
            case 2:
                characterPage.SetActive(false);
                skillsPage.SetActive(false);
                mapPage.SetActive(true);
                break;
            
        }

        UpdatePageButtons();

    }

    public void UpdatePageButtons()
    {
        foreach (var t in buttonsPanel)
        {
            var img = t.GetComponent<Image>();
            var text = t.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            
            
            img.color = normalColor;
            text.color = selectedColor;

        }

        buttonsPanel[currentPageIndex].GetComponent<Image>().color = selectedColor;
        buttonsPanel[currentPageIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = textSelectedColor;

    }

    #region Stats

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
            leftStatsGOList.Add(statGO);
            
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
            rightStatsGOList.Add(statGO);
            
            if(statGO.TryGetComponent(out SC_InventoryStatTemplate statTemplate)) statTemplate.Init(stat, statName, statValue, true); 

            
        }

    }

    private void InitNavigation()
    {

        //Left Stats
        for(var i = 0; i < leftStatsGOList.Count; i++)
        {

            var statGO = leftStatsGOList[i];

            if (!statGO.TryGetComponent(out Selectable selectable)) continue;
            
            var nav = selectable.navigation;

            nav.mode = Navigation.Mode.Explicit;
            if (i != 0) {
                if(!leftStatsGOList[i - 1].TryGetComponent(out Selectable upSelectable)) return;
                    
                nav.selectOnUp = i == 0 ? null : upSelectable;
            }
            else
            {
                if(!resourcesSelectables[0].TryGetComponent(out Selectable upSelectable)) return;

                nav.selectOnUp = upSelectable;
            }
                
            if(i != leftStatsGOList.Count-1){
                if(!leftStatsGOList[i + 1].TryGetComponent(out Selectable downSelectable)) return;
                    
                nav.selectOnDown = i == leftStatsGOList.Count ? null : downSelectable;
            }
            
            if(!rightStatsGOList[i].TryGetComponent(out Selectable rightSelectable)) return;

            nav.selectOnRight = rightSelectable;
            
           

            selectable.navigation = nav;

        }
        
        //Right Stats
        for(var i = 0; i < rightStatsGOList.Count; i++)
        {

            var statGO = rightStatsGOList[i];

            if (!statGO.TryGetComponent(out Selectable selectable)) continue;
            
            var nav = selectable.navigation;

            nav.mode = Navigation.Mode.Explicit;
            if (i != 0) {
                if(!rightStatsGOList[i - 1].TryGetComponent(out Selectable upSelectable)) return;
                    
                nav.selectOnUp = i == 0 ? null : upSelectable;
            } else
            {
                if(!resourcesSelectables[0].TryGetComponent(out Selectable upSelectable)) return;

                nav.selectOnUp = upSelectable;
            }
                
            if(i != rightStatsGOList.Count-1){
                if(!rightStatsGOList[i + 1].TryGetComponent(out Selectable downSelectable)) return;
                    
                nav.selectOnDown = i == rightStatsGOList.Count ? null : downSelectable;
            }
            
            if(!leftStatsGOList[i].TryGetComponent(out Selectable leftSelectable)) return;

            nav.selectOnLeft = leftSelectable;

            selectable.navigation = nav;

        }

        for (var i = 0; i < resourcesSelectables.Count; i++)
        {
            
            var nav = resourcesSelectables[i].navigation;
            
            nav.mode = Navigation.Mode.Explicit;
            
            if(!leftStatsGOList[0].TryGetComponent(out Selectable downSelectable)) return;
                
            nav.selectOnDown = downSelectable;
            
            if (i != 0) {
                if(!resourcesSelectables[i - 1].TryGetComponent(out Selectable leftSelectable)) return;
                    
                nav.selectOnLeft = i == 0 ? null : leftSelectable;
            }
                
            if(i != resourcesSelectables.Count-1){
                if(!resourcesSelectables[i + 1].TryGetComponent(out Selectable rightSelectable)) return;
                    
                nav.selectOnRight = i == resourcesSelectables.Count ? null : rightSelectable;
            }
            
            resourcesSelectables[i].navigation = nav;

        }

        for (var i = 0; i < constellationsGO.Count; i++)
        {
            
            if(!constellationsGO[i].TryGetComponent(out SC_InventoryConstellationManager manager)) return;

            for (int j = 0; j < manager.childsGO.Count; j++)
            {
                // manager.childsGO[j];
            }
            
        }
        
        EventSystem.current.SetSelectedGameObject(resourcesSelectables[0].gameObject);
        
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
    

    #endregion

    private void InitLog()
    {
        var timeSpan = TimeSpan.FromSeconds(SC_GameManager.instance.GetRunTimeElapsed());
        var hours = timeSpan.Hours;
        var minutes = timeSpan.Minutes;
        var seconds = timeSpan.Seconds;
        
        
        timeTMP.text = $"TEMPS : {hours:00}:{minutes:00}:{seconds:00}";

        killsTMP.text = "PURIFIÉS : WIP";

    }

    private void InitWeapons()
    {

        if(SC_GameManager.instance.weaponInventory.weaponsEquipped.Count < 1) return;
        
        for (var i = 0; i < WeaponSlots.Count; i++)
        {

            WeaponSlots[i].sprite = SC_GameManager.instance.weaponInventory.weaponsEquipped[i].icon;

        }

    }

    private void InitConstellationSkills()
    {

        var constellations = Resources.LoadAll<SC_Constellation>("Constellations").ToList();
        const int maxVisibleConstellations = 4;
        
        for (var i = 0; i < maxVisibleConstellations; i++)
        {
            var conste = constellations[i];

            var consteGO = Instantiate(constellationTemplate, ConstellationTransform);
            if (consteGO.TryGetComponent(out SC_InventoryConstellationManager manager)) manager.Init(conste, conste.name);

            consteGO.name = "Constellation_" + conste.name;
            consteGO.SetActive(true);
            constellationsGO.Add(consteGO);
        }
        
    }
    
    private void InitLunarSkills()
    {

        var lunarSkills = Resources.LoadAll<SO_LunarSkill>("Skills");

        foreach (var lunarSkill in lunarSkills)
        {

            var lunarGO = Instantiate(lunarTemplate, lunarTransform);
            if(lunarGO.TryGetComponent(out SC_InventoryLunarSkillTemplate lunarSC)) lunarSC.Init(lunarSkill);
            
            lunarGO.name = "LunarSkill_" + lunarSkill.skillName;
            lunarGO.SetActive(true);

        }
        
    }
    
    
    private void RefreshUI()
    {
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftCharacterSide);
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftStatsContent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightStatsContent.GetComponent<RectTransform>());
        
    }
    
}
