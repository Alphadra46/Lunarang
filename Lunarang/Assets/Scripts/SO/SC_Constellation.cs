using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Constellation")]
public class SC_Constellation : ScriptableObject //A Constellation is used to have a specific loot table attached to it so that is this constellation is followed by the player he will get guaranteed skills from the loot table attached
{
    public SC_LootTable<SC_Skill> constellationLootTable;
    //Temp
    public bool isFollowingThisConstellation;
}
