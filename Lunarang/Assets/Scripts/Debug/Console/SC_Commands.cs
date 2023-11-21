using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public struct Commands
{
    [TableColumnWidth(50)]
    public string name;
    public string description;
    [TableColumnWidth(25)]
    public List<string> aliases;

}

public class SC_Commands : MonoBehaviour
{
    private static SC_Commands instance;
    public SC_DebugConsole console;
    
    private void Awake()
    {
        if(instance!=null) Destroy(this);
        instance = this;
    }


    public void SendCommand(string text)
    {
        var textSplited = text.ToLower().Split(" ");
        var command = textSplited[0];

        switch (command)
        {
            
            // Command : Kill
            case "kill":
                // No argument
                if (textSplited.Length <= 1)
                {
                    Destroy(FindObjectOfType<SC_PlayerController>().gameObject);
                    console.PrintLine("> <color=#42adf5>Player <color=white>has been killed.");
                }
                else
                {
                    // More than one argument
                    if(textSplited[1].Split(",", StringSplitOptions.RemoveEmptyEntries).Length > 1){
                        var args = textSplited[1].Split(",", StringSplitOptions.RemoveEmptyEntries);
                    
                        if (GameObject.Find(args[0]) == null)
                        {
                            console.PrintLine("<color=red> No entity with this name has been found.");
                            return;
                        }

                        var allEntities = FindObjectsOfType<SC_AIStats>().ToList();
                        print(allEntities.Count);

                        foreach (var e in allEntities.Where(e => e.typeID == args[0]))
                        {
                            Destroy(e);
                        }
                        console.PrintLine("<color=#42adf5>"+ args[0] + " <color=white>has been killed.");
                        
                    }
                    else
                    // One argument
                    {
                        
                        var arg = textSplited[1];
                        
                        // Kill all entities of a certain type
                        if(arg.Contains("@e"))
                        {
                            
                            if (arg.Contains(":"))
                            {
                                
                                var id = arg.Split(":")[1];
                                var entities = SC_GameManager.instance.FindEntityType(id);

                                if (entities == null)
                                {
                                    console.PrintLine("<color=red> No entities with this type has been found.");
                                    return;
                                }
                            
                                foreach (var entity in entities)
                                {
                                    Destroy(entity.gameObject);
                                }

                                console.PrintLine(" <color=white>All entities of type : " + "<color=#42adf5>"+ id + " <color=white>has been killed.");
                                
                            }
                            
                            else
                            {
                                
                                var entities = FindObjectsOfType<SC_AIStats>().ToList();
                                foreach (var entity in entities)
                                {
                                    Destroy(entity.gameObject);
                                }
                                
                                console.PrintLine(" <color=white>All entities has been killed.");
                                
                            }
                            
                        }
                        
                    }
                    
                }
                    
                break;
            
            case "summon":

                switch (textSplited.Length)
                {
                    
                    case <= 1:
                        console.PrintLine("<color=red> Please enter an entity type.");
                        break;
                    case 2:
                        foreach (var entity in from prefab in SC_GameManager.instance.prefabsEntities where textSplited[1] == prefab.name.ToLower() select Instantiate(prefab))
                        {
                            var player = GameObject.FindWithTag("Player").transform;

                            entity.transform.position = player.position;
                            
                        }
                        
                        console.PrintLine("<color=green> Entity : " + textSplited[1] + " summoned at player pos");
                        break;
                    case > 2:
                    {
                        var coords = textSplited[2].Split(":")[1].Split(",");
                        var x = coords[0];
                        var y = coords[1];
                        
                        foreach (var entity in from prefab in SC_GameManager.instance.prefabsEntities where textSplited[1] == prefab.GetComponent<SC_AIStats>().typeID select Instantiate(prefab))
                        {
                            if (!textSplited[2].Contains("@pos:")) continue;
                            

                            entity.transform.position = new Vector3(float.Parse(x), 0, float.Parse(y));

                        }
                        
                        console.PrintLine("<color=green> Entity : " + textSplited[1] + " summoned at loc : x: " + coords[0] + " y: " + coords[1]);
                        break;
                    }
                    
                }
                
                break;
            
            case "godmode":
                if (textSplited.Length > 1)
                {
                    console.PrintLine("<color=red> > Looser.");
                }
                else
                {
                    SC_PlayerStats.instance.isGod = !SC_PlayerStats.instance.isGod;
                    console.PrintLine(SC_PlayerStats.instance.isGod
                        ? "<color=#ffac26> > You've been elevated has a god."
                        : "<color=red> > You've just fallen back in with the losers.");
                }
                
                break;
            
            case "help":
                foreach (var c in SC_GameManager.instance.commands)
                {
                    console.PrintLine("<color=white> > " + c.name + " : " + c.description);
                }
                console.PrintLine("<color=white> Here's all the avaible commands : ");
                break;
            
            default:
                Destroy(console.currentUI);
                SC_PlayerController.instance.canMove = true;
                break;
        }

        console.commandline.text = null;
        console.commandline.Select();

    }
    
}
