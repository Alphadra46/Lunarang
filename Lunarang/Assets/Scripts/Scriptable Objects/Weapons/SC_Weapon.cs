using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/Weapon Template")]
public class SC_Weapon : SerializedScriptableObject
{

    public string id;

    [TextArea] public string shortDesc;
    
    [PropertySpace(SpaceBefore = 5)]
    public GameObject weaponPrefab;
    
    [PropertySpace(SpaceBefore = 5)]
    public WeaponType type;
    public ParameterType parameter;
    public int TypeStrength = 1;
    
    [PropertySpace(SpaceBefore = 5)]
    public ImpactPoint impactPoint;
    
    [PropertySpace(SpaceBefore = 5)]
    [Range(0, 360)] public float areaRadius;
    public float areaSize;

    #region Moves Values

    [PropertySpace(SpaceBefore = 5)]
    public List<float> MovesValues = new List<float>();

    #endregion

    #region Level

    [PropertySpace(SpaceBefore = 5)] 
    public int currentLevel = 1;
    public float levelUpStatsRate = 0f;
    public Dictionary<int, List<SC_Ressource>> levelUpPrice = new Dictionary<int, List<SC_Ressource>>();

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
    public float projectileSpeed = 2.5f;

    #endregion

    [PropertySpace(SpaceBefore = 5)]
    [TextArea] public string effectDesc;
    public float effectValue = 0f;
    
    [Button]
    public void ResetStats()
    {

        currentLevel = 1;

    }

}
