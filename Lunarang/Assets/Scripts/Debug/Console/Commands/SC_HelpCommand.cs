public class SC_HelpCommand : SC_Command
{
    public override string descText => "Show all commands.";
    
    /// <summary>
    /// Print all commands available in the console log.
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public override void Execute(string[] args)
    {
        
         foreach (var c in SC_CommandLineSystem.commands)
         {
             SC_DebugConsole.instance.PrintLine("<color=white> > " + c.Key + " : " + c.Value.descText);
         }
         SC_DebugConsole.instance.PrintLine("<color=white> Here's all the avaible commands : ");
        
    }
}
