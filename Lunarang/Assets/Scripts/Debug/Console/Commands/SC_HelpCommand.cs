public class SC_HelpCommand : SC_Command
{
    public override string descText => "Show all commands.";
    
    public override void Execute(string[] args)
    {
        
         foreach (var c in SC_CommandLineSystem.commands)
         {
             SC_DebugConsole.instance.PrintLine("<color=white> > " + c.Key + " : " + c.Value.descText);
         }
         SC_DebugConsole.instance.PrintLine("<color=white> Here's all the avaible commands : ");
        
    }
}
