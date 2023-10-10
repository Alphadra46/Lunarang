using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Skill")]
public class SC_Skill : ScriptableObject
{
    public bool isReinforced;

    public void ReinforceSkill()
    {
        isReinforced = true;
    }
}
