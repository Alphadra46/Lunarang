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
    public Animator animator;

    [HideInInspector] public SC_RoomManager roomManager;
    [HideInInspector] public SC_TutoRoomManager tutoRoomManager;

    public void Initialize(SC_RoomManager roomManager)
    {
        doorRenderer = GetComponent<MeshRenderer>();
        doorCollider = GetComponent<Collider>();
        this.roomManager = roomManager;
    }
    
    public void Initialize(SC_TutoRoomManager tutoRoomManager)
    {
        doorRenderer = GetComponent<MeshRenderer>();
        doorCollider = GetComponent<Collider>();
        this.tutoRoomManager = tutoRoomManager;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        
        OnExitRoom();
    }

    public void EnableDoor()
    {
        animator.SetBool("isOpen",true);
        //doorRenderer.enabled = true;
        doorCollider.isTrigger = true;
    }
    
    public void DisableDoor()
    {
        animator.SetBool("isOpen",false);
        //doorRenderer.enabled = false;
        doorCollider.isTrigger = false;
    }

    public void OnEnterRoom()
    {
        SC_PlayerController.instance.Teleport(new Vector3(doorSpawnPoint.transform.position.x, 0, doorSpawnPoint.transform.position.z));

        if (roomManager != null)
        {
            roomManager.ChangeConfiner();
            if (!roomManager.isSpecialRoom && !roomManager.isClear)
            {
                roomManager.SpawnEnemies();
                roomManager.LockDoors();
                SC_AIStats.onDeath += roomManager.DecreaseEnemiesCount;
            }
        
            SC_MiniMapComponent.changeRoomTransform?.Invoke(roomManager.MinimapPosition);
        }
        else if (tutoRoomManager != null)
        {
            if (!tutoRoomManager.isSpecialRoom && !tutoRoomManager.isClear)
            {
                tutoRoomManager.SpawnEnemies();
                tutoRoomManager.LockDoors();
                SC_AIStats.onDeath += tutoRoomManager.DecreaseEnemyCount;
            }
        }
    }

    public void OnExitRoom()
    {
        doorToConnect.OnEnterRoom();
    }
    
}
