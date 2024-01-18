using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_PathCreator : MonoBehaviour
{
    public GameObject cube;
    [Range(0,99)]
    public int startNodeIndex;
    [Range(0,10)]
    public int range;
    private ABPath p;

    private bool isInit = false;

    private List<GridNode> nodesInRange = new List<GridNode>();

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(AstarPath.active.data.gridGraph.nodes[1].position);

        //Not random
        // p = ABPath.Construct((Vector3)AstarPath.active.data.gridGraph.nodes[0].position,(Vector3)AstarPath.active.data.gridGraph.nodes[15].position);

        
        //Random
        GetNodesInRange(range);
        //var start = (Vector3)AstarPath.active.data.gridGraph.nodes[Random.Range(0, AstarPath.active.data.gridGraph.nodes.Length)].position;
        var start = (Vector3)AstarPath.active.data.gridGraph.nodes[startNodeIndex].position;
        var end = (Vector3)AstarPath.active.data.gridGraph.nodes[nodesInRange[Random.Range(0,nodesInRange.Count)].NodeInGridIndex].position;
        p = ABPath.Construct(start, end);
        
        AstarPath.StartPath(p);

        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInit)
            return;
        
        if (p.IsDone() && Input.GetKeyDown(KeyCode.G))
        {

            foreach (var node in p.path)
            {
                Destroy(Instantiate(cube, (Vector3)node.position, cube.transform.rotation),3f);
            }
        }

        if (p.IsDone() && Input.GetKeyDown(KeyCode.R))
        {
            foreach (var node in nodesInRange)
            {
                Destroy(Instantiate(cube, (Vector3)node.position,cube.transform.rotation),3f);
            }
        }
    }
    
    public void GetNodesInRange(int searchRange)
    {
        nodesInRange.Clear();
            
        for (int i = 0; i < searchRange+1; i++)
        {
            var Y = i * 10;
            var X = searchRange - i;

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
