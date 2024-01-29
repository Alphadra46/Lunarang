using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_PathCreator : MonoBehaviour
{
    public GameObject pathPrefab_TEMP;
    public GameObject doorPrefab_TEMP;

    [Header("General setting")]
    [TabGroup("Settings", "Generation values")] public bool setSeed;
    [TabGroup("Settings", "Generation values"), ShowIf("setSeed")] public int customSeed;
    [TabGroup("Settings", "Generation values"), ShowInInspector, ReadOnly] private int seed;

    [Header("Normal room prefabs lists")]
    [TabGroup("Settings", "Prefabs")] public List<GameObject> easyRoomList = new List<GameObject>();
    [TabGroup("Settings", "Prefabs")] public List<GameObject> mediumRoomList = new List<GameObject>();
    [TabGroup("Settings", "Prefabs")] public List<GameObject> hardRoomList = new List<GameObject>();

    [Header("Chest room prefabs lists")] 
    [TabGroup("Settings", "Prefabs")] public List<GameObject> chestRoomList = new List<GameObject>();
    [Header("Boss room prefabs lists")]
    [TabGroup("Settings", "Prefabs")] public List<GameObject> bossRoomList = new List<GameObject>();
    [Header("Spawn room prefabs lists")]
    [TabGroup("Settings", "Prefabs")] public List<GameObject> spawnRoomList = new List<GameObject>();

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
    private ABPath spawnToChestPath;
    private ABPath bossToChestPath;

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

    /// <summary>
    /// Display paths and spacial rooms 
    /// </summary>
    private void DisplayPath()
    {
        //Create a spawn room
            var spawnRoom = spawnRoomList[Random.Range(0, spawnRoomList.Count - 1)];
            Destroy(Instantiate(spawnRoom, (Vector3)AstarPath.active.data.gridGraph.nodes[spawnNodeIndex].position, spawnRoom.transform.rotation), 3f);
            
            //Then the boss room
            var bossRoom = bossRoomList[Random.Range(0, bossRoomList.Count - 1)];
            Destroy(Instantiate(bossRoom, (Vector3)AstarPath.active.data.gridGraph.nodes[bossRoomIndex].position, bossRoom.transform.rotation), 3f);

            //Next is the chest room
            var chestRoom = chestRoomList[Random.Range(0, chestRoomList.Count - 1)];
            Destroy(Instantiate(chestRoom, (Vector3)AstarPath.active.data.gridGraph.nodes[chestRoomIndex].position, chestRoom.transform.rotation), 3f);
            

            //Finally the path between each room
            //Spawn to Boss
            foreach (var node in spawnToBossPath.path)
            {
                if (AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == spawnNodeIndex || AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == bossRoomIndex)
                    continue;
                
                Destroy(Instantiate(pathPrefab_TEMP, (Vector3)node.position, pathPrefab_TEMP.transform.rotation),3f);
            }

            //Spawn to Chest
            foreach (var node in spawnToChestPath.path)
            {
                if (spawnToBossPath.path.Contains(node))
                    continue;
                
                if (AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == spawnNodeIndex || AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == chestRoomIndex)
                    continue;
                
                Destroy(Instantiate(pathPrefab_TEMP, (Vector3)node.position, pathPrefab_TEMP.transform.rotation),3f);
            }

            //Boss to Chest
            foreach (var node in bossToChestPath.path)
            {
                if (spawnToBossPath.path.Contains(node) || spawnToChestPath.path.Contains(node))
                    continue;
                
                if (AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == bossRoomIndex || AstarPath.active.data.gridGraph.nodes.ToList().IndexOf((GridNode)node) == chestRoomIndex)
                    continue;
                
                Destroy(Instantiate(pathPrefab_TEMP, (Vector3)node.position, pathPrefab_TEMP.transform.rotation),3f);
            }
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
        
        CreateDoors();
        
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
            Destroy(Instantiate(doorPrefab_TEMP, doorPosition, doorPrefab_TEMP.transform.rotation),3f);
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

            if (!doorsPositions.Contains((Vector3)spawnToBossPath.path[i].position + 10*dir))
            {
                doorsPositions.Add((Vector3)spawnToBossPath.path[i].position + 10*dir);
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
            }
        }
        
        DisplayPath();
        DisplayDoors();
        
    }
    
}
