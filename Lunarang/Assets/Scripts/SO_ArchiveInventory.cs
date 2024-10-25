﻿using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Archive Inventory")]
public class SO_ArchiveInventory : SerializedScriptableObject
{
    public List<SO_Archive> archivesOwned = new List<SO_Archive>();
    public List<SO_ArchiveCollection> collections = new List<SO_ArchiveCollection>();

    public void UnlockArchive(SO_Archive archive)
    {
        archive.archiveState = ArchiveState.New;
        archivesOwned.Add(archive);
    }
    
    public int GetNumbersOfDiscovoredArchives()
    {
        return archivesOwned.Count(archive => archive.archiveState != ArchiveState.Hidden);
    }
    
    public int GetNumbersOfArchives()
    {
        var cnt = collections.Sum(collection => collection.allArchives.Count);
        
        return cnt;
    }
    
    [Button]
    public void ClearArchiveOwnedInventory()
    {
        archivesOwned.Clear();
    }
    
    [Button]
    public void ResetArchiveOwnedState()
    {
        foreach (var archive in archivesOwned)
        {
            archive.archiveState = ArchiveState.New;
        }
    }
}

