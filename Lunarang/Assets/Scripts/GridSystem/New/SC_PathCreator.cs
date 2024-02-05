using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_PathCreator : MonoBehaviour
{
    //public GameObject doorPrefab_TEMP;

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

    private List<Vector3> doorsPositions = new List<Vector3>();

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
        
        if (spawnToBossPath.IsDone() && spawnToChestPath.IsDone() && bossToChestPath.IsDone() && Input.GetKeyDown(KeyCode.G))
        {
            DisplayPath();
        }

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
    /// Display paths and spacial rooms 
    /// </summary>
    private void DisplayPath()
    {
            //Create a spawn room
            var spawnRoom = spawnRoomList[Random.Range(0, spawnRoomList.Count)];
            var s = Instantiate(spawnRoom, (Vector3)AstarPath.active.data.gridGraph.nodes[spawnNodeIndex].position, spawnRoom.transform.rotation);
            spawnToBossRooms.Add(s);
            spawnToChestRooms.Add(s); //TODO - Broken
            
            //Then the boss room
            var bossRoom = bossRoomList[Random.Range(0, bossRoomList.Count)];
            var b = Instantiate(bossRoom, (Vector3)AstarPath.active.data.gridGraph.nodes[bossRoomIndex].position, bossRoom.transform.rotation);
            bossToChestRooms.Add(b); //TODO - Broken

            //Next is the chest room
            var chestRoom = chestRoomList[Random.Range(0, chestRoomList.Count)];
            var c = Instantiate(chestRoom, (Vector3)AstarPath.active.data.gridGraph.nodes[chestRoomIndex].position, chestRoom.transform.rotation);
            

            //Finally the path between each room
            //Spawn to Boss
            foreach (var node in spawnToBossPath.path)
            {
                //TODO - Remove this condition for each path because it will not be the actual "end" room (spawn, boss, chest, etc...)
                if (AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == spawnNodeIndex || AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == bossRoomIndex)
                    continue;
                
                var room = GetChallengeRoom();
                spawnToBossRooms.Add(Instantiate(room, (Vector3)node.position, room.transform.rotation));
            }
            spawnToBossRooms.Add(b);
            
            //Spawn to Chest
            foreach (var node in spawnToChestPath.path)
            {
                if (AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == spawnNodeIndex || AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == chestRoomIndex)
                    continue;
                
                if (spawnToBossPath.path.Contains(node))
                {
                    spawnToChestRooms.Add(spawnToBossRooms[spawnToBossPath.path.IndexOf(node)]);
                    continue;
                }
                
                var room = GetChallengeRoom();
                spawnToChestRooms.Add(Instantiate(room, (Vector3)node.position, room.transform.rotation));
            }
            spawnToChestRooms.Add(c);

            //Boss to Chest
            foreach (var node in bossToChestPath.path)
            {
                if (AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == bossRoomIndex || AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == chestRoomIndex)
                    continue;
                
                if (spawnToBossPath.path.Contains(node) || spawnToChestPath.path.Contains(node))
                {
                    bossToChestRooms.Add(spawnToBossPath.path.Contains(node)
                        ? spawnToBossRooms[spawnToBossPath.path.IndexOf(node)]
                        : spawnToChestRooms[spawnToChestPath.path.IndexOf(node)]);
                    continue;
                }
                
                var room = GetChallengeRoom();
                bossToChestRooms.Add(Instantiate(room, (Vector3)node.position, room.transform.rotation));
            }
            bossToChestRooms.Add(c);
            
            CreateDoors();
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
        
        //Set a random spawn location if wanted
        spawnNodeIndex = useRandomOnSpawnLocation
            ? Random.Range(0, AstarPath.active.data.gridGraph.nodes.Length - 1)
            : originalSpawnNodeIndex;
        
        //---------- Boss room ----------
        //Get all of the room between these two ranges to create the boss room and a path between the spawn and the boss room
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

    /// <summary>
    /// Display all of the doors
    /// </summary>
    private void DisplayDoors()
    {
        foreach (var doorPosition in doorsPositions)
        {
            // if (doorPosition.normalized == Vector3.forward)
            // {
            //     Debug.Log("North !");
            // }
            // else if (doorPosition.normalized == Vector3.back)
            // {
            //     Debug.Log("South !");
            // }
            // else if (doorPosition.normalized == Vector3.right)
            // {
            //     Debug.Log("East !");
            // }
            // else if (doorPosition.normalized == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
            // {
            //     Debug.Log("West !");
            // }
            //Destroy(Instantiate(doorPrefab_TEMP, doorPosition, doorPrefab_TEMP.transform.rotation),3f); //TODO - Remove Destroy
        }
    }
    
    /// <summary>
    /// Create all the doors between rooms so that we can actually change room.
    /// </summary>
    private void CreateDoors()
    {
        doorsPositions.Clear();
        
        //First the main path between spawn and boss room in one direction
        for (int i = 0; i < spawnToBossPath.path.Count-1; i++)
        {
            //Take point A position
            var A = (Vector3)spawnToBossPath.path[i].position;
            //Then point B position
            var B = (Vector3)spawnToBossPath.path[i + 1].position;
            //And get the direction
            var dir = B - A;
            dir.Normalize(); //Normalize to get a direction 

            if (!doorsPositions.Contains((Vector3)spawnToBossPath.path[i].position + 10 * dir))
            {
                doorsPositions.Add((Vector3)spawnToBossPath.path[i].position + 10 * dir);
                
                 if (dir == Vector3.forward)
                 {
                     spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                 }
                 else if (dir == Vector3.back)
                 {
                     spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                 }
                 else if (dir == Vector3.right)
                 {
                     spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                 }
                 else if (dir == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                 {
                     spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                 }
            }
        }
        
        //Then go back to do the rest of the doors for the main path
        for (int i = spawnToBossPath.path.Count-1; i > 0; i--)
        {
            //Take point A position
            var A = (Vector3)spawnToBossPath.path[i].position;
            //Then point B position
            var B = (Vector3)spawnToBossPath.path[i - 1].position;
            //And get the direction
            var dir = B - A;
            dir.Normalize(); //Normalize to get a direction 
            
            if (!doorsPositions.Contains((Vector3)spawnToBossPath.path[i].position + 10*dir))
            {
                doorsPositions.Add((Vector3)spawnToBossPath.path[i].position + 10*dir);
                
                if (dir == Vector3.forward)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.back)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.right)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dir == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
            }
        }
        
        //And the same goes for the path between spawn and chest room
        for (int i = 0; i < spawnToChestPath.path.Count-1; i++)
        {
            //Take point A position
            var A = (Vector3)spawnToChestPath.path[i].position;
            //Then point B position
            var B = (Vector3)spawnToChestPath.path[i + 1].position;
            //And get the direction
            var dir = B - A;
            dir.Normalize(); //Normalize to get a direction 
            
            if (!doorsPositions.Contains((Vector3)spawnToChestPath.path[i].position + 10*dir))
            {
                doorsPositions.Add((Vector3)spawnToChestPath.path[i].position + 10*dir);
                
                //TODO - Broken
                
                if (dir == Vector3.forward)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.back)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.right)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dir == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
            }
        }
        
        for (int i = spawnToChestPath.path.Count-1; i > 0; i--)
        {
            //Take point A position
            var A = (Vector3)spawnToChestPath.path[i].position;
            //Then point B position
            var B = (Vector3)spawnToChestPath.path[i - 1].position;
            //And get the direction
            var dir = B - A;
            dir.Normalize(); //Normalize to get a direction 
            
            if (!doorsPositions.Contains((Vector3)spawnToChestPath.path[i].position + 10*dir))
            {
                doorsPositions.Add((Vector3)spawnToChestPath.path[i].position + 10*dir);
                
                //TODO - Broken
                
                if (dir == Vector3.forward)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.back)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.right)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dir == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
            }
        }
        
        //And for the path between boss and chest room
        for (int i = 0; i < bossToChestPath.path.Count-1; i++)
        {
            //Take point A position
            var A = (Vector3)bossToChestPath.path[i].position;
            //Then point B position
            var B = (Vector3)bossToChestPath.path[i + 1].position;
            //And get the direction
            var dir = B - A;
            dir.Normalize(); //Normalize to get a direction 
            
            if (!doorsPositions.Contains((Vector3)bossToChestPath.path[i].position + 10*dir))
            {
                doorsPositions.Add((Vector3)bossToChestPath.path[i].position + 10*dir);
                
                //TODO - Broken
                
                if (dir == Vector3.forward)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.back)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.right)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dir == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
            }
        }
        
        for (int i = bossToChestPath.path.Count-1; i > 0; i--)
        {
            //Take point A position
            var A = (Vector3)bossToChestPath.path[i].position;
            //Then point B position
            var B = (Vector3)bossToChestPath.path[i - 1].position;
            //And get the direction
            var dir = B - A;
            dir.Normalize(); //Normalize to get a direction 
            
            if (!doorsPositions.Contains((Vector3)bossToChestPath.path[i].position + 10*dir))
            {
                doorsPositions.Add((Vector3)bossToChestPath.path[i].position + 10*dir);
                
                //TODO - Broken
                
                if (dir == Vector3.forward)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.back)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dir == Vector3.right)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dir == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
            }
        }
        
        //DisplayPath();
        //DisplayDoors();
        
    }

    private void CreateFloor()
    {
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
        
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.back)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.right)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
            }
            else if (i==spawnToBossPath.path.Count-1)
            {
                A = (Vector3)node.position;
                C = (Vector3)spawnToBossPath.path[i - 1].position;
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.back)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.right)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
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
        
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.back)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.right)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.back)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.right)
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToBossRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
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
        
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.back)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.right)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
            }
            else if (i==spawnToChestPath.path.Count-1)
            {
                A = (Vector3)node.position;
                C = (Vector3)spawnToChestPath.path[i - 1].position;
                dirPrevious = C - A;
                dirPrevious.Normalize();
        
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.back)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.right)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
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
        
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.back)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.right)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.back)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.right)
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    spawnToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
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
        
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.back)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.right)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
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
        
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.back)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.right)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
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
        
                //Check for the next room
                if (dirNext == Vector3.forward)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.back)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.right)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirNext == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
                
                //Check for the previous room
                if (dirPrevious == Vector3.forward)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorNorth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.back)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorSouth.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.right)
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorEast.gameObject.SetActive(true);
                }
                else if (dirPrevious == Vector3.left) //Not necessary but it's just in case the doorPosition behave weirdly
                {
                    bossToChestRooms[i].GetComponent<SC_RoomManager>().doorWest.gameObject.SetActive(true);
                }
                
            }
            
        }
    }
    
}
