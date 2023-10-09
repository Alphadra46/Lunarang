using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LootTable", fileName = "LootTable_")]
public class SO_LootTable : SC_LootTable<GameObject> {}

[System.Serializable]
public class SO_LootTableConfig : Loot<GameObject> {}