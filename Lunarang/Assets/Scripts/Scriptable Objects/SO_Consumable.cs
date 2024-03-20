using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum DurationType
{
    timeInSeconds,
    roomsExploration
}

[CreateAssetMenu(menuName = "Ressources/Consumable", fileName = "Consumable")]
public class SO_Consumable : ScriptableObject
{

    public string id;

    public List<SC_StatModification> dishesEffects = new();

    public int numberOfUses = 1;

    public DurationType durationType;
    public float duration;

}
