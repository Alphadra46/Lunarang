using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/Weapon Template")]
public class SC_Weapon : ScriptableObject
{

    public string id;
    public WeaponType type;
    public ParameterType parameter;
    public int TypeStrength = 1;
    public ImpactPoint impactPoint;
    [Range(0, 360)] public float areaRadius;
    public float areaSize;
    public List<float> MovesValues = new List<float>();
    public float atkSpeed;
    public int hits;
    public int projectilesNumbers;

}
