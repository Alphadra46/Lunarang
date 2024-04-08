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

            var collectionGO = Instantiate(collectionPrefab, collectionsContent.transform).GetComponent<SC_ArchiveCollectionUI>();

            collectionGO.Init(collection);
            
        }
        
    }

    public void SwitchType(ArchiveType newType)
    {

        typeShowed = newType;
        collectionsGOShowed.Clear();

        foreach (var collection in collectionsGO.FindAll(o => o.GetComponent<SC_ArchiveCollectionUI>().collection.collectionType == typeShowed))
        {
            collectionsGOShowed.Add(collection);
            collection.SetActive(false);
            
        }

    }
    
}
