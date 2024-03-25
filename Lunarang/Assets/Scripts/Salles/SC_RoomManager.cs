using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_RoomManager : MonoBehaviour
{
    [Header("Room Type")] 
    public bool isSpecialRoom;
    [SerializeField, TabGroup("Settings", "Global Settings"), HideIf("isSpecialRoom")] private RoomSize roomSize;
    [SerializeField, ReadOnly, TabGroup("Settings", "Global Settings"), HideIf("isSpecialRoom")] private RoomDifficulty roomDifficulty;

    [Header("Stairs")] 
    [SerializeField, TabGroup("Settings", "Global Settings"), ShowIf("isSpecialRoom")] public SC_Stairs stair;
    
    [Header("Door possible position")]
    [TabGroup("Settings", "Global Settings")] public SC_Door doorNorth;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorSouth;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorWest;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorEast;

    [Header("Enemies spawn area")] 
    [TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] public List<GameObject> globalSpawnArea;
    [TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] public List<GameObject> eliteSpawnArea;

    [Header("Enemies wave parameters")]
    [SerializeField, TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private SC_WaveSettings waveSettings;
    [SerializeField, TabGroup("Settings", "Wave Settings"), ShowIf("@(roomSize==RoomSize.Large) && !isSpecialRoom")] private Vector2 numberOfEnemiesHell; //TODO - Later
    
    [Header("Other room parameters")] 
    [SerializeField, TabGroup("Settings", "Global Settings")] private Collider colliderConfiner;
    private CinemachineConfiner confiner;

    private List<SC_Door> activeDoors = new List<SC_Door>();
    [ShowInInspector] private List<GameObject> enemiesInRoom = new List<GameObject>();

    [ShowInInspector] private int numberOfEnemiesInWave;
    [ShowInInspector] private int numberOfEliteEnemiesInWave;
    [ShowInInspector] private int totalEnemiesInWave;
    [ShowInInspector] public bool isClear=false;

    private int actualWave = 0;
    private int numberOfWave = 0;
    private bool isInit = false;
    
    private enum RoomSize
    {
        Small,
        Medium,
        Large
    }
    
    private enum RoomDifficulty
    {
        Easy,
        Medium,
        Hard,
        Hell
    }
    
    private void OnEnable()
    {
        if (isInit)
            return;

        isInit = true;

        if (!isSpecialRoom)
            SetDifficulty();
        
        doorNorth.Initialize(this);
        doorSouth.Initialize(this);
        doorWest.Initialize(this);
        doorEast.Initialize(this);
        
        doorNorth.DisableDoor();
        doorSouth.DisableDoor();
        doorWest.DisableDoor();
        doorEast.DisableDoor();

        confiner = FindObjectOfType<CinemachineConfiner>();
    }

    public void SetDifficulty()
    {
        RoomDifficulty difficulty = RoomDifficulty.Easy;
        float rand = Random.Range(0, 100);
        switch (roomSize)
        {
            case RoomSize.Small:
                difficulty = RoomDifficulty.Easy;
                break;
            case RoomSize.Medium:
                difficulty = rand < 15 ? RoomDifficulty.Easy : rand < 70 ? RoomDifficulty.Medium : RoomDifficulty.Hard;
                break;
            case RoomSize.Large:
                difficulty = rand < 50 ? RoomDifficulty.Medium : RoomDifficulty.Hard;
                difficulty = Random.Range(0, 100)<20?RoomDifficulty.Hell:difficulty;
                break;
            default:
                break;
        }

        roomDifficulty = difficulty;
    }

    public void ChangeConfiner()
    {
        confiner.m_BoundingVolume = colliderConfiner;
    }
    
    
    public void SpawnEnemies()
    {
        actualWave++;

        List<Vector2> numberOfEnemiesRange = new List<Vector2>();
        List<Vector2> numberOfEliteEnemiesRange = new List<Vector2>();
        bool canEliteSpawn = roomDifficulty != RoomDifficulty.Easy;

        switch (roomDifficulty)
        {
            case RoomDifficulty.Easy:
                numberOfEnemiesRange = waveSettings.numberOfEnemiesEasyPerWave;
                break;
            case RoomDifficulty.Medium:
                numberOfEnemiesRange = waveSettings.numberOfEnemiesMediumPerWave;
                numberOfEliteEnemiesRange = waveSettings.numberOfEliteEnemiesMediumPerWave;
                break;
            case RoomDifficulty.Hard or RoomDifficulty.Hell:
                numberOfEnemiesRange = waveSettings.numberOfEnemiesHardPerWave;
                numberOfEliteEnemiesRange = waveSettings.numberOfEliteEnemiesHardPerWave;
                break;
            default:
                break;
        }

        numberOfWave = numberOfEnemiesRange.Count;
        
        numberOfEnemiesInWave = (int)Random.Range(numberOfEnemiesRange[actualWave-1].x, numberOfEnemiesRange[actualWave-1].y);
        
        numberOfEliteEnemiesInWave = canEliteSpawn?(int)Random.Range(numberOfEliteEnemiesRange[actualWave - 1].x, numberOfEliteEnemiesRange[actualWave - 1].y):0;

        totalEnemiesInWave = numberOfEnemiesInWave + numberOfEliteEnemiesInWave;
        
        var enemiesPool = SC_Pooling.instance.poolList.Find(s => s.poolName == "Ennemis");
        
        if (canEliteSpawn)
        {
            var eliteEnemyList = enemiesPool.subPoolsList.ToList();
            eliteEnemyList = eliteEnemyList.Where(e => e.subPoolTransform.gameObject.name == "GO_Bully" || e.subPoolTransform.gameObject.name == "GO_Summoner").ToList();
            for (int i = 0; i < numberOfEliteEnemiesInWave; i++)
            {
                var eliteEnemy = SC_Pooling.instance.GetItemFromPool("Ennemis", eliteEnemyList[Random.Range(0, eliteEnemyList.Count)].subPoolTransform.gameObject.name);
                if(enemiesInRoom.Contains(eliteEnemy))
                    Debug.Log("ERRORRRRR - ELITE");
                enemiesInRoom.Add(eliteEnemy);
            }
        }
        
        var baseEnemyList = enemiesPool.subPoolsList.ToList();
        baseEnemyList = baseEnemyList.Where(e => e.subPoolTransform.gameObject.name != "GO_Bully" && e.subPoolTransform.gameObject.name != "GO_Summoner").ToList();
        for (int i = 0; i < numberOfEnemiesInWave; i++)
        {
            var enemy = SC_Pooling.instance.GetItemFromPool("Ennemis", baseEnemyList[Random.Range(0, baseEnemyList.Count)].subPoolTransform.gameObject.name);
            
            if(enemiesInRoom.Contains(enemy))
                Debug.Log("ERRORRRRR - BASE");
            enemiesInRoom.Add(enemy);
        }

        Debug.Log(numberOfEnemiesInWave == enemiesInRoom.Distinct().Count());
        
        foreach (var enemy in enemiesInRoom)
        {
            Bounds spawnBounds = new Bounds();
            Collider spawnArea = new Collider();
            
            if (enemy.name is "GO_Bully" or "GO_Summoner")
            {
                spawnArea = eliteSpawnArea[Random.Range(0, eliteSpawnArea.Count)].GetComponent<Collider>();
                spawnBounds = spawnArea.bounds;
                enemy.transform.position = new Vector3(spawnBounds.center.x + Random.Range(-spawnBounds.extents.x,spawnBounds.extents.x), enemy.transform.position.y, spawnBounds.center.z + Random.Range(-spawnBounds.extents.z, spawnBounds.extents.z));
            }
            else
            {
                spawnArea = globalSpawnArea[Random.Range(0, globalSpawnArea.Count)].GetComponent<Collider>();
                spawnBounds = spawnArea.bounds;
                enemy.transform.position = new Vector3(spawnBounds.center.x + Random.Range(-spawnBounds.extents.x,spawnBounds.extents.x), enemy.transform.position.y, spawnBounds.center.z + Random.Range(-spawnBounds.extents.z, spawnBounds.extents.z));
            }
            
            enemy.SetActive(true);
            enemy.GetComponent<AI_StateMachine>().TransitionToState(AI_StateMachine.EnemyState.Idle);
        }
        
        
        Debug.Log($"Spawning wave {actualWave} !");
    }

    public void LockDoors()
    {
        if (activeDoors.Count==0)
        {
            if (doorNorth.doorToConnect!=null)
            {
                activeDoors.Add(doorNorth);
            }
            if (doorSouth.doorToConnect!=null)
            {
                activeDoors.Add(doorSouth);
            }
            if (doorWest.doorToConnect!=null)
            {
                activeDoors.Add(doorWest);
            }
            if (doorEast.doorToConnect!=null)
            {
                activeDoors.Add(doorEast);
            }
        }
        
        foreach (var door in activeDoors)
        {
            door.doorCollider.isTrigger = false;
        }
    }

    public void UnlockDoors()
    {
        foreach (var door in activeDoors)
        {
            if(door.doorCollider != null)
                door.doorCollider.isTrigger = true;
        }
    }

    public void DecreaseEnemiesCount(SC_AIStats enemy)
    {
        if (!enemiesInRoom.Contains(enemy.gameObject))
            return;
        
        totalEnemiesInWave--;

        if (totalEnemiesInWave>0) 
            return;

        if (actualWave < numberOfWave)
        {
            enemiesInRoom.Clear();
            SpawnEnemies();
            return;
        }
        
        isClear = true;
        SC_AIStats.onDeath -= DecreaseEnemiesCount;
        UnlockDoors();
    }
    
}
