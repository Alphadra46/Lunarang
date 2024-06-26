using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SC_ArchiveDiscoveredUI : MonoBehaviour
{

    [PropertySpace(SpaceBefore = 15f)]
    public TextMeshProUGUI archiveCollectionTMP;
    public TextMeshProUGUI archiveNameTMP;
    public TextMeshProUGUI archiveDescTMP;
    public TextMeshProUGUI archiveLoreTMP;

    [PropertySpace(SpaceBefore = 5f)]
    public GameObject firstPage;

    [PropertySpace(SpaceBefore = 15f)]
    public TextMeshProUGUI archiveOtherPagesTMP;

    [PropertySpace(SpaceBefore = 5f)]
    public GameObject otherPage;
    
    [PropertySpace(SpaceBefore = 5f)]
    public GameObject pagePanel;
    
    [PropertySpace(SpaceBefore = 5f)]
    public TextMeshProUGUI pageIndicatorTMP;

    [PropertySpace(SpaceBefore = 15f)]
    public SO_Archive archive;


    private int currentPage = 0;

    private void OnEnable()
    {
        
        SC_InputManager.instance.cancel.started += Close;
        
    }
    
    private void OnDisable()
    {
        
        SC_InputManager.instance.cancel.started -= Close;
        
    }

    private void Close(InputAction.CallbackContext ctx)
    {
        SC_GameManager.instance.OpenArchiveUI(archive);
    }

    public void Init(SO_Archive newArchive)
    {

        archive = newArchive;
        
        archiveCollectionTMP.text = SC_GameManager.instance.archivesInventory.collections.FirstOrDefault(o =>
                o.collectionID == archive.collectionID)?.collectionName;

        archiveNameTMP.text = archive.archiveName;
        archiveDescTMP.text = archive.shortDescription;
        archiveLoreTMP.text = archive.loreDescription;
        
        if(archive.splashArt != null)
        {
            firstPage.SetActive(true);
            
            if (archive.pagesText.Count > 0)
                pageIndicatorTMP.text = $"{currentPage + 1}/{archive.pagesText.Count+1}";
            
        }
        else {
            otherPage.SetActive(true);
            archiveOtherPagesTMP.text = archive.pagesText[currentPage];
            
            if (archive.pagesText.Count > 0)
                pageIndicatorTMP.text = $"{currentPage + 1}/{archive.pagesText.Count}";
        }
        
        pagePanel.SetActive(archive.pagesText.Count > 0);

    }

    public void SwitchPage()
    {

        if (currentPage == 0 && archive.splashArt != null)
        {
            firstPage.SetActive(true);
            otherPage.SetActive(false);
        }
        else
        {
            firstPage.SetActive(false);
            otherPage.SetActive(true);
            archiveOtherPagesTMP.text = archive.pagesText[currentPage];
        }
        
        
        if (archive.pagesText.Count > 0)
            pageIndicatorTMP.text = archive.splashArt != null ? $"{currentPage + 1}/{archive.pagesText.Count + 1}" : $"{currentPage+1}/{archive.pagesText.Count}";

    }

    public void SwitchToLeftPage()
    {
        if(currentPage == 0) return;

        currentPage--;
        SwitchPage();

    }
    
    public void SwitchToRightPage()
    {
        
        if(currentPage == archive.pagesText.Count-1) return;

        currentPage++;
        SwitchPage();

    }

    private void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) archiveOtherPagesTMP.transform.parent);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) archiveNameTMP.transform.parent);
    }
}
