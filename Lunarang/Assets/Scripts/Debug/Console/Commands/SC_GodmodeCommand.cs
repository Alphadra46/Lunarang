public class SC_GodmodeCommand : SC_Command
{
    public override string descText => "Elevate the player to god.";

    
    /// <summary>
    /// Switch player state to God, cancelling all damages.
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public override void Execute(string[] args)
    {
        
        if (args.Length > 1)
        {
            SC_DebugConsole.instance.PrintLine("<color=red> > Looser.");
        }
        else
        {
            SC_PlayerStats.instance.isGod = !SC_PlayerStats.instance.isGod;
            SC_PlayerStats.instance.ApplyBuffToSelf("ATK", SC_PlayerStats.instance.isGod ? 1000 : -1000);
            SC_PlayerStats.instance.ApplyBuffToSelf("CD", SC_PlayerStats.instance.isGod ? 250 : -250);
            SC_PlayerStats.instance.ApplyBuffToSelf("CR", SC_PlayerStats.instance.isGod ? 95 : -95);
            SC_DebugConsole.instance.PrintLine(SC_PlayerStats.instance.isGod
                ? "<color=#ffac26> > You've been elevated has a god."
                : "<color=red> > You've just fallen back in with the losers.");
        }
        
    }
}