using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SC_LibraryUI : MonoBehaviour
{

    #region Prefabs

    public GameObject collectionSeparatorPrefab;
    public GameObject collectionPrefab;
    
    #endregion
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject collectionsContent;

    public List<GameObject> collectionsGO = new List<GameObject>();
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

            Instantiate(collectionSeparatorPrefab, collectionsContent.transform);

            var collectionGO = Instantiate(collectionPrefab, collectionsContent.transform);
            var collectionGOScript = collectionGO.GetComponent<SC_ArchiveCollectionUI>();
            
            collectionGOScript.Init(collection);
            
            collectionsGO.Add(collectionGO);
            
            collectionGO.SetActive(false);
            
            SwitchType((int) typeShowed);
            
        }
        
    }

    public void SwitchType(int newType)
    {
        
        typeShowed = (ArchiveType) newType;
        collectionsGOShowed.Clear();
        
        foreach (var collection in collectionsGO)
        {
            
            collection.SetActive(false);

            if (collection.GetComponent<SC_ArchiveCollectionUI>().collection.collectionType != typeShowed) continue;
            
            collectionsGOShowed.Add(collection);
            collection.SetActive(true);


        }

    }
    
}
