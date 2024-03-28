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

    [Header("Hell settings")] 
    [Tooltip("The amount of enemies alive at the same time")] public int numberOfEnemiesAlive;
    [Tooltip("The total amount of enemies in the entire challenge")] public int totalOfEnemiesHell;
    [Tooltip("How many elite enemies can be alive at the same time")] public int maxEliteAlive;
}
