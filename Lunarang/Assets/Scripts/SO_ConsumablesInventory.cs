using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Consumable Inventory")]
public class SO_ConsumablesInventory : SerializedScriptableObject
{
    
    public List<SO_Consumable> consumablesInventory = new();

    public void AddConsumable(SO_Consumable consumable)
    {
        consumablesInventory.Add(consumable);
    }

    public void RemoveResource(SO_Consumable consumable)
    {
        consumablesInventory.Remove(consumable);
    }
    
    [Button]
    public void ClearConsumableInventory()
    {
        consumablesInventory.Clear();
    }
    
}
