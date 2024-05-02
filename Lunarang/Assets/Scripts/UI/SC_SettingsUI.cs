using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_SettingsUI : SerializedMonoBehaviour
{

    public Dictionary<string, GameObject> panels;
    
    [PropertySpace(SpaceBefore = 15f)]
    [ShowInInspector, ReadOnly] private string currentPanelName;

    private void Awake()
    {
        SwitchPanel("Video");
    }

    public void SwitchPanel(string panelName)
    {
        
        if(!string.IsNullOrEmpty(currentPanelName)) panels[currentPanelName].SetActive(false);
        
        panels[panelName].SetActive(true);

        currentPanelName = panelName;

    }

    public void Close()
    {
        
        Destroy(gameObject);
        
    }

}
