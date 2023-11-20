using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager instance;
    
    #region Variables
    [Title("Settings")]
    [PropertySpace(SpaceBefore = 10)]
    public List<GameObject> prefabsEntities = new List<GameObject>();
    [PropertySpace(SpaceBefore = 10)]
    public List<Commands> commands = new List<Commands>();

    #endregion


    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
    }

    //
    // public bool FindEntity(string id, out GameObject entity)
    // {
    //     
    // }

    public bool CheckEntityType(string id)
    {
        var allEntities = FindObjectsOfType<SC_AIStats>().ToList();

        return allEntities.Count(e => e.typeID == id) > 0;
    }
    
    // public bool CheckEntity(string uid)
    // {
    //     var allEntities = FindObjectsOfType<SC_AIStats>().ToList();
    //     print(allEntities.Count);
    //
    //     return allEntities.Count(e => e.uid == int.Parse(uid)) > 0;
    // }
    
    public List<SC_AIStats> FindEntityType(string id)
    {
        var allEntities = FindObjectsOfType<SC_AIStats>().ToList();

        return CheckEntityType(id) ? allEntities.Where(e => e.typeID == id).ToList() : null;
    }

    // public List<SC_AIStats> FindEntity(string uid)
    // {
    //     var allEntities = FindObjectsOfType<SC_AIStats>().ToList();
    //     print(allEntities.Count);
    //
    //     return CheckEntity(uid) ? allEntities.Where(e => e.uid == int.Parse(uid)).ToList() : null;
    // }
}
