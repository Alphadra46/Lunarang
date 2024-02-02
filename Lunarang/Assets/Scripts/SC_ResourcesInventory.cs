using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Middle-Men/Resources Inventory")]
public class SC_ResourcesInventory : ScriptableObject
{
    public SerializedDictionary<SC_Ressource, int> resourceInventory = new SerializedDictionary<SC_Ressource, int>();

    public void AddResource(SC_Ressource resource, int amount)
    {
        if (resourceInventory.ContainsKey(resource))
        {
            resourceInventory[resource] += amount;
        }
        else
        {
            resourceInventory.Add(resource, amount);
        }
        
    }

    public void RemoveResource(SC_Ressource resource, int amount)
    {
        if (resourceInventory[resource] - amount < 0)
        {
            return;
        }
        
        resourceInventory[resource] -= amount;
    }
    
}
