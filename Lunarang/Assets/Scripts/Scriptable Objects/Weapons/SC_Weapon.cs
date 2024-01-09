using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/Weapon Template")]
public class SC_Weapon : ScriptableObject
{

    public string id;
    public WeaponType type;
    public ParameterType parameter;
    public int TypeStrength;
    public ImpactPoint impactPoint;
    public float rayon;
    public float areaSize;
    public List<float> MovesValues = new List<float>();
    public float atkSpeed;
    
    
}
