using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

[Serializable]
public class SC_PoolStruct
{
    [Tooltip("Name of the pool, used to search for the pool when needed")]
    public string poolName;
    [Tooltip("The list of the prefabs that can be instantiated in this pool")] //TODO - Remove ShowInInspector and do a Custom Editor Script
    public SerializedDictionary<GameObject, int> prefabsList = new SerializedDictionary<GameObject, int>();

    [HideInInspector] public List<SC_SubPool> subPoolsList = new List<SC_SubPool>();

    private List<string> prefabTypes = new List<string>();
    private Transform poolParent;

    public void Initiate(Transform poolManagerParent)
    {
        int numberOfSubPools = 0;
        
        foreach (var prefab in prefabsList.Keys)
        {
            if (!prefabTypes.Contains(prefab.name))
            {
                prefabTypes.Add(prefab.name);
                numberOfSubPools++;
            }
        }
        
        GeneratePool(poolManagerParent, numberOfSubPools);
    }

    public void PreGenerateItems()
    {
        foreach (var prefab in prefabsList)
        {
            SC_SubPool subPoolToSearch = null;

            foreach (var subPool in subPoolsList)
            {
                if (subPool.poolType == prefab.Key.name)
                {
                    subPoolToSearch = subPool;
                    break;
                }
            }
            
            subPoolToSearch.PreGenerateItems(prefab.Key, prefab.Value);
        }
    }
    
    private void GeneratePool(Transform poolManagerParent, int numberOfSubPools)
    {
        var o = new GameObject();
        o.name = poolName;
        o.transform.parent = poolManagerParent;

        poolParent = o.transform;
        
        GenerateSubPools(numberOfSubPools);
    }
    
    /// <summary>
    /// Generate sub pools based on the amount needed
    /// </summary>
    /// <param name="numberOfSubPools">The amount of sub pool to create</param>
    private void GenerateSubPools(int numberOfSubPools)
    {
        for (int i = 0; i < numberOfSubPools; i++)
        {
            var o = new GameObject();
            o.transform.parent = poolParent;
            o.name = prefabTypes[i];
            
            subPoolsList.Add(new SC_SubPool(prefabTypes[i], o.transform, GetPrefabWithName(prefabTypes[i])));
        }
        
        PreGenerateItems();
    }

    /// <summary>
    /// Return a prefab in the list using its name
    /// </summary>
    /// <param name="name">Name of the prefab</param>
    /// <returns>The prefab in the list</returns>
    private GameObject GetPrefabWithName(string name)
    {
        GameObject prefabToGet = null;
        
        foreach (var prefab in prefabsList)
        {
            if (prefab.Key.name == name)
            {
                prefabToGet = prefab.Key;
                break;
            }
        }

        return prefabToGet;
    }

    public GameObject GetItem(string itemType)
    {
        SC_SubPool subPoolToSearch = null;

        foreach (var subPool in subPoolsList)
        {
            if (subPool.poolType == itemType)
            {
                subPoolToSearch = subPool;
                break;
            }
        }

        return subPoolToSearch.GetItem(itemType);
    }


    public void ReturnItemToPool(GameObject itemToReturn)
    {
        SC_SubPool subPoolToSearch = null;

        foreach (var subPool in subPoolsList)
        {
            if (subPool.poolType == itemToReturn.name)
            {
                subPoolToSearch = subPool;
                break;
            }
        }
        
        subPoolToSearch.ReturnItem(itemToReturn);
    }
}
