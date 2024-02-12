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
    
    [Header("Door possible position")]
    [TabGroup("Settings", "Global Settings")] public SC_Door doorNorth;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorSouth;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorWest;
    [TabGroup("Settings", "Global Settings")] public SC_Door doorEast;

    [Header("Enemies spawn area")] 
    [TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] public List<GameObject> globalSpawnArea;
    [TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] public List<GameObject> eliteSpawnArea;

    [Header("Enemies wave parameters")] 
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int minNumberOfEnemiesEasy;
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int maxNumberOfEnemiesEasy;
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int minNumberOfEnemiesMedium;
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int maxNumberOfEnemiesMedium;
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int minNumberOfEnemiesHard;
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int maxNumberOfEnemiesHard;
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int minNumberOfEnemiesHell;
    [SerializeField, Range(0, 10), TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private int maxNumberOfEnemiesHell;

    [Header("Other room parameters")] 
    [SerializeField, TabGroup("Settings", "Global Settings")] private Collider colliderConfiner;
    private CinemachineConfiner confiner;

    private List<SC_Door> activeDoors = new List<SC_Door>();
    private List<GameObject> enemiesInRoom = new List<GameObject>();

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
                difficulty = rand < 40 ? RoomDifficulty.Medium : rand < 80 ? RoomDifficulty.Hard : RoomDifficulty.Hell;
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
        Vector2 numberOfEnemiesRange = Vector2.zero;
        bool canEliteSpawn = roomDifficulty != RoomDifficulty.Easy;
        
        switch (roomDifficulty)
        {
            case RoomDifficulty.Easy:
                numberOfEnemiesRange = new Vector2(minNumberOfEnemiesEasy, maxNumberOfEnemiesEasy);
                break;
            case RoomDifficulty.Medium:
                numberOfEnemiesRange = new Vector2(minNumberOfEnemiesMedium, minNumberOfEnemiesMedium);
                break;
            case RoomDifficulty.Hard:
                numberOfEnemiesRange = new Vector2(minNumberOfEnemiesHard, maxNumberOfEnemiesHard);
                break;
            case RoomDifficulty.Hell:
                numberOfEnemiesRange = new Vector2(minNumberOfEnemiesHell, minNumberOfEnemiesHell);
                break;
            default:
                break;
        }

        int numberOfEnemies = Random.Range((int)numberOfEnemiesRange.x, (int)numberOfEnemiesRange.y);

        var enemiesPool = SC_Pooling.instance.poolList.Find(s => s.poolName == "Enemies");
        
        if (!canEliteSpawn)
        {
            enemiesPool.subPoolsList.Remove(enemiesPool.subPoolsList.Find(s => s.subPoolTransform.gameObject.name == "GO_Summoner"));
            enemiesPool.subPoolsList.Remove(enemiesPool.subPoolsList.Find(s => s.subPoolTransform.gameObject.name == "GO_Bully"));
        }
        
        for (int i = 0; i < numberOfEnemies; i++)
        {
            enemiesInRoom.Add(SC_Pooling.instance.GetItemFromPool("Enemies",enemiesPool.subPoolsList[Random.Range(0,enemiesPool.subPoolsList.Count)].subPoolTransform.gameObject.name));
        }
        
        Debug.Log("Spawning first wave !");
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
            door.doorCollider.enabled = false;
        }
    }

    public void UnlockDoors()
    {
        foreach (var door in activeDoors)
        {
            door.doorCollider.enabled = true;
        }
    }
}
