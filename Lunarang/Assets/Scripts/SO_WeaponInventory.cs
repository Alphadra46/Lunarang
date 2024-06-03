using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Weapon Inventory")]
public class SO_WeaponInventory : SerializedScriptableObject
{
    public List<SC_Weapon> weaponsEquipped = new List<SC_Weapon>();
    public List<SC_Weapon> weaponsOwned = new List<SC_Weapon>();

    public void UnlockWeapon(SC_Weapon weapon)
    {
        weaponsOwned.Add(weapon);
    }
    
    public void EquipWeapon(SC_Weapon weapon)
    {
        if(!SC_GameManager.instance.weaponInventory.CheckCanEquip(weapon)) return;

        weaponsEquipped[weaponsEquipped.FindIndex(w => w == null)] = weapon;
        
    }
    
    public void UnequipWeapon(SC_Weapon weapon)
    {
        if(SC_GameManager.instance.weaponInventory.CheckCanEquip(weapon)) return;
        
        weaponsEquipped[weaponsEquipped.FindIndex(w => w == weapon)] = null;
    }

    public bool CheckCanEquip(SC_Weapon newWeapon)
    {

        var count = weaponsEquipped.Count(weapon => weapon == null);

        if (count < 0) return false;

        return !weaponsEquipped.Contains(newWeapon);
        
    }
    
    public bool CheckEnoughWeapons(int numberOfWeapons)
    {

        var count = weaponsEquipped.Count(weapon => weapon == null);

        return count == 0;
        
    }
    
}

