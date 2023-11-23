using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon/Weapon Template")]
public class SC_Weapon : ScriptableObject
{
    
    public WeaponType type;
    public ParameterType parameter;
    public List<float> MovesValues = new List<float>();
    
}
