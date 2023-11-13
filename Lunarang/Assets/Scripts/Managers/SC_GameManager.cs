using System.Collections.Generic;
using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager instance;
    
    #region Variables

    public List<GameObject> prefabsEntities = new List<GameObject>();
    

    #endregion


    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
    }
}