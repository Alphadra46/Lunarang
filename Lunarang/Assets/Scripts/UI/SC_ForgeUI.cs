using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_ForgeUI : MonoBehaviour
{

    public GameObject leftInformationPanel;
    public GameObject rightInformationPanel;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject inputPromptsPanel;

    public SC_Weapon weaponTest;

    private void Awake()
    {
        
    }

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputPromptsPanel.GetComponent<RectTransform>());
    }


    public void UpgradeWeapon(SC_Weapon upgradedWeapon)
    {

        upgradedWeapon.currentLevel++;
        
    }

    public void UnlockWeapon(SC_Weapon unlockedWeapon)
    {
        
        
        
    }

    public void LoadWeaponInventory()
    {

        foreach (var VARIABLE in SC_GameManager.instance.)
        {
            
        }
        
    }

    
}
