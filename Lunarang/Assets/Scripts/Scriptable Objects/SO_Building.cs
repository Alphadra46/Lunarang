using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Building Template")]
public class SO_Building : SerializedScriptableObject
{

    public string buildingName;
    
    [PropertySpace(SpaceBefore = 2.5f)] 
    public string shortDesc;
    public string longDesc;

    [PropertySpace(SpaceBefore = 10f)] 
    public int currentLevel;
    public int maxLevel;
    
    [PropertySpace(SpaceBefore = 2.5f)] 
    public Dictionary<int, Dictionary<SC_Resource, int>> levelUpCosts = new Dictionary<int, Dictionary<SC_Resource, int>>();
    
    [PropertySpace(SpaceBefore = 2.5f)]
    public Dictionary<int, Sprite> spritesByLevel = new Dictionary<int, Sprite>();
    [PropertySpace(SpaceBefore = 2.5f)]
    public Dictionary<int, Sprite> outlinesByLevel = new Dictionary<int, Sprite>();


    public void Upgrade()
    {
        if (currentLevel >= maxLevel) return;
        
        if(!SC_GameManager.instance.playerResourceInventory.CheckHasRessources(levelUpCosts[currentLevel+1])) return;
        
        currentLevel++;
    }
    
}
