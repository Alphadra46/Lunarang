using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Middle-Men/Resources Inventory")]
public class SC_ResourcesInventory : SerializedScriptableObject
{
    public Dictionary<SC_Resource, int> resourceInventory = new Dictionary<SC_Resource, int>();

    public void AddResource(SC_Resource resource, int amount)
    {
        if (resourceInventory.ContainsKey(resource))
        {
            resourceInventory[resource] += amount;
        }
        else
        {
            resourceInventory.Add(resource, amount);
        }

        SC_NotificationManager.addRessourceNotification(resource, amount);

    }

    public void RemoveResource(SC_Resource resource, int amount)
    {
        if (resourceInventory[resource] - amount < 0)
        {
            return;
        }
        
        resourceInventory[resource] -= amount;
    }

    public bool CheckHasRessources(Dictionary<SC_Resource, int> costs)
    {

        var count = 0;
        
        foreach (var (ressource, amount) in costs)
        {
            
            if (!resourceInventory.TryGetValue(ressource, out var value))
                return false;

            if (value < amount) return false;
            
            count++;

        }
        
        return count >= costs.Count;

    }
    
    public bool CheckHasRessource(SC_Resource resource, int amount)
    {
        
        if (!resourceInventory.TryGetValue(resource, out var value))
            return false;

        return value >= amount;
        
    }
    
    [Button]
    public void ClearResourceInventory()
    {
        resourceInventory.Clear();
    }
    
    [Button]
    public void AddResourceInventory()
    {
        AddResource(Resources.Load<SC_Resource>("Ressources/Base/Ressource_1"), Random.Range(1 , 10));
    }
}
