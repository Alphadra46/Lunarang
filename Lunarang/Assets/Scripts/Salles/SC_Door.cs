using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Door : MonoBehaviour
{
    public MeshRenderer doorRenderer;
    public Collider doorCollider;
    public SC_Door doorToConnect;
    public GameObject doorSpawnPoint;

    [HideInInspector] public SC_RoomManager roomManager;

    public void Initialize(SC_RoomManager roomManager)
    {
        doorRenderer = GetComponent<MeshRenderer>();
        doorCollider = GetComponent<Collider>();
        this.roomManager = roomManager;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        
        OnExitRoom();
    }

    public void EnableDoor()
    {
        doorRenderer.enabled = true;
        doorCollider.isTrigger = true;
    }
    
    public void DisableDoor()
    {
        doorRenderer.enabled = false;
        doorCollider.isTrigger = false;
    }

    public void OnEnterRoom()
    {
        SC_PlayerController.instance.Teleport(new Vector3(doorSpawnPoint.transform.position.x, 0, doorSpawnPoint.transform.position.z));

        roomManager.ChangeConfiner();
        if (!roomManager.isSpecialRoom && !roomManager.isClear)
        {
            roomManager.SpawnEnemies();
            roomManager.LockDoors();
            SC_AIStats.onDeath += roomManager.DecreaseEnemiesCount;
        }
    }

    public void OnExitRoom()
    {
        doorToConnect.OnEnterRoom();
    }
    
}
