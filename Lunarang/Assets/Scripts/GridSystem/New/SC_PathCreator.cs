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

    private List<GridNode> nodesInRange = new List<GridNode>();

    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log(AstarPath.active.data.gridGraph.nodes[1].position);

        //Not random
        p = ABPath.Construct((Vector3)AstarPath.active.data.gridGraph.nodes[0].position,(Vector3)AstarPath.active.data.gridGraph.nodes[15].position);

        //Random
        //p = ABPath.Construct((Vector3)AstarPath.active.data.gridGraph.nodes[Random.Range(0,AstarPath.active.data.gridGraph.nodes.Length)].position,);
        
        AstarPath.StartPath(p);
    }

    // Update is called once per frame
    void Update()
    {
        if (p.IsDone() && Input.GetKeyDown(KeyCode.G))
        {
            nodesInRange.Clear();
            // foreach (var node in p.path)
            // {
            //     Destroy(Instantiate(cube, (Vector3)node.position, cube.transform.rotation),5f);
            // }
            
            
            
            for (int i = 0; i < range+1; i++)
            {
                var Y = i * 10;
                var X = range - i;
                

                //TODO - Check if there is already a cube on a node so that it can't spawn (using position)
                
                if ((startNodeIndex + Y + X<(startNodeIndex - startNodeIndex%10)+(i+1)*10) && ((startNodeIndex - startNodeIndex%10) + Y + X>=i*10))
                {
                    if (nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y + X]))
                        continue;
                    
                    nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y + X]);
                    //Destroy(Instantiate(cube, (Vector3)AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y + X].position, cube.transform.rotation),3f); //Initial Point
                }

                if ((startNodeIndex + Y - X<(startNodeIndex - startNodeIndex%10)+(i+1)*10) && ((startNodeIndex - startNodeIndex%10) + Y - X>=i*10))
                {
                    if (nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y - X]))
                        continue;
                    
                    nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y - X]);
                    //Destroy(Instantiate(cube, (Vector3)AstarPath.active.data.gridGraph.nodes[startNodeIndex + Y - X].position, cube.transform.rotation),3f); //Mirror on X
                }

                if ((startNodeIndex - Y + X<(startNodeIndex - startNodeIndex%10)+(-i+1)*10) && ((startNodeIndex - startNodeIndex%10) - Y + X>=-i*10))
                {
                    if (nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y + X]))
                        continue;
                    
                    nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y + X]);
                    //Destroy(Instantiate(cube, (Vector3)AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y + X].position, cube.transform.rotation),3f); //Mirror on Y
                }

                if ((startNodeIndex - Y - X<(startNodeIndex - startNodeIndex%10)+(-i+1)*10) && ((startNodeIndex - startNodeIndex%10) - Y - X>=-i*10))
                {
                    if (nodesInRange.Contains(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y - X]))
                        continue;

                    nodesInRange.Add(AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y - X]);
                    //Destroy(Instantiate(cube, (Vector3)AstarPath.active.data.gridGraph.nodes[startNodeIndex - Y - X].position, cube.transform.rotation),3f); //Mirror on X & Y
                }
            }

            foreach (var node in nodesInRange)
            {
                Destroy(Instantiate(cube, (Vector3)node.position,cube.transform.rotation),3f);
            }
            
        }

        
    }
}
