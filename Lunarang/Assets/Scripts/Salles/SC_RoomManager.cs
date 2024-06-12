using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

[Serializable]
public class CustomTable
{
    [VerticalGroup(("Small")), LabelWidth(22)]
    public int Easy_S, Medium_S, Hard_S;
    
    [VerticalGroup(("Medium")), LabelWidth(22)]
    public int Easy_M, Medium_M, Hard_M;
    
    [VerticalGroup(("Large")), LabelWidth(22)]
    public int Easy_L, Medium_L, Hard_L;
}

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
    
    [Header("Minimap Position")]
    [TabGroup("Settings", "Global Settings")] public GameObject MinimapPosition;

    [Header("Enemies spawn area")] 
    [TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] public List<GameObject> globalSpawnArea;
    [TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] public List<GameObject> eliteSpawnArea;

    [Header("Enemies wave parameters")]
    [SerializeField, TabGroup("Settings", "Wave Settings"), HideIf("isSpecialRoom")] private SC_WaveSettings waveSettings;
    
    [Header("Other room parameters")] 
    [SerializeField, TabGroup("Settings", "Global Settings")] private Collider colliderConfiner;
    [SerializeField, TabGroup("Settings", "Global Settings")] private GameObject resourceChest;
    [SerializeField, TabGroup("Settings", "Global Settings")] private GameObject skillChest;
    [SerializeField, TabGroup("Settings", "Global Settings")] private Interactor roomInteractor;
    
    private CinemachineConfiner confiner;

    private VisualEffect roomClearVFX;
    private List<SC_Door> activeDoors = new List<SC_Door>();
    [ShowInInspector] private List<GameObject> enemiesInRoom = new List<GameObject>();

    [ShowInInspector] private int numberOfEnemiesInWave;
    [ShowInInspector] private int numberOfEliteEnemiesInWave;
    [ShowInInspector] private int totalEnemiesInWave;
    [ShowInInspector] public bool isClear=false;

    private int actualWave = 0;
    private int numberOfWave = 0;
    private bool isInit = false;
    private bool hasBonusChallenge = false;
    private bool isBonusChallengeActive = false;

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
        Hard
    }
    
    private void OnEnable()
    {
        if (isSpecialRoom)
        {
            doorNorth.animator.SetBool("isOpen", true);
        }
        
        if (isClear)
        {
            roomClearVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_02").GetComponent<VisualEffect>();
            roomClearVFX.transform.position = transform.position;
            float size = roomSize == RoomSize.Large ? 30 : roomSize == RoomSize.Medium ? 22 : 10;
            roomClearVFX.SetVector2("Dimensions",new Vector2(size,size));
            roomClearVFX.gameObject.SetActive(true);
            roomClearVFX.Play();
        }
        
        if (isInit)
            return;

        isInit = true;

        
        if (!isSpecialRoom)
            SetDifficulty();

        if (skillChest != null && resourceChest != null)
        {
            print("Start Chest Checkup");
            skillChest.SetActive(false);
            resourceChest.SetActive(false);
            SkillChestSpawn();
            ResourceChestSpawn();
        }
        
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

    private void OnDisable()
    {
        if (isClear)
        {
            roomClearVFX.Stop();
            roomClearVFX.gameObject.SetActive(false);
            SC_Pooling.instance.ReturnItemToPool("VFX",roomClearVFX.gameObject);
            roomClearVFX = null;
        }
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
                hasBonusChallenge = Random.Range(0, 100) < 20;
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
            case RoomDifficulty.Hard:
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
            enemy.GetComponent<AI_StateMachine>().CanAttack(true);
        }
        
        
        Debug.Log($"Spawning wave {actualWave} !");
        SC_FeedbackRoomStatusUI.roomNewWave?.Invoke(actualWave);
    }

    public void HellChallenge()
    {
        isClear = false;
        SC_AIStats.onDeath += DecreaseEnemiesCount;
        LockDoors();

        numberOfEliteEnemiesInWave = waveSettings.maxEliteAlive;
        numberOfEnemiesInWave = waveSettings.numberOfEnemiesAlive - numberOfEliteEnemiesInWave;
        
        isBonusChallengeActive = true;
        totalEnemiesInWave = waveSettings.totalOfEnemiesHell;
        
        var enemiesPool = SC_Pooling.instance.poolList.Find(s => s.poolName == "Ennemis");
        
        for (int i = 0; i < waveSettings.numberOfEnemiesAlive; i++) //Spawn the first batch of enemies
        {
            if (i<waveSettings.maxEliteAlive) //TODO - Maybe change the condition to be a const or another variable than maxEliteAlive
            {
                var eliteEnemyList = enemiesPool.subPoolsList.ToList();
                eliteEnemyList = eliteEnemyList.Where(e => e.subPoolTransform.gameObject.name == "GO_Bully" || e.subPoolTransform.gameObject.name == "GO_Summoner").ToList();
                for (int j = 0; j < numberOfEliteEnemiesInWave; j++)
                {
                    var eliteEnemy = SC_Pooling.instance.GetItemFromPool("Ennemis", eliteEnemyList[Random.Range(0, eliteEnemyList.Count)].subPoolTransform.gameObject.name);
                    
                    if(enemiesInRoom.Contains(eliteEnemy))
                        Debug.Log("ERRORRRRR - ELITE");
                    enemiesInRoom.Add(eliteEnemy);
                }
            }
            else
            {
                var baseEnemyList = enemiesPool.subPoolsList.ToList();
                baseEnemyList = baseEnemyList.Where(e => e.subPoolTransform.gameObject.name != "GO_Bully" && e.subPoolTransform.gameObject.name != "GO_Summoner").ToList();
                for (int j = 0; j < numberOfEnemiesInWave; j++)
                {
                    var enemy = SC_Pooling.instance.GetItemFromPool("Ennemis", baseEnemyList[Random.Range(0, baseEnemyList.Count)].subPoolTransform.gameObject.name);
            
                    if(enemiesInRoom.Contains(enemy))
                        Debug.Log("ERRORRRRR - BASE");
                    enemiesInRoom.Add(enemy);
                }
            }
        }

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
    }

    private void SpawnHellEnemy()
    {
        var enemiesPool = SC_Pooling.instance.poolList.Find(s => s.poolName == "Ennemis");
        GameObject enemy;
        
        if (numberOfEnemiesInWave<waveSettings.maxEliteAlive && Random.Range(0,100)<20)
        {
            numberOfEliteEnemiesInWave++;
            var eliteEnemyList = enemiesPool.subPoolsList.ToList();
            eliteEnemyList = eliteEnemyList.Where(e => e.subPoolTransform.gameObject.name == "GO_Bully" || e.subPoolTransform.gameObject.name == "GO_Summoner").ToList();
            var eliteEnemy = SC_Pooling.instance.GetItemFromPool("Ennemis", eliteEnemyList[Random.Range(0, eliteEnemyList.Count)].subPoolTransform.gameObject.name);
                    
            if(enemiesInRoom.Contains(eliteEnemy))
                Debug.Log("ERRORRRRR - ELITE");
            enemiesInRoom.Add(eliteEnemy);
            enemy = eliteEnemy;
        }
        else
        {
            numberOfEnemiesInWave++;
            var baseEnemyList = enemiesPool.subPoolsList.ToList();
            baseEnemyList = baseEnemyList.Where(e => e.subPoolTransform.gameObject.name != "GO_Bully" && e.subPoolTransform.gameObject.name != "GO_Summoner").ToList();
            var baseEnemy = SC_Pooling.instance.GetItemFromPool("Ennemis", baseEnemyList[Random.Range(0, baseEnemyList.Count)].subPoolTransform.gameObject.name);
            
            if(enemiesInRoom.Contains(baseEnemy))
                Debug.Log("ERRORRRRR - BASE");
            enemiesInRoom.Add(baseEnemy);
            enemy = baseEnemy;
        }
        
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
        
        enemy.GetComponent<NavMeshAgent>().enabled = true;
        enemy.SetActive(true);
        enemy.GetComponent<AI_StateMachine>().TransitionToState(AI_StateMachine.EnemyState.Idle);
    }
    
    public void LockDoors()
    {
        SC_GameManager.clearRoom += ClearRoom;
        
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
            door.animator.SetBool("isOpen", false);
        }
    }

    public void UnlockDoors()
    {
        SC_GameManager.clearRoom -= ClearRoom;
        SC_FeedbackRoomStatusUI.roomCleared?.Invoke();
        
        foreach (var door in activeDoors)
        {
            if (door.doorCollider != null)
            {
                door.doorCollider.isTrigger = true;
                door.animator.SetBool("isOpen", true);
            }
        }
    }

    public void DecreaseEnemiesCount(SC_AIStats enemy)
    {
        if (!enemiesInRoom.Contains(enemy.gameObject))
            return;

        totalEnemiesInWave--;
        enemiesInRoom.Remove(enemy.gameObject);

        if (isBonusChallengeActive)
        {
            if (totalEnemiesInWave > waveSettings.numberOfEnemiesAlive)
            {
                enemiesInRoom.Remove(enemy.gameObject);
                
                if (enemy.isElite)
                    numberOfEliteEnemiesInWave--;
                else
                    numberOfEnemiesInWave--;
                
                SpawnHellEnemy();
            }
        }
        
        if (totalEnemiesInWave>0) 
            return;

        if (isBonusChallengeActive)
        {
            isBonusChallengeActive = false;
            //TODO - Spawn a treasure chest
        }
        else if (actualWave < numberOfWave)
        {
            enemiesInRoom.Clear();
            SpawnEnemies();
            return;
        }
        
        enemiesInRoom.Clear();
        isClear = true;
        SC_AIStats.onDeath -= DecreaseEnemiesCount;
        UnlockDoors();
        var clearRoomVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_01").GetComponent<VisualEffect>();
        clearRoomVFX.gameObject.SetActive(true);
        clearRoomVFX.transform.position = transform.position;
        clearRoomVFX.Play();
        roomClearVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_02").GetComponent<VisualEffect>();
        roomClearVFX.transform.position = transform.position;
        roomClearVFX.gameObject.SetActive(true);
        float size = roomSize == RoomSize.Large ? 30 : roomSize == RoomSize.Medium ? 22 : 10;
        roomClearVFX.SetVector2("Dimensions",new Vector2(size,size));
        roomClearVFX.Play();
        StartCoroutine(EndClearVFX(clearRoomVFX.GetFloat("Duration"), clearRoomVFX));
        StartCoroutine(PurifyRoom(3, roomInteractor));

        if (skillChest != null && resourceChest != null)
        {
            RevealChest(skillChest);
            RevealChest(resourceChest);
        }
        

        if (hasBonusChallenge)
        {
            //TODO - Display the lever to activate the bonus challenge
        }
        
    }

    private IEnumerator PurifyRoom(float duration, Interactor interactor)
    {
        float timer = duration;
        float radiusSize = roomSize == RoomSize.Large ? 18 : roomSize == RoomSize.Medium ? 13 : 8;
        while (timer>0)
        {
            interactor.radius = Mathf.Lerp(0, radiusSize, 1 - (timer / duration));
            timer -= Time.deltaTime;
            yield return null;
        }
        interactor.radius = radiusSize;
    }
    
    private IEnumerator EndClearVFX(float duration, VisualEffect vfx)
    {
        yield return new WaitForSeconds(duration);
        SC_Pooling.instance.ReturnItemToPool("VFX",vfx.gameObject);
        vfx.gameObject.SetActive(false);
    }

    public void SkillChestSpawn()
    {
        var chances = SC_RewardManager.instance.skillChestChances[0];

        int spawnChance;
        
        switch (roomSize,roomDifficulty)
        {
            case (RoomSize.Small,RoomDifficulty.Easy):
                spawnChance = chances.Easy_S;
                break;
            case (RoomSize.Medium,RoomDifficulty.Medium):
                spawnChance = chances.Medium_M;
                break;
            case (RoomSize.Medium,RoomDifficulty.Hard):
                spawnChance = chances.Hard_M;
                break;
            case (RoomSize.Large,RoomDifficulty.Medium):
                spawnChance = chances.Medium_L;
                break;
            case (RoomSize.Large,RoomDifficulty.Hard):
                spawnChance = chances.Hard_L;
                break;
            default:
                spawnChance = 0;
                break;
        }

        if (Random.Range(1,101) <= spawnChance)
        {
            print("Skill chest spawn !");
            skillChest.SetActive(true);
        }
        
    }

    private void RevealChest(GameObject chest)
    {
        if (!chest.activeSelf)
            return;
        
        var chestInteractor = chest.GetComponentInChildren<Interactor>();
        var ps = chest.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in ps)
        {
            particle.Play();
        }
        StartCoroutine(LerpSwap(chestInteractor,3));
    }
    
    public void ResourceChestSpawn()
    {
        var chances = SC_RewardManager.instance.resourceChestChances[0];

        int spawnChance;
        
        switch (roomSize,roomDifficulty)
        {
            case (RoomSize.Small,RoomDifficulty.Easy):
                spawnChance = chances.Easy_S;
                break;
            case (RoomSize.Medium,RoomDifficulty.Medium):
                spawnChance = chances.Medium_M;
                break;
            case (RoomSize.Medium,RoomDifficulty.Hard):
                spawnChance = chances.Hard_M;
                break;
            case (RoomSize.Large,RoomDifficulty.Medium):
                spawnChance = chances.Medium_L;
                break;
            case (RoomSize.Large,RoomDifficulty.Hard):
                spawnChance = chances.Hard_L;
                break;
            default:
                spawnChance = 0;
                break;
        }
        
        if (Random.Range(1,101) <= spawnChance)
        {
            print("Resource chest spawn !");
            resourceChest.SetActive(true);
        }
        
    }
    
    public void ClearRoom()
    {

        foreach (var enemy in enemiesInRoom)
        {
            enemy.GetComponent<SC_AIStats>().ResetStats();
            enemy.SetActive(false);
            SC_Pooling.instance.ReturnItemToPool("Ennemis", enemy);
        }
        
        enemiesInRoom.Clear();
        isClear = true;
        SC_AIStats.onDeath -= DecreaseEnemiesCount;
        UnlockDoors();
        var clearRoomVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_01").GetComponent<VisualEffect>();
        clearRoomVFX.transform.position = transform.position;
        clearRoomVFX.Play();
        roomClearVFX = SC_Pooling.instance.GetItemFromPool("VFX", "VFX_Purification_02").GetComponent<VisualEffect>();
        roomClearVFX.transform.position = transform.position;
        roomClearVFX.gameObject.SetActive(true);
        float size = roomSize == RoomSize.Large ? 30 : roomSize == RoomSize.Medium ? 22 : 10;
        roomClearVFX.SetVector2("Dimensions",new Vector2(size,size));
        roomClearVFX.Play();
        StartCoroutine(EndClearVFX(clearRoomVFX.GetFloat("Duration"), clearRoomVFX));
        StartCoroutine(PurifyRoom(3, roomInteractor));
        clearRoomVFX.gameObject.SetActive(true);
        
        if (skillChest != null && resourceChest != null)
        {
            RevealChest(skillChest);
            RevealChest(resourceChest);
        }
    }

    
    IEnumerator LerpSwap(Interactor interactor, float duration)
    {
        float timer = duration;

        while (timer>0)
        {
            timer -= Time.deltaTime;
            interactor.radius = Mathf.Lerp(0, 3, 1 - (timer / duration));
            yield return null;
        }
    }
    
}
