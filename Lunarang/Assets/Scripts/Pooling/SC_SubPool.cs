using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SubPool
{
    public string poolType;
    public GameObject objectPrefab;
    public Transform subPoolTransform;
    public Stack<GameObject> objectsInPool = new Stack<GameObject>();

    public SC_SubPool(string poolType, Transform subPoolTransform, GameObject objectPrefab)
    {
        this.poolType = poolType;
        this.subPoolTransform = subPoolTransform;
        this.objectPrefab = objectPrefab;
    }

    public void PreGenerateItems(GameObject item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var o = Object.Instantiate(objectPrefab, subPoolTransform);
            o.name = item.name;
            objectsInPool.Push(o);
            o.SetActive(false);
        }
    }
    
    public GameObject GetItem(string itemName)
    {
        
        if (objectsInPool.Count > 0)
        {
            objectsInPool.Peek().name = itemName;
            return objectsInPool.Pop();
        }
        
        objectsInPool.Push(Object.Instantiate(objectPrefab, subPoolTransform));
        objectsInPool.Peek().name = itemName;
        return objectsInPool.Pop();
        
    }

    public void ReturnItem(GameObject itemToReturn)
    {
        objectsInPool.Push(itemToReturn);
    }
    
}
