using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_ForgeUI : MonoBehaviour
{

    public GameObject leftInformationPanel;
    public GameObject rightInformationPanel;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject inputPromptsPanel;

    [PropertySpace(SpaceBefore = 15f)]
    public Transform weaponInventoryContent;
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject weaponInventorySlotPrefab;
    
    public SC_Weapon weaponTest;

    private List<GameObject> weaponInventorySlots = new List<GameObject>();
    
    private void Awake()
    {
        
    }

    private void Start()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputPromptsPanel.GetComponent<RectTransform>());
        
        LoadWeaponInventory();
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
        foreach (var weaponGO in SC_GameManager.instance.weaponInventory.weaponsOwned.Select(weapon => Instantiate(weaponInventorySlotPrefab, weaponInventoryContent)))
        {
            weaponInventorySlots.Add(weaponGO);
        }
    }

    
}
