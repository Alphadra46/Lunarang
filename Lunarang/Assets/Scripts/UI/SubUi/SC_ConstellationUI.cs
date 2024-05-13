using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_ConstellationUI : MonoBehaviour
{
    public SC_Constellation constellation;
    [ShowInInspector, ReadOnly] public List<SC_SkillBranchUI> skillBranches = new List<SC_SkillBranchUI>();
    private int selectedPanelIndex;

    private void Start()
    {
        int i = 0;
        foreach (var skillBranch in transform.GetComponentsInChildren<SC_SkillBranchUI>())
        {
            skillBranches.Add(skillBranch);
            var branchParent = constellation.skills.Keys.ToList()[i];
            skillBranch.SkillBranchSetup(branchParent, constellation.skills[branchParent].ToList(), constellation);
            i++;
        }

        transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = constellation.name;

    }

    private void OnEnable()
    {
        StartCoroutine(DelayedEnable());
    }

    private IEnumerator DelayedEnable()
    {
        yield return null;
        yield return null;
        selectedPanelIndex = 0;
        skillBranches[selectedPanelIndex].PanelSelection();
        SC_InputManager.instance.switchToLeft.started += SwitchPanelLeft;
        SC_InputManager.instance.switchToRight.started += SwitchPanelRight;
        SC_InputManager.instance.cancel.started += ReturnToMainPage;
    }
    
    private void OnDisable()
    {
        SC_InputManager.instance.switchToLeft.started -= SwitchPanelLeft;
        SC_InputManager.instance.switchToRight.started -= SwitchPanelRight;
        SC_InputManager.instance.cancel.started -= ReturnToMainPage;
    }

    private void SwitchPanelLeft(InputAction.CallbackContext context)
    {
        skillBranches[selectedPanelIndex].PanelDeselection();
        
        selectedPanelIndex--;
        if (selectedPanelIndex < 0)
            selectedPanelIndex = skillBranches.Count - 1;
        
        skillBranches[selectedPanelIndex].PanelSelection();
    }
    
    private void SwitchPanelRight(InputAction.CallbackContext context)
    {
        skillBranches[selectedPanelIndex].PanelDeselection();

        selectedPanelIndex++;
        if (selectedPanelIndex > skillBranches.Count - 1)
            selectedPanelIndex = 0;
        
        skillBranches[selectedPanelIndex].PanelSelection();
    }

    private void ReturnToMainPage(InputAction.CallbackContext context)
    {
        SC_SkillTreeUI.instance.mainPage.SetActive(true);
        gameObject.SetActive(false);
    }
    
}
