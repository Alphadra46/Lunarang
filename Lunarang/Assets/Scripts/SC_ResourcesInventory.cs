using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Middle-Men/Resources Inventory")]
public class SC_ResourcesInventory : SerializedScriptableObject
{
    public Dictionary<SC_Ressource, int> resourceInventory = new Dictionary<SC_Ressource, int>();

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

        SC_NotificationManager.addRessourceNotification(resource, amount);

    }

    public void RemoveResource(SC_Ressource resource, int amount)
    {
        if (resourceInventory[resource] - amount < 0)
        {
            return;
        }
        
        resourceInventory[resource] -= amount;
    }
    
    [Button]
    public void ClearResourceInventory()
    {
        resourceInventory.Clear();
    }
    
    [Button]
    public void AddResourceInventory()
    {
        AddResource(Resources.Load<SC_Ressource>("Ressources/Base/Ressource_1"), Random.Range(1 , 10));
    }
}
