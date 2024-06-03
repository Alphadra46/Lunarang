using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_ForgeUI : MonoBehaviour
{

    #region Variables

    #region Actions

    public Action<SC_ForgeInventorySlotUI, bool> switchSelectedInventory;
    public Action<SC_ForgeEquippedSlotUI, bool> switchSelectedEquipped;
    public Action<SC_ForgeInventoryTypeButton, bool, ParameterType> switchInventoryType;

    public Action onViewChange;
    
    #endregion

    #region References

    public GameObject headerPanel;
    
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject leftInformationPanel;
    public GameObject leftTopInformationPanel;
    
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject rightInformationPanel;
    public GameObject rightUpgradePanel;
    public GameObject rightTopInformationPanel;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject inputPromptsPanel;

    [PropertySpace(SpaceBefore = 15f)]
    public Transform weaponInventoryContent;
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject weaponInventorySlotPrefab;
    [PropertySpace(SpaceBefore = 15f)]
    public Transform weaponEquippedContent;
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject weaponEquippedSlotPrefab;
    
    [PropertySpace(SpaceBefore = 15f)]
    public Button upgradeButton;
    [PropertySpace(SpaceBefore = 15f)]
    public SC_InputPrompt equipButtonPrompt;


    [PropertySpace(SpaceBefore = 15f)]
    public Transform resourcesContent;
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject resourcesPrefab;
    
    [PropertySpace(SpaceBefore = 15f)]
    public Transform upgradeCostContent;
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject upgradeCostPrefab;
    
    #endregion

    private ParameterType inventoryType = ParameterType.Projectile;
    
    private SC_ForgeInventorySlotUI weaponLeftPanel;
    private SC_ForgeEquippedSlotUI weaponRightPanel;

    #region Lists
    
    [BoxGroup("Lists")]
    [PropertySpace(SpaceBefore = 5f),ShowInInspector]
    private List<GameObject> weaponInventorySlots = new List<GameObject>();
    [BoxGroup("Lists")]
    [PropertySpace(SpaceBefore = 5f),ShowInInspector]
    private List<GameObject> weaponEquippedSlots = new List<GameObject>();
    [BoxGroup("Lists")]
    [PropertySpace(SpaceBefore = 5f),ShowInInspector]
    public List<SC_ForgeInventoryTypeButton> buttonsInventoryType = new List<SC_ForgeInventoryTypeButton>();

    private List<GameObject> upgradeCostGOList = new List<GameObject>();

    #endregion

    private bool viewState = false; // false = Inventory, true = Equipped
    
    #endregion

    private void OnEnable()
    {
        switchSelectedInventory += SwitchSelectedInventory;
        switchSelectedEquipped += SwitchSelectedEquipped;
        switchInventoryType += SwitchInventoryType;

        SC_InputManager.instance.switchToLeft.started += SwitchToLeftType;
        SC_InputManager.instance.switchToRight.started += SwitchToRightType;
        
        SC_InputManager.instance.switchView.started += SwitchView;
        SC_InputManager.instance.develop.started += UpgradeInput;
        SC_InputManager.instance.cancel.started += CloseInput;
        SC_InputManager.instance.submit.started += EquipUnequipInput;
    }

    private void OnDisable()
    {
        switchSelectedInventory -= SwitchSelectedInventory;
        switchSelectedEquipped -= SwitchSelectedEquipped;
        switchInventoryType -= SwitchInventoryType;
        
        SC_InputManager.instance.switchToLeft.started -= SwitchToLeftType;
        SC_InputManager.instance.switchToRight.started -= SwitchToRightType;

        SC_InputManager.instance.switchView.started -= SwitchView;
        SC_InputManager.instance.develop.started -= UpgradeInput;
        SC_InputManager.instance.cancel.started -= CloseInput;
        SC_InputManager.instance.submit.started -= EquipUnequipInput;
    }


    private void Start()
    {
        
        RefreshUI();
        
        LoadWeaponsInventory();
        LoadWeaponsEquipped();
        LoadResources();
        
        SwitchInventoryType(buttonsInventoryType[0], true, ParameterType.Projectile);
        
    }

    public void Close()
    {

        if (!SC_GameManager.instance.weaponInventory.CheckEnoughWeapons(3))
        {
            
            print("ERROR - Please Take 3 Weapons.");
            return;
            
        }
        
        SC_Lobby.instance.ShowLobby();
        SC_UIManager.instance.ShowForge();
        
        
    }


    #region Forge Functions

    public void UpgradeWeapon()
    {

        var upgradedWeapon = weaponLeftPanel.weapon;
        
        if(!upgradedWeapon.levelUpCosts.ContainsKey(upgradedWeapon.currentLevel)) return;
        
        if(!SC_GameManager.instance.playerResourceInventory.CheckHasRessources(upgradedWeapon.levelUpCosts[upgradedWeapon.currentLevel])) return;
        
        upgradedWeapon.Upgrade();
        
        LoadWeaponLeftInformationPanel(upgradedWeapon);
        LoadWeaponRightUpgradePanel(upgradedWeapon);
        
    }
    

    public void UnlockWeapon(SC_Weapon unlockedWeapon)
    {
        
        //TODO
        
    } 

    
    public void EquipWeapon()
    {
        
        if(!SC_GameManager.instance.weaponInventory.CheckCanEquip(weaponLeftPanel.weapon))
        {
            print("ERROR, CAN'T EQUIP");
            return;
        }

        
        SC_GameManager.instance.weaponInventory.EquipWeapon(weaponLeftPanel.weapon);
        
        RefreshWeaponEquipped();
        
    }

    public void UnequipWeapon()
    {

        if (SC_GameManager.instance.weaponInventory.CheckCanEquip(weaponRightPanel.weapon))
        {
            print("ERROR, CAN'T UNEQUIP");
            return;
        }
        
        var indexLastEquipped = weaponEquippedSlots.FindIndex(w => w == weaponRightPanel.gameObject);
        
        SC_GameManager.instance.weaponInventory.UnequipWeapon(weaponRightPanel.weapon);
        
        RefreshWeaponEquipped();
        
        weaponRightPanel = null;
        
        if(weaponEquippedSlots[indexLastEquipped].TryGetComponent(out Selectable weaponZeroSelectable)) weaponZeroSelectable.Select();
        
    } 

    #endregion

    
    
    private void RefreshInventory(ParameterType newInventoryType)
    {

        inventoryType = newInventoryType;
        
        foreach (var slot in weaponInventorySlots.Where(w => w.GetComponent<SC_ForgeInventorySlotUI>().weapon.parameter != inventoryType))
        {
            
            slot.SetActive(false);
            
        }

        var index = 0;
        
        foreach (var slot in weaponInventorySlots.Where(w => w.GetComponent<SC_ForgeInventorySlotUI>().weapon.parameter == inventoryType))
        {
            
            slot.SetActive(true);
            
            if(!slot.TryGetComponent(out Selectable selectable)) return;

            var activeList = weaponInventorySlots
                .Where(w => w.GetComponent<SC_ForgeInventorySlotUI>().weapon.parameter == inventoryType).ToList();

            var nav = selectable.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnLeft = index > 0 ? activeList[index - 1].GetComponent<Selectable>() : null;
            nav.selectOnRight = index < activeList.Count-1 ? activeList[index + 1].GetComponent<Selectable>() : null;

            selectable.navigation = nav;

            index++;
        }
        
    }
    
    public void LoadWeaponsInventory()
    {
        var index = 0;

        foreach (var weapon in SC_GameManager.instance.weaponInventory.weaponsOwned)
        {
            var weaponGO = Instantiate(weaponInventorySlotPrefab, weaponInventoryContent);
            weaponGO.GetComponent<SC_ForgeInventorySlotUI>().Init(this, weapon);
            weaponGO.name = "Inventory_" + index;
            
            weaponInventorySlots.Add(weaponGO);
            index++;
        }

        RefreshInventory(inventoryType);

        RefreshUI();
        
        if(weaponInventorySlots[0].TryGetComponent(out Selectable selectable)) selectable.Select();
        
        LoadWeaponRightUpgradePanel(weaponInventorySlots[0].GetComponent<SC_ForgeInventorySlotUI>().weapon);
    }
    
    public void LoadWeaponsEquipped()
    {
        var index = 0;
        
        foreach (var weapon in SC_GameManager.instance.weaponInventory.weaponsEquipped)
        {
            var weaponGO = Instantiate(weaponEquippedSlotPrefab, weaponEquippedContent);
            weaponGO.GetComponent<SC_ForgeEquippedSlotUI>().Init(this, weapon);
            weaponGO.name = "Equipped_" + index;
            
            weaponEquippedSlots.Add(weaponGO);
            index++;
        }

        index = 0;
        
        foreach (var slot in weaponEquippedSlots)
        {
            
            if(!slot.TryGetComponent(out Selectable selectable)) return;

            var nav = selectable.navigation;

            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnLeft = index > 0 ? weaponEquippedSlots[index - 1].GetComponent<Selectable>() : null;
            nav.selectOnRight = index < weaponEquippedSlots.Count-1 ? weaponEquippedSlots[index + 1].GetComponent<Selectable>() : null;

            selectable.navigation = nav;

            index++;
            
        }

        RefreshUI();

    }

    public void RefreshWeaponEquipped()
    {

        foreach (var slotGO in weaponEquippedSlots)
        {
            Destroy(slotGO);
        }
        
        weaponEquippedSlots.Clear();
        
        LoadWeaponsEquipped();
        
    }

    public void LoadResources()
    {

        var index = 0;
        
        foreach (var resource in Resources.LoadAll<SC_Resource>("Ressources/Base"))
        {
            if(!SC_GameManager.instance.playerResourceInventory.resourceInventory.ContainsKey(resource)) return;
            
            var resourceGO = Instantiate(resourcesPrefab, resourcesContent);

            resourceGO.name = "Resource_" + index;
            
            resourceGO.GetComponent<SC_ForgeResourceSlot>().Init(resource, SC_GameManager.instance.playerResourceInventory.resourceInventory[resource]);

            index++;
        }

    }


    public void LoadWeaponLeftInformationPanel(SC_Weapon weapon)
    {
        
        RefreshUI();
        
        if(!leftTopInformationPanel.transform.GetChild(0).GetChild(0).TryGetComponent(out TextMeshProUGUI name)) return;
        if(!leftTopInformationPanel.transform.GetChild(1).GetChild(0).TryGetComponent(out Image img)) return;
        
        if(!leftInformationPanel.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI type)) return;
        if(!leftInformationPanel.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI shortDesc)) return;
        if(!leftInformationPanel.transform.GetChild(4).TryGetComponent(out TextMeshProUGUI stats)) return;
        if(!leftInformationPanel.transform.GetChild(6).TryGetComponent(out TextMeshProUGUI effect)) return;

        name.text = weapon.weaponName + " - " + weapon.currentLevel;
        img.sprite = weapon.icon;
        
        type.text = weapon.parameter.ToString();
        shortDesc.text = weapon.shortDesc;
        
        var maxProjectiles = weapon.parameter switch
        {
            ParameterType.MultiHit => weapon.projectilesNumbers * (2 * 1),
            ParameterType.AreaOfEffect => weapon.projectilesNumbers * (2 * 1),
            ParameterType.Projectile => weapon.projectilesNumbers * (3 * 1),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        stats.text = $"SPEED : {weapon.atkSpeed}" +
                     $"\nAoE SIZE : {weapon.areaSize}" +
                     $"\nPROJECTILES NB : {weapon.projectilesNumbers} - {maxProjectiles}" +
                     $"\nADDITONAL HITS : {weapon.hits}";
        effect.text = weapon.effectDesc;
        
        CheckIfUpgradeable();

    }
    
    public void LoadWeaponRightInformationPanel(SC_Weapon weapon)
    {
        rightUpgradePanel.SetActive(false);
        rightInformationPanel.SetActive(true);
        rightTopInformationPanel.SetActive(true);
        
        RefreshUI();
        
        if(!rightTopInformationPanel.transform.GetChild(0).GetChild(0).TryGetComponent(out TextMeshProUGUI name)) return;
        if(!rightTopInformationPanel.transform.GetChild(1).GetChild(0).TryGetComponent(out Image img)) return;
        
        if(!rightInformationPanel.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI type)) return;
        if(!rightInformationPanel.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI shortDesc)) return;
        if(!rightInformationPanel.transform.GetChild(4).TryGetComponent(out TextMeshProUGUI stats)) return;
        if(!rightInformationPanel.transform.GetChild(6).TryGetComponent(out TextMeshProUGUI effect)) return;

        name.text = weapon.weaponName + " - " + weapon.currentLevel;
        img.sprite = weapon.icon;

        type.text = weapon.parameter switch
        {
            ParameterType.MultiHit => "Multi Hit",
            ParameterType.AreaOfEffect => "Area of Effect",
            ParameterType.Projectile => "Projectile",
            _ => throw new ArgumentOutOfRangeException()
        };
        
        shortDesc.text = weapon.shortDesc;

        var maxProjectiles = weapon.parameter switch
        {
            ParameterType.MultiHit => weapon.projectilesNumbers * (2 * 1),
            ParameterType.AreaOfEffect => weapon.projectilesNumbers * (2 * 1),
            ParameterType.Projectile => weapon.projectilesNumbers * (3 * 1),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        stats.text = $"SPEED : {weapon.atkSpeed}" +
                     $"\nAoE SIZE : {weapon.areaSize}" +
                     $"\nPROJECTILES NB : {weapon.projectilesNumbers} - {maxProjectiles}" +
                     $"\nADDITONAL HITS : {weapon.hits}";
        effect.text = weapon.effectDesc;

    }

    public void LoadWeaponRightUpgradePanel(SC_Weapon weapon)
    {
        rightInformationPanel.SetActive(false);
        rightTopInformationPanel.SetActive(false);
        rightUpgradePanel.SetActive(true);
        
        ClearUpgradeCost();
        
        if(!rightUpgradePanel.transform.GetChild(0).TryGetComponent(out TextMeshProUGUI lvl)) return;
        if(!rightUpgradePanel.transform.GetChild(2).TryGetComponent(out TextMeshProUGUI stats)) return;
        if(!rightUpgradePanel.transform.GetChild(4).TryGetComponent(out TextMeshProUGUI cost)) return;

        lvl.text = "LEVEL " + (weapon.currentLevel + 1).ToString();
        
        var maxProjectiles = weapon.parameter switch
        {
            ParameterType.MultiHit => weapon.projectilesNumbers * (2 * 1),
            ParameterType.AreaOfEffect => weapon.projectilesNumbers * (2 * 1),
            ParameterType.Projectile => weapon.projectilesNumbers * (3 * 1),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        stats.text = $"SPEED : {weapon.atkSpeed}" +
                     $"\nAoE SIZE : {weapon.areaSize}" +
                     $"\nPROJECTILES NB : {weapon.projectilesNumbers} - {maxProjectiles}" +
                     $"\nADDITONAL HITS : {weapon.hits}";

        if (weapon.levelUpCosts.Count > 0)
        {
            
            if (weapon.levelUpCosts.TryGetValue(weapon.currentLevel, out var upCost))
            {
                cost.color = Color.white;
                cost.text = "<b>UPGRADE COST</b>\n";
            
                foreach (var (resource, amount) in upCost)
                {

                    var costGO = Instantiate(upgradeCostPrefab, upgradeCostContent);
                    if(costGO.TryGetComponent(out SC_ForgeUpgradeCostText scCost)) scCost.Init(resource, amount);

                    upgradeCostGOList.Add(costGO);
                
                }
            }

        }
        
        RefreshUI();
        
    }

    private void ClearUpgradeCost()
    {

        foreach (var upgradeCost in upgradeCostGOList)
        {
            
            Destroy(upgradeCost);
            
        }
        
        upgradeCostGOList.Clear();
        
        RefreshUI();
        
    }

    
    private void CheckIfUpgradeable()
    {
        
        var upgradedWeapon = weaponLeftPanel.weapon;

        if (!upgradedWeapon.levelUpCosts.ContainsKey(upgradedWeapon.currentLevel))
        {
            upgradeButton.interactable =
                false;
            return;
        }
        
        upgradeButton.interactable =
            SC_GameManager.instance.playerResourceInventory.CheckHasRessources(
                upgradedWeapon.levelUpCosts[upgradedWeapon.currentLevel]);
        
    }

    
    #region Action Events

    private void SwitchSelectedInventory(SC_ForgeInventorySlotUI newSlotUI, bool value)
    {
        weaponLeftPanel = newSlotUI;
        
        foreach (var slots in weaponInventorySlots.Where(slots => slots != newSlotUI.gameObject))
        {
            slots.GetComponent<SC_ForgeInventorySlotUI>().SwitchState(false);
        }
        
        newSlotUI.SwitchState(value);

        LoadWeaponLeftInformationPanel(newSlotUI.weapon);
        
        if(weaponRightPanel != null) return;
        
        LoadWeaponRightUpgradePanel(weaponLeftPanel.weapon);
        
    }
    
    private void SwitchSelectedEquipped(SC_ForgeEquippedSlotUI newSlotUI, bool value)
    {
        weaponRightPanel = value ? newSlotUI : null;
        
        if(value) {
            foreach (var slots in weaponEquippedSlots.Where(slots => slots != newSlotUI.gameObject))
            {
                slots.GetComponent<SC_ForgeEquippedSlotUI>().SwitchState(false);
            }
        }
        
        newSlotUI.SwitchState(value);

        if(value)
            if (newSlotUI.weapon == null)
            {
                LoadWeaponRightUpgradePanel(weaponLeftPanel.weapon);
            }
            else
                LoadWeaponRightInformationPanel(newSlotUI.weapon);
        else
            LoadWeaponRightUpgradePanel(weaponLeftPanel.weapon);
    }
    
    private void SwitchInventoryType(SC_ForgeInventoryTypeButton selectedBtn, bool value, ParameterType newType)
    {
        foreach (var btn in buttonsInventoryType.Where(btn => btn != selectedBtn))
        {
            btn.SwitchState(false);
        }
        
        selectedBtn.SwitchState(value);

        RefreshInventory(newType);
    }
    
    #endregion
    
    
    #region Inputs Events

    private void SwitchToLeftType(InputAction.CallbackContext ctx)
    {
        if(viewState) return;

        switch (inventoryType)
        {
            case ParameterType.Projectile:
                break;
            case ParameterType.AreaOfEffect:
                SwitchInventoryType(buttonsInventoryType[0], true, ParameterType.Projectile);
                break;
            case ParameterType.MultiHit:
                SwitchInventoryType(buttonsInventoryType[1], true, ParameterType.AreaOfEffect);
                break;
        }
        
        var activeList = weaponInventorySlots.Where(slot => slot.activeSelf).ToList();
        activeList[0].GetComponent<Selectable>().Select();

    }
    
    private void SwitchToRightType(InputAction.CallbackContext ctx)
    {
        if(viewState) return;
        
        switch (inventoryType)
        {
            case ParameterType.Projectile:
                SwitchInventoryType(buttonsInventoryType[1], true, ParameterType.AreaOfEffect);
                break;
            case ParameterType.AreaOfEffect:
                SwitchInventoryType(buttonsInventoryType[2], true, ParameterType.MultiHit);
                break;
            case ParameterType.MultiHit:
                break;
        }
        
        var activeList = weaponInventorySlots.Where(slot => slot.activeSelf).ToList();
        activeList[0].GetComponent<Selectable>().Select();
        
    }
    
    private void SwitchView(InputAction.CallbackContext ctx)
    {

        viewState = !viewState;

        equipButtonPrompt.promptText = viewState ? "Unequip" : "Equip";

        if (viewState)
        {
            if(weaponRightPanel == null)
                if(weaponEquippedSlots[0].TryGetComponent(out Selectable weaponZeroSelectable)) weaponZeroSelectable.Select();
            else
                if(weaponRightPanel.TryGetComponent(out Selectable selectable)) selectable.Select();
        }
        else
        {
            if(weaponLeftPanel.TryGetComponent(out Selectable selectable)) selectable.Select();
        }
        
        onViewChange?.Invoke();
        
    }

    
    private void UpgradeInput(InputAction.CallbackContext ctx)
    {
        
        UpgradeWeapon();
        
    }

    
    private void EquipUnequipInput(InputAction.CallbackContext ctx)
    {
        
        if(viewState) UnequipWeapon();
        else EquipWeapon();
        
    }
    
    
    public void CloseInput(InputAction.CallbackContext ctx)
    {
        
        Close();
        
    }
    
    #endregion
    
    
    private void RefreshUI()
    {
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(headerPanel.GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightUpgradePanel.GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(upgradeCostContent.gameObject.GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftTopInformationPanel.transform.GetChild(0).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftTopInformationPanel.transform.GetChild(1).GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightTopInformationPanel.transform.GetChild(0).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightTopInformationPanel.transform.GetChild(1).GetComponent<RectTransform>());
            
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.transform.GetChild(0).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.transform.GetChild(2).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.transform.GetChild(4).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(leftInformationPanel.transform.GetChild(6).GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.transform.GetChild(0).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.transform.GetChild(2).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.transform.GetChild(4).GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(rightInformationPanel.transform.GetChild(6).GetComponent<RectTransform>());
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(inputPromptsPanel.GetComponent<RectTransform>());
        
    }
    
    
}
