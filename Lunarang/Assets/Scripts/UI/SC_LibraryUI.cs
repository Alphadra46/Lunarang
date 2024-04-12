using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SC_LibraryUI : MonoBehaviour
{

    #region Prefabs
    
    [BoxGroup("Prefab")]
    public GameObject collectionSeparatorPrefab;
    [BoxGroup("Prefab")]public GameObject collectionPrefab;
    
    #endregion
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject collectionsContent;
    
    [BoxGroup("List")]
    public List<GameObject> collectionsGO = new List<GameObject>();
    [BoxGroup("List")]
    public List<GameObject> collectionsGOShowed = new List<GameObject>();
    
    public ArchiveType typeShowed = ArchiveType.Story;

    public Scrollbar scrollbar;
        
    private void Awake()
    {
        scrollbar.Select();
        Init();
    }

    public void Init()
    {
        
        foreach (var collection in SC_GameManager.instance.archivesInventory.collections)
        {

            var collectionGO = Instantiate(collectionPrefab, collectionsContent.transform);
            var collectionGOScript = collectionGO.GetComponent<SC_ArchiveCollectionUI>();
            
            collectionGOScript.Init(collection);
            
            collectionsGO.Add(collectionGO);
        }
        
        SwitchType((int) typeShowed);
        
    }

    public void SwitchType(int newType)
    {
        
        typeShowed = (ArchiveType) newType;
        collectionsGOShowed.Clear();
        
        foreach (var collectionGO in collectionsGO)
        {
            
            collectionGO.SetActive(collectionGO.GetComponent<SC_ArchiveCollectionUI>().collection.collectionType ==
                                   typeShowed);
            
            if(collectionGO.activeInHierarchy) collectionsGOShowed.Add(collectionGO);
            
        }

    }

    public void BackToLobby()
    {
        
        Destroy(gameObject);
        
    }
    
}
