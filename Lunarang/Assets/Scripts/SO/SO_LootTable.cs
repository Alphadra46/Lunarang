using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/LootTable/Skill Loot Table", fileName = "LootTable_")]
public class SO_LootTable : SC_LootTable<SC_Skill> {}

[System.Serializable]
public class SO_LootTableConfig : Loot<SC_Skill> {}