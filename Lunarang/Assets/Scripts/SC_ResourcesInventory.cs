using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Resources Inventory")]
public class SC_ResourcesInventory : ScriptableObject
{
    public List<SC_Resources> resourceInventory = new List<SC_Resources>();

    public void AddResource(SC_Resources resource)
    {
        if (resourceInventory.Contains(resource))
        {
            resource.quantity += resource.amount;
        }
        else
        {
            resourceInventory.Add(resource);
            resource.quantity += resource.amount;
        }
        
    }



}
