using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_ReloadSceneCommand : SC_Command
{
    public override string descText => "Reload the current scene.";
    
    /// <summary>
    /// Teleport the player to the desired location.
    /// </summary>
    /// <param name="args">Parameters of the actions.</param>
    public override void Execute(string[] args)
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SC_DebugConsole.instance.PrintLine("<color=#ffac26> > Scene reloaded");

    }
}