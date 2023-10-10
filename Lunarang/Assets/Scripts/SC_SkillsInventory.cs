using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Middle-Men/Skill Inventory")]
public class SC_SkillsInventory : ScriptableObject
{
    //Temp - GameObject will be replaced by the skill class
    public List<SC_Skill> skillInventory = new List<SC_Skill>();

    //TODO when skill class is created
    public void AddSkill()
    {
        
    }
}
