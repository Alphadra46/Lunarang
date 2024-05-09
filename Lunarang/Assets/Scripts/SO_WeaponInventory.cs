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
        weaponsEquipped.Add(weapon);
    }
    
    public void UnequipWeapon(SC_Weapon weapon)
    {
        weaponsEquipped.Remove(weapon);
    }
    
}

