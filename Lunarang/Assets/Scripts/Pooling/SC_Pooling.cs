using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Pooling : MonoBehaviour
{
    public static SC_Pooling instance;
    
    [Tooltip("The list of every pool needed")]
    public List<SC_PoolStruct> poolList = new List<SC_PoolStruct>();


    // Start is called before the first frame update
    void Awake()
    {
        if (instance!=null)
            Destroy(instance);
        
        instance = this;

        Initiate();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Initiate()
    {
        foreach (var pool in poolList)
        {
            pool.Initiate(transform);
        }
    }

    /// <summary>
    /// Get a random item from a specific pool. The pool is found using it's name
    /// </summary>
    /// <param name="poolName">Name of the pool to retrieve the item from</param>
    public GameObject GetItemFromPool(string poolName, string itemType)
    {
        SC_PoolStruct poolToSearch = new SC_PoolStruct();

        //Search for the right pool using name as an ID
        foreach (var pool in poolList)
        {
            if (pool.poolName == poolName)
            {
                poolToSearch = pool;
                break;
            }
        }
        
        return poolToSearch.GetItem(itemType);
    }

    /// <summary>
    /// Return the object passed in parameter to its original pool.
    /// </summary>
    /// <param name="itemToReturn">The Game object to return</param>
    public void ReturnItemToPool(string poolName, GameObject itemToReturn)
    {
        SC_PoolStruct poolToSearch = new SC_PoolStruct();

        //Search for the right pool using name as an ID
        foreach (var pool in poolList)
        {
            if (pool.poolName == poolName)
            {
                poolToSearch = pool;
                break;
            }
        }
        
        poolToSearch.ReturnItemToPool(itemToReturn);
    }
    
}
