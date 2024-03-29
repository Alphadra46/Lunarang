
public class SC_RoomManagingCommand : SC_Command
{
    public override string descText => "Manage the current room.";

    
    /// <summary>
    /// Switch player state to God, cancelling all damages.
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public override void Execute(string[] args)
    {
        
        if (args.Length <= 1)
        {
            SC_DebugConsole.instance.PrintLine("<color=red> > Looser.");
        }
        else
        {
            var arg = args[1];
            
            // Only specified entities
            if (arg.Contains("clear"))
            {
                SC_DebugConsole.instance.PrintLine("<color=#ffac26> > Room cleared.");
                SC_GameManager.clearRoom?.Invoke();
            }
            
        }
        
    }
}