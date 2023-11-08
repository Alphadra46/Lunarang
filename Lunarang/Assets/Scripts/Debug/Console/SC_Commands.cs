using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_Commands : MonoBehaviour
{
    public static SC_Commands instance;
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

                        foreach (var e in allEntities)
                        {
                            if(e.id == args[0]) Destroy(e);
                        }
                        console.PrintLine("<color=#42adf5>"+ args[0] + " <color=white>has been killed.");
                        
                    }
                    else
                    // One argument
                    {
                        
                        var arg = textSplited[1];
                        
                        // Kill all entities of a certain type
                        if(arg.Contains("@e:"))
                        {

                            var id = arg.Split(":")[1];
                            var allEntities = FindObjectsOfType<SC_AIStats>().ToList();
                            print(allEntities.Count);

                            foreach (var e in allEntities.Where(e => e.id == id))
                            {
                                Destroy(e.gameObject);
                            }
                            
                            console.PrintLine(" <color=white>All entities of type : " + "<color=#42adf5>"+ id + " <color=white>has been killed.");
                            
                        }
                        
                    }
                    
                }
                    
                break;
            
            case "summon":

                if (textSplited.Length <= 1)
                {
                    console.PrintLine("<color=red> Please enter an entity type.");
                }
                else
                {
                    
                    
                    
                }
                
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
