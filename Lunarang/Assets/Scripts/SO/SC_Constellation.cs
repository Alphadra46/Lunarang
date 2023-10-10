using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Constellation")]
public class SC_Constellation : ScriptableObject
{
    public SC_LootTable<SC_Skill> constellationLootTable;
    //Temp
    public bool isFollowingThisConstellation;
}
