
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SC_SummonCommand : SC_Command
{
    public override string descText => "Summon an entity.";
    
    
    public override void Execute(string[] args)
    {
        switch (args.Length)
        {
            
            case <= 1:
                SC_DebugConsole.instance.PrintLine("<color=red> Please enter an entity type.");
                break;
            case 2:
                var entityID = args[1];
                
                foreach (var entity in from prefab in SC_GameManager.instance.prefabsEntities where entityID == prefab.GetComponent<SC_AIStats>().typeID select Object.Instantiate(prefab))
                {
                    entity.GetComponent<NavMeshAgent>().enabled = false;
                    var screenPos = Input.mousePosition;
                    
                    var ray = Camera.main!.ScreenPointToRay(screenPos);
        
                    if (!Physics.Raycast(ray, out var hitData)) continue;
                    var worldPos = hitData.point;
                    entity.transform.position = new Vector3(worldPos.x, 0.5f, worldPos.z);
                    entity.GetComponent<NavMeshAgent>().enabled = true;
                    
                    SC_DebugConsole.instance.PrintLine("<color=green> Entity : " + entityID + " summoned at mouse position.");
                    return;
                }
                
                SC_DebugConsole.instance.PrintLine("<color=red> Entity : " + entityID + " doesn't exist.");
                
                break;
            
        }
    }
    
}