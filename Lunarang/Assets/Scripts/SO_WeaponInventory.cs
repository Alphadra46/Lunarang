using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Weapon Inventory")]
public class SO_WeaponInventory : SerializedScriptableObject
{
    public List<SC_Weapon> weaponsOwned = new List<SC_Weapon>();

    public void UnlockWeapon(SC_Weapon weapon)
    {
        weaponsOwned.Add(weapon);
    }
    
}

