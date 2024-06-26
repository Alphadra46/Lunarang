using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    public GameObject firstPage;
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject otherPage;

    public TextMeshProUGUI archiveOtherPagesTMP;
    public TextMeshProUGUI pageIndicatorTMP;

    
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

    private List<GameObject> typeButtons = new List<GameObject>();

    #endregion
    
    public ArchiveType typeShowed = ArchiveType.Story;

    public int currentPage = 0;
    public SO_Archive currentArchive;

    public Scrollbar scrollbar;

    #endregion

    
    private void OnEnable()
    {
        showInformations += ShowInformations;
        SC_InputManager.instance.cancel.started += Close;
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            typeButtons.Add(transform.GetChild(1).GetChild(i).gameObject);
        }
        
        if (collectionsGOShowed.Count>0&&collectionsGOShowed[0].GetComponent<SC_ArchiveCollectionUI>().archivesGO[0].gameObject)
            EventSystem.current.SetSelectedGameObject(collectionsGOShowed[0].GetComponent<SC_ArchiveCollectionUI>().archivesGO[0].gameObject);
        SC_InputManager.instance.switchToRight.started += SwitchTypeRight;
        SC_InputManager.instance.switchToLeft.started += SwitchTypeLeft;
        SwitchType((int) typeShowed);
    }

    private void OnDisable()
    {
        typeButtons.Clear();
        showInformations -= ShowInformations;
        SC_InputManager.instance.cancel.started -= Close;
        SC_InputManager.instance.switchToRight.started -= SwitchTypeRight;
        SC_InputManager.instance.switchToLeft.started -= SwitchTypeLeft;
    }

    private void Awake()
    {
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
        
        
        
        
    }
    
    public void SwitchTypeLeft(InputAction.CallbackContext context)
    {
        SwitchType(Mathf.Clamp((int)typeShowed-1,0,4));
    }
    
    public void SwitchTypeRight(InputAction.CallbackContext context)
    {
        SwitchType(Mathf.Clamp((int)typeShowed+1,0,4));
    }
    
    public void SwitchType(int newType)
    {
        foreach (var button in typeButtons)
        {
            button.transform.GetChild(1).gameObject.SetActive(false);
        }
        typeButtons[newType].transform.GetChild(1).gameObject.SetActive(true);
        typeShowed = (ArchiveType) newType;
        
        collectionsGOShowed.Clear();
        
        foreach (var collectionGO in collectionsGO)
        {
            
            collectionGO.SetActive(collectionGO.GetComponent<SC_ArchiveCollectionUI>().collection.collectionType == typeShowed);
            
            if(collectionGO.activeInHierarchy) collectionsGOShowed.Add(collectionGO);
            
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(collectionsContent.GetComponent<RectTransform>());

        if (collectionsGOShowed.Count>0&&collectionsGOShowed[0].GetComponent<SC_ArchiveCollectionUI>().archivesGO[0].gameObject)
        {
            EventSystem.current.SetSelectedGameObject(collectionsGOShowed[0].GetComponent<SC_ArchiveCollectionUI>().archivesGO[0].gameObject);
        }
        
        SetNavigation();
    }

    private void SetNavigation()
    {
        for (int i = 0; i < collectionsGOShowed.Count; i++)
        {
            var collection = collectionsGOShowed[i];
            var c = collection.GetComponent<SC_ArchiveCollectionUI>();
            for (int j = 0; j < c.archivesGO.Count; j++)
            {
                var archive = c.archivesGO[j];
                var archiveUIElement = archive.GetComponent<Button>();
                var nav = Navigation.defaultNavigation;
                nav.mode = Navigation.Mode.Explicit;
                nav.selectOnRight = j + 1 >= c.archivesGO.Count ? null : c.archivesGO[j + 1].GetComponent<Button>();
                nav.selectOnLeft = j - 1 < 0 ? null : c.archivesGO[j - 1].GetComponent<Button>();
                nav.selectOnDown = i + 1 >= collectionsGOShowed.Count
                    ? null
                    : collectionsGOShowed[i + 1].GetComponent<SC_ArchiveCollectionUI>().archivesGO[Mathf.Clamp(j,0,collectionsGOShowed[i + 1].GetComponent<SC_ArchiveCollectionUI>().archivesGO.Count - 1)].GetComponent<Button>();
                nav.selectOnUp = i - 1 < 0
                    ? null
                    : collectionsGOShowed[i - 1].GetComponent<SC_ArchiveCollectionUI>().archivesGO[Mathf.Clamp(j,0,collectionsGOShowed[i - 1].GetComponent<SC_ArchiveCollectionUI>().archivesGO.Count - 1)].GetComponent<Button>();
                archiveUIElement.navigation = nav;
            }
        }
    }
    
    private void Close(InputAction.CallbackContext ctx)
    {
        
        BackToLobby();
        
    }
    
    public void BackToLobby()
    {
        
        SC_Lobby.instance.ShowLobby();
        SC_UIManager.instance.ShowLibrary();
        
    }

    public void ShowInformations(SO_Archive archiveToDisplay)
    {

        currentArchive = archiveToDisplay;
        
        if (archiveToDisplay.archiveState is ArchiveState.Hidden)
        {
            SwitchInformationsPanelState("locked");
            
        }
        else
        {
            SwitchInformationsPanelState("unlocked");
            
            var collectionNameTMP = firstPage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var archiveNameTMP = firstPage.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            var archiveImage = firstPage.transform.GetChild(3).GetComponent<Image>();
            var archiveDescTMP = firstPage.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
            var archiveLoreTMP = firstPage.transform.GetChild(6).GetComponent<TextMeshProUGUI>();

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

            firstPage.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(unlockedContent.GetComponent<RectTransform>());

        }
        
        
    }

    public void SwitchPage()
    {
        
        if (currentPage == 0 && currentArchive.splashArt != null)
        {
            firstPage.SetActive(true);
            otherPage.SetActive(false);
        }
        else
        {
            firstPage.SetActive(false);
            otherPage.SetActive(true);
            archiveOtherPagesTMP.text = currentArchive.pagesText[currentPage];
        }
        
        
        if (currentArchive.pagesText.Count > 0)
            pageIndicatorTMP.text = $"{currentPage + 1}/{currentArchive.pagesText.Count+1}";
        
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
