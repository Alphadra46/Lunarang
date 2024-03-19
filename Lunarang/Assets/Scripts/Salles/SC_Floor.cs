using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class SC_Floor
{
    public List<GameObject> spawnToBossRooms;
    public List<GameObject> spawnToChestRooms;
    public List<GameObject> bossToChestRooms;
    public ABPath spawnToBossPath;
    public ABPath spawnToChestPath;
    public ABPath bossToChestPath;

    public GameObject floorStartRoom;
    public GameObject floorEndRoom;
    public GameObject floorChestRoom;
    
    public SC_Floor(List<GameObject> spawnToBossRooms, List<GameObject> spawnToChestRooms, List<GameObject> bossToChestRooms, ABPath spawnToBossPath, ABPath spawnToChestPath, ABPath bossToChestPath, GameObject floorStartRoom, GameObject floorEndRoom, GameObject floorChestRoom)
    {
        this.spawnToBossRooms = spawnToBossRooms;
        this.spawnToChestRooms = spawnToChestRooms;
        this.bossToChestRooms = bossToChestRooms;
        this.spawnToBossPath = spawnToBossPath;
        this.spawnToChestPath = spawnToChestPath;
        this.bossToChestPath = bossToChestPath;
        this.floorStartRoom = floorStartRoom;
        this.floorEndRoom = floorEndRoom;
        this.floorChestRoom = floorChestRoom;
    }
    
    public void HideFloor()
    {
        foreach (var room in spawnToBossRooms)
        {
            room.SetActive(false);
        }
        foreach (var room in spawnToChestRooms)
        {
            room.SetActive(false);
        }
        foreach (var room in bossToChestRooms)
        {
            room.SetActive(false);
        }
        
        floorStartRoom.SetActive(false);
        floorEndRoom.SetActive(false);
        floorChestRoom.SetActive(false);
    }

    public void DisplayFloor()
    {
        foreach (var room in spawnToBossRooms)
        {
            room.SetActive(true);
        }
        foreach (var room in spawnToChestRooms)
        {
            room.SetActive(true);
        }
        foreach (var room in bossToChestRooms)
        {
            room.SetActive(true);
        }
        
        floorStartRoom.SetActive(true);
        floorEndRoom.SetActive(true);
        floorChestRoom.SetActive(true);
    }
    
}
