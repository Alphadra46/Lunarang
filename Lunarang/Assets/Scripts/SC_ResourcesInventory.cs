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
    
    public void RemoveResources(Dictionary<SC_Resource, int> costs)
    {
        foreach (var (resource, amount) in costs)
        {
            
            if (!resourceInventory.TryGetValue(resource, out var value))
               return;

            if (value < amount) return;

            resourceInventory[resource] -= amount;

        }
    }

    public bool CheckHasResources(Dictionary<SC_Resource, int> costs)
    {

        var count = 0;
        
        foreach (var (resource, amount) in costs)
        {
            
            if (!resourceInventory.TryGetValue(resource, out var value))
                return false;

            if (value < amount) return false;
            
            count++;

        }
        
        return count >= costs.Count;

    }
    
    public bool CheckHasReource(SC_Resource resource, int amount)
    {
        
        if (!resourceInventory.TryGetValue(resource, out var value))
            return false;

        return value >= amount;
        
    }

    public int AmountOf(SC_Resource resource)
    {
        return resourceInventory.TryGetValue(resource, out var value) ? value : 0;
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
