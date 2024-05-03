using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_ConstellationUI : MonoBehaviour
{
    public SC_Constellation constellation;
    [ShowInInspector, ReadOnly] public List<SC_SkillBranchUI> skillBranches = new List<SC_SkillBranchUI>();

    private void Start()
    {
        int i = 0;
        foreach (var skillBranch in transform.GetComponentsInChildren<SC_SkillBranchUI>())
        {
            skillBranches.Add(skillBranch);
            var branchParent = constellation.skills.Keys.ToList()[i];
            skillBranch.SkillBranchSetup(branchParent, constellation.skills[branchParent].ToList());
            i++;
        }
    }
}
