using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/Weapon Template")]
public class SC_Weapon : SerializedScriptableObject
{

    #region Variables

    public string id;

    [PropertySpace(SpaceBefore = 5)]
    public string weaponName;
    
    [TextArea] public string shortDesc;
    public Sprite icon;
    
    [PropertySpace(SpaceBefore = 5)]
    public GameObject weaponPrefab;
    
    [PropertySpace(SpaceBefore = 5)]
    public ParameterType parameter;
    public float TypeStrength = 1;
    
    [PropertySpace(SpaceBefore = 5)]
    public ImpactPoint impactPoint;
    
    [PropertySpace(SpaceBefore = 5)]
    [Range(0, 360)] public float areaRadius;
    public float areaSize;

    #region Moves Values

    [PropertySpace(SpaceBefore = 5)]
    public List<float> baseMovesValues = new List<float>();

    #endregion

    #region Level

    [PropertySpace(SpaceBefore = 5)] 
    public int currentLevel = 1;
    public float levelUpStatsRate = 0f;
    
    [PropertySpace(SpaceBefore = 2.5f)] 
    public Dictionary<int, Dictionary<SC_Resource, int>> levelUpCosts = new Dictionary<int, Dictionary<SC_Resource, int>>();

    #endregion

    #region atkSpeed

    [PropertySpace(SpaceBefore = 5)]
    public float atkSpeed;

    #endregion

    #region MultipleHits

    [PropertySpace(SpaceBefore = 5)]
    public int hits;

    #endregion

    #region Projectiles

    [PropertySpace(SpaceBefore = 5)]
    public int projectilesNumbers;
    public GameObject projectilePrefab;

    #endregion

    [PropertySpace(SpaceBefore = 5)]
    [TextArea] public string effectDesc;
    public float effectValue = 0f;
    

    #endregion

    public void Upgrade()
    {

        currentLevel += 1;

    }
    
    [Button]
    public void ResetStats()
    {

        currentLevel = 1;

    }

}
