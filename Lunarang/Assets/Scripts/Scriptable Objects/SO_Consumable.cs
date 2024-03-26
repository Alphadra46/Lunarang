using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;



public enum DurationType
{
    timeInSeconds,
    roomsExploration
}

[CreateAssetMenu(menuName = "Ressources/Consumable", fileName = "Consumable")]
public class SO_Consumable : SerializedScriptableObject
{

    public string id;

    [PropertySpace(SpaceBefore = 5f)]
    public List<SC_StatModification> dishesEffects = new();

    [PropertySpace(SpaceBefore = 5f)]
    public int numberOfUses = 1;

    [PropertySpace(SpaceBefore = 5f)]
    public DurationType durationType;
    public float duration;

}
