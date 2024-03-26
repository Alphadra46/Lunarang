using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/Weapon Template")]
public class SC_Weapon : SerializedScriptableObject
{

    public string id;
    
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
    
    [PropertySpace(SpaceBefore = 5)]
    public List<float> MovesValues = new List<float>();

    [PropertySpace(SpaceBefore = 5)] 
    public int currentLevel = 1;
    public float levelUpStatsRate = 0f;
    public Dictionary<int, List<SC_Ressource>> levelUpPrice = new Dictionary<int, List<SC_Ressource>>();
    
    [PropertySpace(SpaceBefore = 5)]
    public float atkSpeed;
    
    [PropertySpace(SpaceBefore = 5)]
    public int hits;
    
    [PropertySpace(SpaceBefore = 5)]
    public int projectilesNumbers;
    public GameObject projectilePrefab;
    public float projectileSpeed = 2.5f;

    [Button]
    public void ResetStats()
    {

        currentLevel = 1;

    }

}
