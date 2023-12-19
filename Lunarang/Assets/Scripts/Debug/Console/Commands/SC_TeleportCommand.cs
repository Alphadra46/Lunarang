using System.Linq;
using UnityEngine;

public class SC_TeleportCommand : SC_Command
{
    public override string descText => "Teleport the player at a certain location.";
    
    /// <summary>
    /// Teleport the player to the desired location.
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public override void Execute(string[] args)
    {
        var player = GameObject.FindWithTag("Player").GetComponent<SC_PlayerController>();
        
        if (args.Length < 1)
        {
            SC_DebugConsole.instance.PrintLine("<color=red> > tp <location>.");
        }
        else
        {
            var waypoints = GameObject.FindGameObjectsWithTag("Waypoint").Where(wp => args[1] == wp.GetComponent<SC_WaypointComponent>().id);

            foreach (var wp in waypoints)
            {
                var id = wp.GetComponent<SC_WaypointComponent>().id; 
                var loc = wp.GetComponent<SC_WaypointComponent>().loc;   
                
                player.Teleport(loc);
                SC_DebugConsole.instance.PrintLine("<color=#ffac26> > You've been teleported to " + id + " at location : " + loc);
               

            }
            
        }
        
    }
}