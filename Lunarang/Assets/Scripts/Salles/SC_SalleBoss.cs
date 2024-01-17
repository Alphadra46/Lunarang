using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_SalleBoss : SC_FonctionnementSalle, IObserver
{
    [Header("The door(s) that will notify the room that the player has entered (from which he can enter this room)")]
    [SerializeField] private List<SC_Subject> subjects = new List<SC_Subject>();
    [Header("The door(s) of the room")]
    [SerializeField] private List<GameObject> roomDoors = new List<GameObject>();
    [Header("The total number of enemies regardless of their type")]
    [SerializeField] private int numberOfEnemies = default; //TODO - Change this to be linked to an enemy type rather than just a total number of enemies
    [Header("The possibles types of enemies which can spawn in this room")]
    [SerializeField] private List<GameObject> possibleTypesOfEnemiesInRoom;
    
    [SerializeField] private GameObject spawnArea;
    
    //Save each enemy that have spawned in the room
    [HideInInspector] public List<SC_AIStats> listOfEnemiesInRoom = new List<SC_AIStats>();
    private Bounds spawnAreaBounds;
    
    // Start is called before the first frame update
    void Start()
    {
        AddSelfToSubjectList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Spawn a certain amount of enemies at a random location in the room
    /// </summary>
    protected override void SetupRoom()
    {
        spawnAreaBounds.center = spawnArea.transform.position;
        spawnAreaBounds.size = spawnArea.transform.localScale;
        
        for (int i = 0; i < numberOfEnemies; i++) //TODO - Use pooling instead of instantiate
        {
            var enemy = Instantiate(possibleTypesOfEnemiesInRoom[Random.Range(0, possibleTypesOfEnemiesInRoom.Count)],
                new Vector3(Random.Range(-spawnAreaBounds.extents.x ,spawnAreaBounds.extents.x)+ spawnArea.transform.position.x, 
                    0,
                    Random.Range(-spawnAreaBounds.extents.z,spawnAreaBounds.extents.z)+ spawnArea.transform.position.z),
                Quaternion.identity).GetComponent<SC_AIStats>();
            
            listOfEnemiesInRoom.Add(enemy);
        }

        foreach (var enemy in listOfEnemiesInRoom)
        {
            enemy.TryGetComponent(out SC_Subject subject);
            subject.AddObserver(this);
        }
        
        foreach (var door in roomDoors)
        {
            door.TryGetComponent(out Collider collider);
            collider.enabled = false;
        }
        
        Debug.Log("Room setup");
    }

    //Not used in this script
    public void OnNotify()
    {
        throw new System.NotImplementedException();
    }

    public void OnNotify(string context, SC_Subject subjectReference)
    {
        if (IsRoomCleared)
            return;

        switch (context)
        {
            case "enter":
                SetupRoom();
                break;
            case "enemyDeath":
                if (subjectReference.TryGetComponent(out SC_AIStats enemy))
                    listOfEnemiesInRoom.Remove(enemy);
                
                if (listOfEnemiesInRoom.Count <= 0)
                {
                    IsRoomCleared = true;
                    foreach (var door in roomDoors)
                    {
                        door.TryGetComponent(out Collider collider);
                        collider.enabled = true;
                    }
                    Debug.Log("Room Cleared");
                    //TODO - Do magic stuff after the room is cleared
                    
                    
                    SC_GameManager.instance.ChangeState(GameState.WIN);
                    SC_PlayerController.instance.FreezeMovement(true);
                    SC_PlayerController.instance.FreezeDash(true);
                }
                break;
            default:
                break;
        }
    }
    //Not used in this script
    public void OnNotify(float newCurrentHP, float newMaxHP)
    {
        throw new System.NotImplementedException();
    }
    
    private void AddSelfToSubjectList()
    {
        foreach (var subject in subjects)
        {
            subject.AddObserver(this);
        }
    }
}
