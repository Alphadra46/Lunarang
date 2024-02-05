using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Ressource"), Serializable]
public class SC_Ressource : ScriptableObject
{
    public string name;
    [TextArea] public string description;
    public ResourceRarity rarityLevel;
}

public enum ResourceRarity
{
    LVL1,
    LVL2,
    LVL3,
    LVL4,
}
