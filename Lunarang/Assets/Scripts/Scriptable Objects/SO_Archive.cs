using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ArchiveType
{
    
    Story,
    Weapon,
    Recipe,
    Enemies,
    Buildings
    
}

[CreateAssetMenu(menuName = "Ressources/Archive", fileName = "Archive")]
public class SO_Archive : SerializedScriptableObject
{
    
    public string id;

    public ArchiveType type; 

    [PropertySpace(SpaceBefore = 10f)]
    public string collectionID;
    
    [PropertySpace(SpaceBefore = 15f), TextArea]
    public string shortDescription;
    
    [PropertySpace(SpaceBefore = 15f), TextArea]
    public string loreDescription;

    #region Weapon & Enemies
    [PropertySpace(SpaceBefore = 25f)]
    [ShowIf("type", ArchiveType.Weapon)]
    public ParameterType weaponType;
    
    [ShowIf("@this.type != ArchiveType.Story"), TextArea]
    public string behaviorDescription;
    [ShowIf("@this.type != ArchiveType.Story")]
    public Sprite splashArt;
    
    [ShowIf("@this.type == ArchiveType.Enemies")]
    public string enemiesType;

    #endregion

    #region Story
    [PropertySpace(SpaceBefore = 25f)]
    [ShowIf("@this.type == ArchiveType.Story"), TextArea]
    public List<string> storyText = new List<string>();

    #endregion
    
    

}
