using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Ressource"), Serializable]
public class SC_Ressource : ScriptableObject
{
    public string name;
    [TextArea] public string description;
    [Range(0,6)] public int rarityLevel;
    public Sprite sprite;
}
