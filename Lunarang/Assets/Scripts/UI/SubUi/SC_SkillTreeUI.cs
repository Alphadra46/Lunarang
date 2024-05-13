using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = System.Object;

public class SC_SkillTreeUI : MonoBehaviour
{
    public static SC_SkillTreeUI instance;
    
    public int maxSP;
    [HideInInspector] public int currentSPLeft;

    private TextMeshProUGUI spText;
    [HideInInspector] public GameObject mainPage;

    public static Action<int> updateSP;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainPage = transform.GetChild(2).gameObject;
        currentSPLeft = maxSP;
        spText = transform.GetChild(1).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>();
        updateSP += UpdateSPText;

        var inv = Resources.Load<SO_SkillInventory>("SkillInventory");
        if (inv.skillsOwned.Count>0)
        {
            foreach (var skill in inv.skillsOwned)
            {
                currentSPLeft -= skill.spCost;
            }
        }

        spText.text = currentSPLeft.ToString();
    }

    private void UpdateSPText(int amount)
    {
        currentSPLeft += amount;
        spText.text = currentSPLeft.ToString();
    }
    
}
