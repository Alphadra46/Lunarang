using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public enum ArchiveType
{
    
    Story,
    Weapon,
    Recipe,
    Enemies,
    Buildings
    
}

public enum ArchiveState
{
    
    Hidden,
    Discover,
    New,
    Special
    
}

[CreateAssetMenu(menuName = "Ressources/Archive", fileName = "Archive")]
public class SO_Archive : SerializedScriptableObject
{
    
    public string archiveID;
    
    public string archiveName;

    public ArchiveType archiveType;

    public ArchiveState archiveState;

    [PropertySpace(SpaceBefore = 10f)]
    public string collectionID;
    
    [PropertySpace(SpaceBefore = 15f), TextArea]
    public string shortDescription;
    
    [PropertySpace(SpaceBefore = 15f), TextArea]
    public string loreDescription;

    #region Weapon & Enemies
    [PropertySpace(SpaceBefore = 25f)]
    [ShowIf("archiveType", ArchiveType.Weapon)]
    public ParameterType weaponType;
    
    [ShowIf("@this.archiveType != ArchiveType.Story"), TextArea]
    public string behaviorDescription;
    [ShowIf("@this.archiveType != ArchiveType.Story")]
    public Sprite splashArt;
    
    [ShowIf("@this.archiveType == ArchiveType.Enemies")]
    public string enemiesType;

    #endregion

    #region Pages
    [PropertySpace(SpaceBefore = 25f)]
    [TextArea]
    public List<string> pagesText = new List<string>();

    #endregion
    
    

}
