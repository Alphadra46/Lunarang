using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/WaveSettings", fileName = "Wave_Settings"), Serializable]
public class SC_WaveSettings : ScriptableObject
{
    [Header("Easy settings")]
    public List<Vector2> numberOfEnemiesEasyPerWave;
    
    [Header("Medium settings")]
    public List<Vector2> numberOfEnemiesMediumPerWave;
    public List<Vector2> numberOfEliteEnemiesMediumPerWave;
    
    [Header("Hard settings")]
    public List<Vector2> numberOfEnemiesHardPerWave;
    public List<Vector2> numberOfEliteEnemiesHardPerWave;
}
