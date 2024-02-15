using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_PathCreator : MonoBehaviour
{
    [Header("General setting")]
    [TabGroup("Settings", "Generation values")] public bool setSeed;
    [TabGroup("Settings", "Generation values"), ShowIf("setSeed")] public int customSeed;
    [TabGroup("Settings", "Generation values"), ShowInInspector, ReadOnly] private int seed;

    [Header("Normal room prefabs lists")]
    [TabGroup("Settings", "Prefabs")] public List<GameObject> smallRoomList = new List<GameObject>();
    [TabGroup("Settings", "Prefabs")] public List<GameObject> mediumRoomList = new List<GameObject>();
    [TabGroup("Settings", "Prefabs")] public List<GameObject> bigRoomList = new List<GameObject>();

    [Header("Chest room prefabs lists")] 
    [TabGroup("Settings", "Prefabs")] public List<GameObject> chestRoomList = new List<GameObject>();
    [Header("Boss room prefabs lists")]
    [TabGroup("Settings", "Prefabs")] public List<GameObject> bossRoomList = new List<GameObject>();
    [Header("Spawn room prefabs lists")]
    [TabGroup("Settings", "Prefabs")] public List<GameObject> spawnRoomList = new List<GameObject>();
    [Header("Stair room prefabs lists")]
    [TabGroup("Settings", "Prefabs")] public List<GameObject> stairRoomList = new List<GameObject>(); //TODO - When the floor system will be done use this list for "spawn" rooms and "boss" rooms

    [Space(10), Header("Challenge room parameters")] 
    [TabGroup("Settings", "Generation values")] public int smallRoomSpawnRate = 20;
    [TabGroup("Settings", "Generation values")] public int mediumRoomSpawnRate = 50;
    [TabGroup("Settings", "Generation values")] public int bigRoomSpawnRate = 30;
    
    [Space(10), Header("Spawn room parameters")]
    [TabGroup("Settings", "Generation values")] public bool useRandomOnSpawnLocation;
    [Range(0,99)]
    [TabGroup("Settings", "Generation values"), HideIf("useRandomOnSpawnLocation")] public int spawnNodeIndex;
    
    [Space(10), Header("Boss room parameters")]
    [Range(0,10)]
    [TabGroup("Settings", "Generation values")] public int bossRoomSpawnMinRange;
    [Range(0,10)]
    [TabGroup("Settings", "Generation values")] public int bossRoomSpawnMaxRange;

    [Space(10), Header("Chest room parameters")]
    [Range(0,10)]
    [TabGroup("Settings", "Generation values")] public int chestRoomSpawnMinRangeFromSpawn;
    [Range(0,10)]
    [TabGroup("Settings", "Generation values")] public int chestRoomSpawnMaxRangeFromSpawn;
    [Range(0,10)]
    [TabGroup("Settings", "Generation values")] public int chestRoomSpawnMinRangeFromBoss;
    [Range(0,10)]
    [TabGroup("Settings", "Generation values")] public int chestRoomSpawnMaxRangeFromBoss;
    
    private ABPath spawnToBossPath;
    private List<GameObject> spawnToBossRooms = new List<GameObject>();
    private ABPath spawnToChestPath;
    private List<GameObject> spawnToChestRooms = new List<GameObject>();
    private ABPath bossToChestPath;
    private List<GameObject> bossToChestRooms = new List<GameObject>();


    private int originalSpawnNodeIndex;
    
    private bool isInit = false;
    
    private List<GridNode> nodesInRange = new List<GridNode>();
    
    private int bossRoomIndex;
    private int chestRoomIndex;

    private GameObject spawn;
    private GameObject boss;
    private GameObject chest;
    
    // Start is called before the first frame update
    void Start()
    {
        originalSpawnNodeIndex = spawnNodeIndex;
        SetupGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInit)
            return;
        

        if (Input.GetKeyDown(KeyCode.R))
        {
            SetupGeneration();
        }
    }

    public GameObject GetChallengeRoom()
    {
        var r = Random.Range(0, 100);

        return r < smallRoomSpawnRate
            ? smallRoomList[Random.Range(0, smallRoomList.Count)]
            :
            (r >= smallRoomSpawnRate && r < smallRoomSpawnRate + mediumRoomSpawnRate)
                ?
                mediumRoomList[Random.Range(0, mediumRoomList.Count)]
                : bigRoomList[Random.Range(0, bigRoomList.Count)];
    }
    
    
    /// <summary>
    /// Setup all the room placements and create the path between the rooms
    /// </summary>
    private void SetupGeneration()
    {
        //Set a seed for the Random class
        seed = setSeed?customSeed:Random.Range(0, 1000000);
        Random.InitState(seed);
        
        //Reset the nodes in range so that there is no conflict with previous paths
        nodesInRange.Clear();
        foreach (var room in spawnToBossRooms)
        {
            Destroy(room);
        }
        spawnToBossRooms.Clear();
        foreach (var room in spawnToChestRooms)
        {
            Destroy(room);
        }
        spawnToChestRooms.Clear();
        foreach (var room in bossToChestRooms)
        {
            Destroy(room);
        }
        bossToChestRooms.Clear();
        Destroy(spawn);
        Destroy(boss);
        Destroy(chest);
        
        
        //Set a random spawn location if wanted
        spawnNodeIndex = useRandomOnSpawnLocation
            ? Random.Range(0, AstarPath.active.data.gridGraph.nodes.Length - 1)
            : originalSpawnNodeIndex;
        
        //---------- Boss room ----------
        //Get all of the room between these two ranges to create the boss room and a path between the spawn and the boss room
        nodesInRange.Clear();
        GetNodesInRange(spawnNodeIndex , bossRoomSpawnMinRange, bossRoomSpawnMaxRange);

        var start = (Vector3)AstarPath.active.data.gridGraph.nodes[spawnNodeIndex].position;
        var end = (Vector3)AstarPath.active.data.gridGraph.nodes[nodesInRange[Random.Range(0,nodesInRange.Count)].NodeInGridIndex].position;
        spawnToBossPath = ABPath.Construct(start, end);
        AstarPath.StartPath(spawnToBossPath);

        while (!spawnToBossPath.IsDone())
        {
            
        } //Used to wait for the path to be done TODO - Maybe use async function or IEnumerator
        
        bossRoomIndex = AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)spawnToBossPath.path[^1]); //Get the index of the node the boss room have been created on

        //---------- Chest room ----------
        nodesInRange.Clear(); //Reset the nodes in range because we don't want it to contains information from the boss room path
        
        //First search all nodes in range from the spawn room
        GetNodesInRange(spawnNodeIndex, chestRoomSpawnMinRangeFromSpawn, chestRoomSpawnMaxRangeFromSpawn);
        var nodesInRangeFromSpawn = nodesInRange.ToList(); //Use a temporary list to get all of the nodes that matches the above criteria
        
        nodesInRange.Clear(); //Reset the nodes in range because we don't want it to contains information from the previous research
        
        //Then search all nodes in range from the boss room
        GetNodesInRange(bossRoomIndex, chestRoomSpawnMinRangeFromBoss, chestRoomSpawnMaxRangeFromBoss);
        var nodesInRangeFromBoss = nodesInRange.ToList();

        //Reset a third time the list so we can use it as the real node in range that contains node that fulfill both previous criteria 
        nodesInRange.Clear();
        
        //Go through every node found to check if they are in both range lists
        foreach (var node in (nodesInRangeFromBoss.Count>nodesInRangeFromSpawn.Count?nodesInRangeFromBoss:nodesInRangeFromSpawn))
        {
            if (!nodesInRangeFromBoss.Contains(node) || !nodesInRangeFromSpawn.Contains(node)) 
                continue;
            
            nodesInRange.Add(node); //If so add it to the "real" nodes in range
        }
        
        //Remove the nodes used for the main path so that the chest room have to spawn somewhere else, preventing the map to look like a straight line.
        foreach (var node in spawnToBossPath.path)
        {
            if (nodesInRange.Contains(node))
                nodesInRange.Remove((GridNode)node);
        }
        
        //First the path from the spawn to the chest room
        start = (Vector3)AstarPath.active.data.gridGraph.nodes[spawnNodeIndex].position;
        end = (Vector3)AstarPath.active.data.gridGraph.nodes[nodesInRange[Random.Range(0, nodesInRange.Count)].NodeInGridIndex].position;
        spawnToChestPath = ABPath.Construct(start, end);
        AstarPath.StartPath(spawnToChestPath);
        while (!spawnToChestPath.IsDone())
        {
            
        } //Used to wait for the path to be done TODO - Maybe use async function or IEnumerator
        
        chestRoomIndex = AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)spawnToChestPath.path[^1]); //Get the index of the node the chest room have been created on
        
        //Then the path from the boss room to the chest one
        start = (Vector3)AstarPath.active.data.gridGraph.nodes[bossRoomIndex].position;
        end = (Vector3)AstarPath.active.data.gridGraph.nodes[chestRoomIndex].position;
        bossToChestPath = ABPath.Construct(start, end);
        AstarPath.StartPath(bossToChestPath);
        while (!bossToChestPath.IsDone())
        {
            
        } //Used to wait for the path to be done TODO - Maybe use async function or IEnumerator
        
        //DisplayPath();
        CreateFloor();
        
        isInit = true;
    }
    
    /// <summary>
    /// Add to the list all of the room in the range of the start node
    /// </summary>
    /// <param name="startNodeIndex">The index in the list of where the spawn room will be created</param>
    /// <param name="searchRangeMin">The minimum range at which the room will be created</param>
    /// <param name="searchRangeMax">The maximum range at which the room will be created</param>
    public void GetNodesInRange(int startNodeIndex, int searchRangeMin, int searchRangeMax)
    {
        for (int j = searchRangeMin; j <= searchRangeMax; j++)
        {
            for (int i = 0; i < j+1; i++)
            {
                var Y = i * 10;
                var X = j - i;

                if ((startNodeIndex + Y + X < (startNodeIndex - startNodeIndex%10)+(i+1)*10) && startNodeIndex + Y + X >= startNodeIndex - startNodeIndex%10 + i*10 && startNodeIndex + Y + X <AstarPath.active.data.gridGraph.nodes.Length) //Initial Point
                {
                    if (!nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y + X]))
                        nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y + X]);
                }

                if ((startNodeIndex + Y - X < (startNodeIndex - startNodeIndex%10)+(i+1)*10) && startNodeIndex + Y - X >= startNodeIndex - startNodeIndex%10 + i*10 && startNodeIndex + Y - X <AstarPath.active.data.gridGraph.nodes.Length) //Mirror on X
                {
                    if (!nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y - X]))
                        nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y - X]);
                }

                if ((startNodeIndex - Y + X >= (startNodeIndex - startNodeIndex%10)-(i)*10) && startNodeIndex - Y + X < startNodeIndex - startNodeIndex%10 - (i-1)*10 && startNodeIndex - Y + X >=0) //Mirror on Y
                {
                    if (!nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y + X]))
                        nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y + X]);
                }

                if ((startNodeIndex - Y - X >= (startNodeIndex - startNodeIndex%10)-(i)*10) && startNodeIndex - Y - X < startNodeIndex - startNodeIndex%10 + (i-1)*10 && startNodeIndex - Y - X >=0) //Mirror on X & Y
                {
                    if (!nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y - X]))
                        nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y - X]);
                }
            }
        }
    }
    
    private void CreateFloor()
    {
        try
        {
            //Create rooms and doors for each path
        foreach (var node in spawnToBossPath.path)
        {
            var i = spawnToBossPath.path.IndexOf(node);
        
            var A = Vector3.zero;
            var B = Vector3.zero;
            var C = Vector3.zero;
            var dirNext = Vector3.zero;
            var dirPrevious = Vector3.zero;
        
            if (i==0)
            {
                var room = GetChallengeRoom();
                var preSpawnRoom = Instantiate(room, (Vector3)node.position, room.transform.rotation);
        
                room = GetChallengeRoom();
                var nextRoom = Instantiate(room, (Vector3)spawnToBossPath.path[i+1].position, room.transform.rotation);
                
                spawnToBossRooms.Add(preSpawnRoom);
                spawnToBossRooms.Add(nextRoom);
                
                A = (Vector3)node.position;
                B = (Vector3)spawnToBossPath.path[i + 1].position;
                dirNext = B - A;
                dirNext.Normalize();

                var roomManager = spawnToBossRooms[i].GetComponent<SC_RoomManager>();
                var nextRoomManager = nextRoom.GetComponent<SC_RoomManager>();
                
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = nextRoomManager.doorSouth;
                }
                else if (dirNext == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = nextRoomManager.doorNorth;
                }
                else if (dirNext == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = nextRoomManager.doorWest;
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = nextRoomManager.doorEast;
                }
            }
            else if (i==spawnToBossPath.path.Count-1)
            {
                A = (Vector3)node.position;
                C = (Vector3)spawnToBossPath.path[i - 1].position;
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                var roomManager = spawnToBossRooms[i].GetComponent<SC_RoomManager>();
                var previousRoomManager = spawnToBossRooms[i - 1].GetComponent<SC_RoomManager>();
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = previousRoomManager.doorSouth;
                }
                else if (dirPrevious == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = previousRoomManager.doorNorth;
                }
                else if (dirPrevious == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = previousRoomManager.doorWest;
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = previousRoomManager.doorEast;
                }
            }
            else
            {
                var room = GetChallengeRoom();
                var nextRoom = Instantiate(room, (Vector3)spawnToBossPath.path[i+1].position, room.transform.rotation);
                
                spawnToBossRooms.Add(nextRoom);
                
                A = (Vector3)node.position;
                B = (Vector3)spawnToBossPath.path[i + 1].position;
                C = (Vector3)spawnToBossPath.path[i - 1].position;
                dirNext = B - A;
                dirNext.Normalize();
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                var roomManager = spawnToBossRooms[i].GetComponent<SC_RoomManager>();
                var nextRoomManager = nextRoom.GetComponent<SC_RoomManager>();
                var previousRoomManager = spawnToBossRooms[i - 1].GetComponent<SC_RoomManager>();
                
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = nextRoomManager.doorSouth;
                }
                else if (dirNext == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = nextRoomManager.doorNorth;
                }
                else if (dirNext == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = nextRoomManager.doorWest;
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = nextRoomManager.doorEast;
                }
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = previousRoomManager.doorSouth;
                }
                else if (dirPrevious == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = previousRoomManager.doorNorth;
                }
                else if (dirPrevious == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = previousRoomManager.doorWest;
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = previousRoomManager.doorEast;
                }
                
            }
            
        }
        
        foreach (var node in spawnToChestPath.path)
        {
            var i = spawnToChestPath.path.IndexOf(node);
        
            var A = Vector3.zero;
            var B = Vector3.zero;
            var C = Vector3.zero;
            var dirNext = Vector3.zero;
            var dirPrevious = Vector3.zero;
        
            if (i==0)
            {
                
                var room = GetChallengeRoom();
                var nextRoom = spawnToBossPath.path.Contains(spawnToChestPath.path[i + 1])
                    ? spawnToBossRooms[spawnToBossPath.path.IndexOf(spawnToChestPath.path[i + 1])]
                    : Instantiate(room, (Vector3)spawnToChestPath.path[i + 1].position, room.transform.rotation);
                
                spawnToChestRooms.Add(spawnToBossRooms[0]);
                spawnToChestRooms.Add(nextRoom);
                
                A = (Vector3)node.position;
                B = (Vector3)spawnToChestPath.path[i + 1].position;
                dirNext = B - A;
                dirNext.Normalize();
        
                var roomManager = spawnToChestRooms[i].GetComponent<SC_RoomManager>();
                var nextRoomManager = nextRoom.GetComponent<SC_RoomManager>();
                
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = nextRoomManager.doorSouth;
                }
                else if (dirNext == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = nextRoomManager.doorNorth;
                }
                else if (dirNext == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = nextRoomManager.doorWest;
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = nextRoomManager.doorEast;
                }
            }
            else if (i==spawnToChestPath.path.Count-1)
            {
                A = (Vector3)node.position;
                C = (Vector3)spawnToChestPath.path[i - 1].position;
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                var roomManager = spawnToChestRooms[i].GetComponent<SC_RoomManager>();
                var previousRoomManager = spawnToChestRooms[i - 1].GetComponent<SC_RoomManager>();
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = previousRoomManager.doorSouth;
                }
                else if (dirPrevious == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = previousRoomManager.doorNorth;
                }
                else if (dirPrevious == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = previousRoomManager.doorWest;
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = previousRoomManager.doorEast;
                }
            }
            else
            {
                var room = GetChallengeRoom();
                var nextRoom = spawnToBossPath.path.Contains(spawnToChestPath.path[i + 1])
                    ? spawnToBossRooms[spawnToBossPath.path.IndexOf(spawnToChestPath.path[i + 1])]
                    : Instantiate(room, (Vector3)spawnToChestPath.path[i + 1].position, room.transform.rotation);
                
                spawnToChestRooms.Add(nextRoom);
                
                A = (Vector3)node.position;
                B = (Vector3)spawnToChestPath.path[i + 1].position;
                C = (Vector3)spawnToChestPath.path[i - 1].position;
                dirNext = B - A;
                dirNext.Normalize();
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                var roomManager = spawnToChestRooms[i].GetComponent<SC_RoomManager>();
                var nextRoomManager = nextRoom.GetComponent<SC_RoomManager>();
                var previousRoomManager = spawnToChestRooms[i - 1].GetComponent<SC_RoomManager>();
                
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = nextRoomManager.doorSouth;
                }
                else if (dirNext == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = nextRoomManager.doorNorth;
                }
                else if (dirNext == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = nextRoomManager.doorWest;
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = nextRoomManager.doorEast;
                }
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = previousRoomManager.doorSouth;
                }
                else if (dirPrevious == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = previousRoomManager.doorNorth;
                }
                else if (dirPrevious == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = previousRoomManager.doorWest;
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = previousRoomManager.doorEast;
                }
                
            }
            
        }
        
        foreach (var node in bossToChestPath.path)
        {
        
            var i = bossToChestPath.path.IndexOf(node);
        
            var A = Vector3.zero;
            var B = Vector3.zero;
            var C = Vector3.zero;
            var dirNext = Vector3.zero;
            var dirPrevious = Vector3.zero;
        
            if (i==0)
            {
                var room = GetChallengeRoom();
                var nextRoom = (spawnToBossPath.path.Contains(bossToChestPath.path[i + 1]) || spawnToChestPath.path.Contains(bossToChestPath.path[i + 1])
                    ? spawnToBossPath.path.Contains(bossToChestPath.path[i + 1])
                        ? spawnToBossRooms[spawnToBossPath.path.IndexOf(bossToChestPath.path[i + 1])]
                        : spawnToChestRooms[spawnToChestPath.path.IndexOf(bossToChestPath.path[i + 1])]
                    : Instantiate(room, (Vector3)bossToChestPath.path[i + 1].position, room.transform.rotation));
                
                bossToChestRooms.Add(spawnToBossRooms[^1]);
                bossToChestRooms.Add(nextRoom);
                
                A = (Vector3)node.position;
                B = (Vector3)bossToChestPath.path[i + 1].position;
                dirNext = B - A;
                dirNext.Normalize();
        
                var roomManager = bossToChestRooms[i].GetComponent<SC_RoomManager>();
                var nextRoomManager = nextRoom.GetComponent<SC_RoomManager>();
                
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = nextRoomManager.doorSouth;
                }
                else if (dirNext == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = nextRoomManager.doorNorth;
                }
                else if (dirNext == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = nextRoomManager.doorWest;
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = nextRoomManager.doorEast;
                }
            }
            else if (i==bossToChestPath.path.Count-1)
            {
                //bossToChestRooms.Add(spawnToChestRooms[^1]);
                Debug.LogWarning(bossToChestRooms[^1]);
                
                A = (Vector3)node.position;
                C = (Vector3)bossToChestPath.path[i - 1].position;
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                var roomManager = bossToChestRooms[i].GetComponent<SC_RoomManager>();
                var previousRoomManager = bossToChestRooms[i - 1].GetComponent<SC_RoomManager>();
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = previousRoomManager.doorSouth;
                }
                else if (dirPrevious == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = previousRoomManager.doorNorth;
                }
                else if (dirPrevious == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = previousRoomManager.doorWest;
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = previousRoomManager.doorEast;
                }
            }
            else
            {
                var room = GetChallengeRoom();
                var nextRoom = (spawnToBossPath.path.Contains(bossToChestPath.path[i + 1]) || spawnToChestPath.path.Contains(bossToChestPath.path[i + 1])
                    ? spawnToBossPath.path.Contains(bossToChestPath.path[i + 1])
                        ? spawnToBossRooms[spawnToBossPath.path.IndexOf(bossToChestPath.path[i + 1])]
                        : spawnToChestRooms[spawnToChestPath.path.IndexOf(bossToChestPath.path[i + 1])]
                    : Instantiate(room, (Vector3)bossToChestPath.path[i + 1].position, room.transform.rotation));
                
                bossToChestRooms.Add(nextRoom);
                
                A = (Vector3)node.position;
                B = (Vector3)bossToChestPath.path[i + 1].position;
                C = (Vector3)bossToChestPath.path[i - 1].position;
                dirNext = B - A;
                dirNext.Normalize();
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                var roomManager = bossToChestRooms[i].GetComponent<SC_RoomManager>();
                var nextRoomManager = nextRoom.GetComponent<SC_RoomManager>();
                var previousRoomManager = bossToChestRooms[i - 1].GetComponent<SC_RoomManager>();
                
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = nextRoomManager.doorSouth;
                }
                else if (dirNext == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = nextRoomManager.doorNorth;
                }
                else if (dirNext == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = nextRoomManager.doorWest;
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = nextRoomManager.doorEast;
                }
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    roomManager.doorNorth.EnableDoor();
                    roomManager.doorNorth.doorToConnect = previousRoomManager.doorSouth;
                }
                else if (dirPrevious == Vector3.back)
                {
                    roomManager.doorSouth.EnableDoor();
                    roomManager.doorSouth.doorToConnect = previousRoomManager.doorNorth;
                }
                else if (dirPrevious == Vector3.right)
                {
                    roomManager.doorEast.EnableDoor();
                    roomManager.doorEast.doorToConnect = previousRoomManager.doorWest;
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    roomManager.doorWest.EnableDoor();
                    roomManager.doorWest.doorToConnect = previousRoomManager.doorEast;
                }
                
            }
            
        }
        
        
        //Then add the real spawn, boos and chest room
        var spawnRoom = Instantiate(spawnRoomList[Random.Range(0, spawnRoomList.Count)], GetAvailableRoomSpace(spawnNodeIndex, out int si, 0, false), Quaternion.identity).GetComponent<SC_RoomManager>();
        var bossRoom = Instantiate(bossRoomList[Random.Range(0, bossRoomList.Count)], GetAvailableRoomSpace(bossRoomIndex, out int bi, si, true), Quaternion.identity).GetComponent<SC_RoomManager>();
        var chestRoom = Instantiate(chestRoomList[Random.Range(0, chestRoomList.Count)], GetAvailableRoomSpace(chestRoomIndex, out chestRoomIndex, bi, true), Quaternion.identity).GetComponent<SC_RoomManager>();

        spawn = spawnRoom.gameObject;
        boss = bossRoom.gameObject;
        chest = chestRoom.gameObject;
        
        SC_PlayerController.instance.transform.position = spawn.GetComponent<SC_RoomManager>().doorNorth.doorSpawnPoint.transform.position;
        
        //Link these rooms to their pre-room
        var roomPos = Vector3.zero;
        var nextRoomPos = Vector3.zero;
        var dirNextRoom = Vector3.zero;

        SC_Door nextRoomDoor = null;

        spawnRoom.doorNorth.EnableDoor();
        roomPos = spawnRoom.transform.position;
        nextRoomPos = spawnToBossRooms[0].transform.position;
        dirNextRoom = nextRoomPos - roomPos;
        dirNextRoom.Normalize();
        var preSpawnRoomManager = spawnToBossRooms[0].GetComponent<SC_RoomManager>();
        //Rotation on spawn chest and boss room may cause this to not work as intended
        nextRoomDoor = dirNextRoom == Vector3.forward ? 
            preSpawnRoomManager.doorSouth:
            dirNextRoom == Vector3.back? 
                preSpawnRoomManager.doorNorth:
                dirNextRoom == Vector3.right? 
                    preSpawnRoomManager.doorWest: 
                    preSpawnRoomManager.doorEast;
        nextRoomDoor.EnableDoor();
        spawnRoom.doorNorth.doorToConnect = nextRoomDoor;
        nextRoomDoor.doorToConnect = spawnRoom.doorNorth;
            
        bossRoom.doorNorth.EnableDoor();
        roomPos = bossRoom.transform.position;
        nextRoomPos = bossToChestRooms[0].transform.position;
        dirNextRoom = nextRoomPos - roomPos;
        dirNextRoom.Normalize();
        var preBossRoomManager = bossToChestRooms[0].GetComponent<SC_RoomManager>();
        //Rotation on spawn chest and boss room may cause this to not work as intended
        nextRoomDoor = dirNextRoom == Vector3.forward? 
            preBossRoomManager.doorSouth:
            dirNextRoom == Vector3.back? 
                preBossRoomManager.doorNorth: 
                dirNextRoom == Vector3.right? 
                    preBossRoomManager.doorWest: 
                    preBossRoomManager.doorEast;
        nextRoomDoor.EnableDoor();
        bossRoom.doorNorth.doorToConnect = nextRoomDoor;
        nextRoomDoor.doorToConnect = bossRoom.doorNorth;
        
        
        chestRoom.doorNorth.EnableDoor();
        roomPos = chestRoom.transform.position;
        nextRoomPos = spawnToChestRooms[^1].transform.position;
        dirNextRoom = nextRoomPos - roomPos;
        dirNextRoom.Normalize();
        var preChestRoomManager = spawnToChestRooms[^1].GetComponent<SC_RoomManager>();
        //Rotation on spawn chest and boss room may cause this to not work as intended
        nextRoomDoor = dirNextRoom == Vector3.forward? 
            preChestRoomManager.doorSouth:
            dirNextRoom == Vector3.back? 
                preChestRoomManager.doorNorth: 
                dirNextRoom == Vector3.right? 
                    preChestRoomManager.doorWest: 
                    preChestRoomManager.doorEast;
        nextRoomDoor.EnableDoor();
        chestRoom.doorNorth.doorToConnect = nextRoomDoor;
        nextRoomDoor.doorToConnect = chestRoom.doorNorth;
        
        
        }
        catch (Exception e)
        {
            Debug.LogError($"Generating a new floor, cause of failure : {e.Message}");
            SetupGeneration();
            throw;
        }
    }

    private Vector3 GetAvailableRoomSpace(int nodeIndex, out int indexOfNewNode, int nodeIndexToExclude, bool canExcludeNode)
    {
        nodesInRange.Clear();
        GetNodesInRange(nodeIndex, 1, 1);
        var availableNodesInRange = nodesInRange.ToList();
        foreach (var node in nodesInRange)
        {
            if (spawnToBossPath.path.Contains(node) || spawnToChestPath.path.Contains(node) || bossToChestPath.path.Contains(node))
                availableNodesInRange.Remove(node);
        }

        if (canExcludeNode)
        {
            if (availableNodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[nodeIndexToExclude]))
                availableNodesInRange.Remove(AstarPath.active.data.gridGraph.nodes[nodeIndexToExclude]);
        }

        var newNode = availableNodesInRange[Random.Range(0, availableNodesInRange.Count)];
        indexOfNewNode = newNode.NodeInGridIndex;
        availableNodesInRange.Clear();
        
        return (Vector3)newNode.position;
    }
    
}