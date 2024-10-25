﻿using System.Linq;
using UnityEngine;

public class SC_KillCommand : SC_Command
{
    public override string descText => "Kill an entity targeted.";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public override void Execute(string[] args)
    {
        
        // Without argument
        if (args.Length <= 1)
        {
            
            var screenPos = Input.mousePosition;
                
            var ray = Camera.main!.ScreenPointToRay(screenPos);

            if (!Physics.Raycast(ray, out var hitData)) return;
            
            if (hitData.collider.CompareTag("Entity") && hitData.collider.TryGetComponent(out SC_AIStats _ai))
            {
                _ai.Death();
                SC_DebugConsole.instance.PrintLine("<color=#42adf5>"+ hitData.collider.name + " <color=white>has been killed.");
            }
            else if(hitData.collider.CompareTag("Player") && hitData.collider.TryGetComponent(out SC_PlayerStats _p))
            {
                _p.Death();
                SC_DebugConsole.instance.PrintLine("<color=#42adf5>"+ hitData.collider.name + " <color=white>has been killed.");
            }
            
        }
        
        // With arguments
        else
        {
            var arg = args[1];
             
            // Only specified entities
            if(arg.Contains("@e"))
            {
                // Kill all entities of a certain type
                if (arg.Contains(":"))
                {
                    
                    var id = arg.Split(":")[1];
                    var entities = SC_GameManager.instance.FindEntityType(id);
            
                    if (entities == null)
                    {
                        SC_DebugConsole.instance.PrintLine("<color=red> No entities with this type has been found.");
                        return;
                    }
                
                    foreach (var entity in entities)
                    {
                        entity.Death();
                    }
            
                    SC_DebugConsole.instance.PrintLine(" <color=white>All entities of type : " + "<color=#42adf5>"+ id + " <color=white>has been killed.");
                    
                }
                // Kill all entities except the player.
                else
                {
                    
                    var entities = Object.FindObjectsOfType<SC_AIStats>().ToList();
                    foreach (var entity in entities)
                    {
                        entity.Death();
                    }
                    
                    SC_DebugConsole.instance.PrintLine(" <color=white>All entities has been killed.");
                    
                }
                
            }
            
            // Kill Player
            else if (arg.Contains("@p"))
            {
                SC_PlayerStats.instance.Death();
                SC_DebugConsole.instance.PrintLine("> <color=#42adf5>Player <color=white>has been killed.");
            }
            
        }

    }
    
}
