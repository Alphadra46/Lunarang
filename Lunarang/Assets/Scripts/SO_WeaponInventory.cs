using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Archive Inventory")]
public class SO_WeaponInventory : SerializedScriptableObject
{
    public List<SC_Weapon> weaponsOwned = new List<SC_Weapon>();

    public void UnlockWeapon(SC_Weapon weapon)
    {
        weaponsOwned.Add(weapon);
    }
    
    public int GetNumbersOfDiscovoredArchives()
    {
        return archivesOwned.Count(archive => archive.archiveState != ArchiveState.Hidden);
    }
    
    public int GetNumbersOfArchives()
    {
        var cnt = collections.Sum(collection => collection.allArchives.Count);
        
        return cnt;
    }
    
    [Button]
    public void ClearArchiveOwnedInventory()
    {
        archivesOwned.Clear();
    }
    
    [Button]
    public void ResetArchiveOwnedState()
    {
        foreach (var archive in archivesOwned)
        {
            archive.archiveState = ArchiveState.New;
        }
    }
}

