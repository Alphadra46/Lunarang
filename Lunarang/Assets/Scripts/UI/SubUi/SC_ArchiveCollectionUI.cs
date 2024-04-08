using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SC_ArchiveCollectionUI : MonoBehaviour
{
    
    public GameObject archivePrefab;
    
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;

    public Transform archivesParent;

    public List<SC_ArchiveButtonUI> archivesGO = new List<SC_ArchiveButtonUI>();

    public SO_ArchiveCollection collection;

    public void Init(SO_ArchiveCollection newCollection)
    {
        collection = newCollection;
        
        title.text = collection.collectionName;

        desc.text = collection.collectionShortDesc;

        foreach (var archive in collection.allArchives)
        {
            var archiveGO = Instantiate(archivePrefab, archivesParent).GetComponent<SC_ArchiveButtonUI>();

            archiveGO.name.text = archive.archiveName;

        }

    }

}
