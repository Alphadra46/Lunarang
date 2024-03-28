using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Ressources/ArchiveCollection", fileName = "ArchiveCollection")]
public class SO_ArchiveCollection : SerializedScriptableObject
{

    public string collectionID;

    public ArchiveType collectionType;

    public List<SO_Archive> allArchives;
    
    public bool hasBonusArchive;
    [ShowIf("hasBonusArchive")]
    public SO_Archive bonusArchive;

}
