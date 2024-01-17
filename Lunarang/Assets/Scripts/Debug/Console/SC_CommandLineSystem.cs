using System.Collections.Generic;
using UnityEngine;

public static class SC_CommandLineSystem
{
    private const string cmdNotFound = "Command file \"{0}\" not found";

    public static readonly Dictionary<string, SC_Command> commands = new Dictionary<string, SC_Command>()
    {
        {"kill", new SC_KillCommand()},
        {"summon", new SC_SummonCommand()},
        {"help", new SC_HelpCommand()},
        {"gm", new SC_GodmodeCommand()},
        {"tp", new SC_TeleportCommand()},
        {"rl", new SC_ReloadSceneCommand()},
        {"stats", new SC_StatsDebugCommand()}
    };
    
    /// <summary>
    /// Finds a command based on a string, then executes it.
    /// </summary>
    /// <param name="input">String key to find an command</param>
    public static void Execute(string input)
    {
        
        var lines = input.Split(new string[] { "\r\n", "\n\r", "\n" },
            System.StringSplitOptions.RemoveEmptyEntries);
        
        if(lines.Length == 0) return;

        foreach (var command in lines)
        {
            var parameters = command.Split(new char[] { ' ' });
            
            if(commands.ContainsKey(parameters[0])) commands[parameters[0]].Execute(parameters);
            else SC_DebugConsole.instance.PrintLine(string.Format(cmdNotFound, parameters[0]));

        }
        
    }

}
