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

    #region Variables

    public GameObject leftInformationPanel;
    public GameObject rightInformationPanel;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject inputPromptsPanel;

    [PropertySpace(SpaceBefore = 15f)]
    public Transform weaponInventoryContent;
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject weaponInventorySlotPrefab;
    
    private int indexHoverWeapon;
    private int indexSelectedWeapon;

    private List<GameObject> weaponInventorySlots = new List<GameObject>();

    #endregion

    private void Start()
    {
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputPromptsPanel.GetComponent<RectTransform>());
        
        LoadWeaponInventory();
        LoadSelectedWeaponInformationPanel(SC_GameManager.instance.weaponInventory.weaponsEquipped[indexSelectedWeapon]);
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


    public void LoadSelectedWeaponInformationPanel(SC_Weapon weapon)
    {
        
        if(!leftInformationPanel.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI type)) return;
        if(!leftInformationPanel.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI shortDesc)) return;
        if(!leftInformationPanel.transform.GetChild(4).TryGetComponent(out TextMeshProUGUI stats)) return;
        if(!leftInformationPanel.transform.GetChild(6).TryGetComponent(out TextMeshProUGUI effect)) return;

        type.text = weapon.type.ToString();
        shortDesc.text = weapon.shortDesc;
        stats.text = $"VITESSE  {weapon.atkSpeed}\nAOE  {weapon.areaSize}\nNB PROJECTILES  {weapon.projectilesNumbers}\nNB COUPS  {weapon.hits}";
        effect.text = weapon.effectDesc;

    }

    public void Close()
    {
        
        SC_UIManager.instance.ShowForge();
        
    }

    
}
