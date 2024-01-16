using UnityEngine;

public class SC_StatsDebugCommand : SC_Command
{
    public override string descText => "Show all player stats.";
    
    /// <summary>
    /// Switch player state to God, cancelling all damages.
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public override void Execute(string[] args)
    {
        
        if (args.Length == 1)
        {
            var statsUI = Object.Instantiate(SC_DebugConsole.instance.StatsUI).GetComponent<SC_StatsDebug>();
            SC_PlayerStats.instance.statsDebug = statsUI;
            statsUI.LoadStats();
            statsUI.InsantiateStats();
        }
        else
        {
            SC_DebugConsole.instance.PrintLine("<color=red> > Please type /stats.");
        }
        
    }
}
