public class SC_GodmodeCommand : SC_Command
{
    public override string descText => "Elevate the player to god.";

    public override void Execute(string[] args)
    {
        
        if (args.Length > 1)
        {
            SC_DebugConsole.instance.PrintLine("<color=red> > Looser.");
        }
        else
        {
            SC_PlayerStats.instance.isGod = !SC_PlayerStats.instance.isGod;
            SC_DebugConsole.instance.PrintLine(SC_PlayerStats.instance.isGod
                ? "<color=#ffac26> > You've been elevated has a god."
                : "<color=red> > You've just fallen back in with the losers.");
        }
        
    }
}