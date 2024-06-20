using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SC_InventoryConstellationManager : MonoBehaviour
{

    #region Variables

    [PropertySpace(SpaceBefore = 15f)] public TextMeshProUGUI TitleTMP;
    
    [PropertySpace(SpaceBefore = 15f)]
    public GameObject parentSkillTemplate;
    public Transform parentTransform;

    [PropertySpace(SpaceBefore = 15f)]
    public GameObject childSkillsPanelTemplate;
    public GameObject childSkillTemplate;
    [PropertySpace(SpaceBefore = 5f)]
    public Transform childPanelTransform;

    [PropertySpace(SpaceBefore = 15f)]
    public SC_Constellation constellation;
    
    #endregion

    public void Init(SC_Constellation newConstellation, string consteID)
    {
        constellation = newConstellation;
        TitleTMP.text = consteID;
        
        GenerateParentSkills();
        GenerateChildsSkills();
    }

    public void GenerateParentSkills()
    {

        var parentSkills = SC_GameManager.instance.playerSkillInventory.GetAllParentSkillsByConstellation(constellation);

        var index = 0;
        
        foreach (var parentSkill in parentSkills)
        {

            var skillGO = Instantiate(parentSkillTemplate, parentTransform);
            if(skillGO.TryGetComponent(out SC_InventoryParentSkillTemplate sc)) sc.Init(parentSkill, "PST"+index);

            
            skillGO.SetActive(true);
            index++;
        }
        
    }
    
    public void GenerateChildsSkills()
    {

        var parentSkills = SC_GameManager.instance.playerSkillInventory.GetAllParentSkillsByConstellation(constellation);

        var childID = 0;

        while (childID != 4)
        {

            var childPanel = Instantiate(childSkillsPanelTemplate, childPanelTransform);
            
            foreach (var parentSkill in parentSkills)
            {
                var childSkill = (SO_ChildSkill) parentSkill.childrenSkills[childID];
                
                var childSkillGO = Instantiate(childSkillTemplate, childPanel.transform);
                if(childSkillGO.TryGetComponent(out SC_InventoryChildSkillTemplate sc)) sc.Init(childSkill, childID);
                
                childSkillGO.SetActive(true);
            }
            
            childPanel.SetActive(true);
            
            childID++;
            
        }

    }
    
}
