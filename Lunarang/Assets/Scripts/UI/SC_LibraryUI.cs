using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_LibraryUI : MonoBehaviour
{
    #region Variables

    #region Actions

    public static Action<SO_Archive> showInformations;

    #endregion
    
    #region Prefabs
    
    [BoxGroup("Prefab")]
    public GameObject collectionSeparatorPrefab;
    [BoxGroup("Prefab")] public GameObject collectionPrefab;
    [BoxGroup("Prefab")]public GameObject enemiesCollectionPrefab;
    
    #endregion

    #region References

    [PropertySpace(SpaceBefore = 15f)]
    public GameObject collectionsContent;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject lockedContent;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject unlockedContent;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject unselectedContent;
    
    [PropertySpace(SpaceBefore = 15f)]
    public Image counterImage;
    [PropertySpace(SpaceBefore = 15f)]
    public Sprite counterSprite;
    [PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI counterText;
    

    #endregion

    #region Lists

    [BoxGroup("List")]
    public List<GameObject> collectionsGO = new List<GameObject>();
    [BoxGroup("List")]
    public List<GameObject> collectionsGOShowed = new List<GameObject>();

    #endregion
    
    public ArchiveType typeShowed = ArchiveType.Story;

    public Scrollbar scrollbar;

    #endregion

    
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
        
        SwitchInformationsPanelState("unselected");
        
    }

    public void Init()
    {
        
        foreach (var collection in SC_GameManager.instance.archivesInventory.collections)
        {
            
            var collectionGO = Instantiate(collectionPrefab, collectionsContent.transform);
            var collectionGOScript = collectionGO.GetComponent<SC_ArchiveCollectionUI>();
                
            if(collection.collectionType == ArchiveType.Enemies) collectionGOScript.InitEnemiesCollection(collection);
            else collectionGOScript.InitStantardCollection(collection);
                
            collectionsGO.Add(collectionGO);
            
        }
        
        

        counterImage.sprite = counterSprite;
        var rect = counterImage.GetComponent<RectTransform>().rect;
        rect.height = 50f;
        rect.width = 50f;
        
        counterText.text = SC_GameManager.instance.archivesInventory.GetNumbersOfDiscovoredArchives() + "/" +
                           SC_GameManager.instance.archivesInventory.GetNumbersOfArchives();
        
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
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(collectionsContent.GetComponent<RectTransform>());
        
    }

    public void BackToLobby()
    {
        
        SC_UIManager.instance.ShowLibrary();
        
    }

    public void ShowInformations(SO_Archive archiveToDisplay)
    {
        
        if (archiveToDisplay.archiveState is ArchiveState.Hidden)
        {
            SwitchInformationsPanelState("locked");
            
        }
        else
        {
            SwitchInformationsPanelState("unlocked");
            
            var collectionNameTMP = unlockedContent.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var archiveNameTMP = unlockedContent.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            var archiveImage = unlockedContent.transform.GetChild(3).GetComponent<Image>();
            var archiveDescTMP = unlockedContent.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            var archiveLoreTMP = unlockedContent.transform.GetChild(6).GetComponent<TextMeshProUGUI>();

            var collection =
                SC_GameManager.instance.archivesInventory.collections.FirstOrDefault(o =>
                    o.collectionID == archiveToDisplay.collectionID);

            if (collection != null)
            {
                collectionNameTMP.text = collection.collectionName;
                collectionNameTMP.CalculateLayoutInputVertical();
            }

            archiveNameTMP.text = archiveToDisplay.archiveName;
            archiveNameTMP.CalculateLayoutInputVertical();

            archiveImage.sprite = archiveToDisplay.splashArt;
            archiveImage.CalculateLayoutInputVertical();

            archiveDescTMP.text = archiveToDisplay.shortDescription;
            archiveDescTMP.CalculateLayoutInputVertical();

            archiveLoreTMP.text = archiveToDisplay.loreDescription;
            archiveLoreTMP.CalculateLayoutInputVertical();

            LayoutRebuilder.ForceRebuildLayoutImmediate(unlockedContent.GetComponent<RectTransform>());

        }
        
        
    }

    public void SwitchInformationsPanelState(string state)
    {

        switch (state)
        {
            case "unselected":
                lockedContent.SetActive(false);
                unlockedContent.SetActive(false);
                unselectedContent.SetActive(true);
                break;
            
            case "locked":
                unselectedContent.SetActive(false);
                unlockedContent.SetActive(false);
                lockedContent.SetActive(true);
                break;
            
            case "unlocked":
                unselectedContent.SetActive(false);
                lockedContent.SetActive(false);
                unlockedContent.SetActive(true);
                break;
            
            default:
                break;
        }
        
    }
    
}
