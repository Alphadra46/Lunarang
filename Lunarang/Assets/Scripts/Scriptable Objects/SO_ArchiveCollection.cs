using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Ressources/ArchiveCollection", fileName = "ArchiveCollection")]
public class SO_ArchiveCollection : SerializedScriptableObject
{

    public string collectionID;
    
    [PropertySpace(SpaceBefore = 10f)]
    public string collectionName;
    [PropertySpace(SpaceBefore = 10f), Multiline]
    public string collectionShortDesc;
    
    [PropertySpace(SpaceBefore = 10f)]
    public ArchiveType collectionType;
    [PropertySpace(SpaceBefore = 15f)]
    public List<SO_Archive> allArchives;
    
    [PropertySpace(SpaceBefore = 15f)]
    public bool hasBonusArchive;
    [ShowIf("hasBonusArchive"), PropertySpace(SpaceBefore = 5f)]
    public SO_Archive bonusArchive;

}
