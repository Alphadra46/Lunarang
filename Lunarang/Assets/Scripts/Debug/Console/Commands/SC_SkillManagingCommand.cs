public class SC_SkillManagingCommand : SC_Command
{
    public override string descText => "Give/Remove skills.";
    
    public override void Execute(string[] args)
    {
        switch (args.Length)
        {
            
            case <= 1:
                SC_DebugConsole.instance.PrintLine("<color=red> Please enter add/remove.");
                break;
            case > 2:
                
                SC_DebugConsole.instance.PrintLine("<color=red> Please enter add/remove.");
                
                break;
            
        }
    }
}
