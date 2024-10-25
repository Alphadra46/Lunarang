using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class SC_LootTable<T> : ScriptableObject where T : Object
{
    public List<Loot<T>> lootTable = new List<Loot<T>>();
    public List<Loot<T>> lootTableSave = new List<Loot<T>>();

    public bool saveDoneOnce = false;


    public float GetChanceFor(Loot<T> dropConfig)
    {
        if (dropConfig == null)
            return 0f;
        return (float)dropConfig.Probability / OverallDropProbability;
    }

    public int OverallDropProbability
    {
        get
        {
            return lootTable.Aggregate(0, (acc, x) => acc + x.Probability);
        }
    }

    public List<T> GetDrops(int count)
    {
        var result = new List<T>();
        for (int i = 0; i < count; i++)
        {
            var drop = GetDrop();
            if (drop != null) result.AddRange(drop);
        }

        return result;
    }

    public List<T> GetDrop()
    {
        if (!saveDoneOnce)
        {
            //lootTableSave.AddRange(lootTable);
            lootTableSave = lootTable.ToList();
        }
        saveDoneOnce = true;
        var result = new List<T>();
        var roll = Random.Range(0, OverallDropProbability);

        var adjustedRoll = roll;
        for (int i = 0; i < lootTable.Count; i++)
        {
            var dropConfig = lootTable[i];
            adjustedRoll -= dropConfig.Probability;
            
            if (adjustedRoll > 0) continue;

            var count = 1;
            for (int j = 0; j < count; j++)
            {
                result.Add(dropConfig.Drop);
            }
            break;
        }
        
        return result;
    }

    public void ResetLootTable()
    {
        lootTable = lootTableSave.ToList();
    }

    public void SimulateDrop()
    {
        Debug.Log("Simulating Drop for " + name);

        var drops = GetDrops(1);
        
        var line = "Drops: ";
        drops.ForEach(drop => line += drop + ", ");
        Debug.Log(line);
    }

}

[System.Serializable]
public class Loot<T> where T : Object
{
    [Tooltip("Between 0 and 100")]
    public int Probability;
    public T Drop;
    
    public override string ToString() {
        if (Drop == null) return "Missing Assignment (!)";
        var count = string.Format("1");

        return Drop.name +" " +count;
    }
}
