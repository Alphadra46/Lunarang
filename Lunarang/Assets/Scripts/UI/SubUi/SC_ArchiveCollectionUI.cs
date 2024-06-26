using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_ArchiveCollectionUI : MonoBehaviour
{
    
    public GameObject archivePrefab;
    
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;

    public Transform archivesParent;
    
    public Transform normalEnemiesParent;
    public Transform eliteEnemiesParent;
    public Transform bossEnemiesParent;

    public List<SC_ArchiveButtonUI> archivesGO = new List<SC_ArchiveButtonUI>();

    public GameObject standardCollectionPanel;

    public SO_ArchiveCollection collection;

    public void InitStantardCollection(SO_ArchiveCollection newCollection)
    {
        
        standardCollectionPanel.SetActive(true);
        
        collection = newCollection;
        
        title.text = collection.collectionName;

        desc.text = collection.collectionShortDesc;

        foreach (var archive in collection.allArchives)
        {
            
            var archiveGO = Instantiate(archivePrefab, archivesParent).GetComponent<SC_ArchiveButtonUI>();
            
            archiveGO.Init(archive);
            archivesGO.Add(archiveGO);
        }

        if (!collection.hasBonusArchive) return;
        
        var specialGO = Instantiate(archivePrefab, archivesParent).GetComponent<SC_ArchiveButtonUI>();
        
        specialGO.Init(collection.bonusArchive);
        archivesGO.Add(specialGO);
    }
    
    public void InitEnemiesCollection(SO_ArchiveCollection newCollection)
    {
        
        standardCollectionPanel.SetActive(true);
        
        collection = newCollection;
        
        title.text = collection.collectionName;

        desc.text = collection.collectionShortDesc;

        foreach (var archive in collection.allArchives)
        {
            
            var archiveGO = Instantiate(archivePrefab, archive.enemiesType switch
            {
                "Normal" => normalEnemiesParent,
                "Elite" => eliteEnemiesParent,
                _ => bossEnemiesParent
            }).GetComponent<SC_ArchiveButtonUI>();

            archiveGO.Init(archive);
            archivesGO.Add(archiveGO);
        }

    }

}
