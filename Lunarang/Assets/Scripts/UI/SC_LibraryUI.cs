using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_LibraryUI : MonoBehaviour
{

    #region Actions

    public static Action<SO_Archive> showInformations;

    #endregion
    
    #region Prefabs
    
    [BoxGroup("Prefab")]
    public GameObject collectionSeparatorPrefab;
    [BoxGroup("Prefab")]public GameObject collectionPrefab;
    
    #endregion
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject collectionsContent;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject lockedContent;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject unlockedContent;

    #region Lists

    [BoxGroup("List")]
    public List<GameObject> collectionsGO = new List<GameObject>();
    [BoxGroup("List")]
    public List<GameObject> collectionsGOShowed = new List<GameObject>();

    #endregion
    
    public ArchiveType typeShowed = ArchiveType.Story;

    public Scrollbar scrollbar;

    private void OnEnable()
    {
        showInformations += ShowInformations;
    }

    private void OnDisable()
    {
        showInformations -= ShowInformations;
    }

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

    public void ShowInformations(SO_Archive archiveToDisplay)
    {

        if (archiveToDisplay.archiveState is ArchiveState.Hidden)
        {
            
            unlockedContent.SetActive(false);
            lockedContent.SetActive(true);
            
        }
        else
        {
            lockedContent.SetActive(false);
            unlockedContent.SetActive(true);

            var collectionNameTMP = unlockedContent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var archiveNameTMP = unlockedContent.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            var archiveImage = unlockedContent.transform.GetChild(3).GetComponent<Image>();
            var archiveDescTMP = unlockedContent.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            var archiveLoreTMP = unlockedContent.transform.GetChild(6).GetComponent<TextMeshProUGUI>();

            var collection =
                SC_GameManager.instance.archivesInventory.collections.FirstOrDefault(o =>
                    o.collectionID == archiveToDisplay.collectionID);

            if (collection != null) collectionNameTMP.text = collection.collectionName;

            archiveNameTMP.text = archiveToDisplay.archiveName;

            archiveImage.sprite = archiveToDisplay.splashArt;

            archiveDescTMP.text = archiveToDisplay.shortDescription;

            archiveLoreTMP.text = archiveToDisplay.loreDescription;

        }
        
        
    }
    
}
