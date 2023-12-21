using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/LootTable/Resource Loot Table", fileName = "ResourceLootTable_")]
public class SC_ResourceLootTable : SC_LootTable<SC_Resources> {}

[System.Serializable]
public class SC_ResourceLootTableConfig : Loot<SC_Resources> {}