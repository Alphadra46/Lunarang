public class SC_SkillManagingCommand : SC_Command
{
    public override string descText => "Give/Remove skills.";
    
    public override void Execute(string[] args)
    {
        
        
        switch (args.Length)
        {
            
            case <= 1:
                SC_DebugConsole.instance.PrintLine("<color=red> Please enter add or remove and the skillname.");
                break;
            case > 2:
                
                var action = args[1];
                var skillName = args[2];

                // skillName = skillName.Split("'");

                if (SC_GameManager.instance.playerSkillInventory.FindSkillByName(skillName) != null)
                {
                    var skill = SC_GameManager.instance.playerSkillInventory.FindSkillByName(skillName);
                    
                    switch (action)
                    {
                        case "add" :
                            SC_GameManager.instance.playerSkillInventory.AddSkill(skill);
                            SC_DebugConsole.instance.PrintLine("<color=green> Skill : " + skillName + " added.");
                            return;
                    }
                    
                }
                else
                {
                    SC_DebugConsole.instance.PrintLine("<color=red> Please enter a valid skill, looser.");
                    return;
                }
                
                SC_DebugConsole.instance.PrintLine("<color=red> Please enter add/remove and the skillname.");
                
                break;
            
        }
    }
}
