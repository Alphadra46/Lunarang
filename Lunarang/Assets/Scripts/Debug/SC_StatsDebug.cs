using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_StatsDebug : MonoBehaviour
{
    
    public GameObject statsPrefab;

    public Dictionary<String, float> stats = new Dictionary<string, float>();

    private List<GameObject> statsInstantiated = new List<GameObject>();

    public void LoadStats()
    {
        
        stats.Add("Base ATK : ",SC_PlayerStats.instance.atkBase);
        stats.Add("Bonus ATK : ",SC_PlayerStats.instance.atkModifier);
        stats.Add("Current ATK : ",SC_PlayerStats.instance.currentATK);
        stats.Add("Base DEF : ",SC_PlayerStats.instance.defBase);
        stats.Add("Bonus DEF : ",SC_PlayerStats.instance.currentDEF);
        stats.Add("Current DEF : ",SC_PlayerStats.instance.currentDEF);
        stats.Add("Base CRIT Rate : ", SC_PlayerStats.instance.baseCritRate);
        stats.Add("Bonus CRIT Rate : ", SC_PlayerStats.instance.bonusCritRate);
        stats.Add("Current CRIT Rate : ", SC_PlayerStats.instance.critRate);
        stats.Add("Base CRIT DMG : ", SC_PlayerStats.instance.baseCritDMG);
        stats.Add("Bonus CRIT DMG : ", SC_PlayerStats.instance.bonusCritDMG);
        stats.Add("Current CRIT DMG : ", SC_PlayerStats.instance.critDMG);
        stats.Add("Bonus Damage % : ", SC_PlayerStats.instance.damageBonus);
        
    }

    public void InsantiateStats()
    {
        if(statsInstantiated.Count > 0)
            ClearStatsInstantiate();
        foreach (var stat in stats)
        {
            var statGO = Instantiate(statsPrefab, transform.GetChild(0)).GetComponent<TextMeshProUGUI>();
            statGO.text = stat.Key + stat.Value;
            statsInstantiated.Add(statGO.gameObject);
        }
    }

    public void ClearStatsInstantiate()
    {
        foreach (var sGameObject in statsInstantiated)
        {
            Destroy(sGameObject);
        }
        
        statsInstantiated.Clear();
    }

    public void RefreshStats()
    {
        stats.Clear();
        
        LoadStats();
        InsantiateStats();
    }
    
}
