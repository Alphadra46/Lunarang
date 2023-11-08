using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_DebugConsole : MonoBehaviour
{

    #region Variables
    
    public GameObject UIPrefab;
    public GameObject TextLinePrefab;
    public GameObject currentUI;
    private VerticalLayoutGroup textLinesPanel;
    public TMP_InputField commandline;
    public SC_Commands commands;

    public string[] separators = {" "};
    
    #endregion


    #region Init

    private void Start()
    {
        SC_InputManager.instance.console.started += ShowConsole;
        commands = gameObject.AddComponent<SC_Commands>();
        commands.console = this;
    }

    private void ShowConsole(InputAction.CallbackContext ctx)
    {

        
        
        if (currentUI != null)
        {
            Destroy(currentUI);
            SC_PlayerController.instance.canMove = true;
        }
        else
        {
            SC_PlayerController.instance.canMove = false;
            currentUI = Instantiate(UIPrefab);
            if(!currentUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).TryGetComponent(out commandline)) return;
            if(!currentUI.transform.GetChild(0).GetChild(0).TryGetComponent(out textLinesPanel)) return;
            
            commandline.onEndEdit.AddListener(commands.SendCommand);
            commandline.onValueChanged.AddListener(OnChange);
            
            EventSystem.current.SetSelectedGameObject(commandline.gameObject);
        }
        
    }

    private void OnChange(string text)
    {

        // switch (text)
        // {
        //     case string a when a.Contains("e:"):
        //
        //         var array = GameObject.FindGameObjectsWithTag("Entity").ToList();
        //         
        //         
        //         
        //         break;
        // }
        
    }
    


    public void PrintLine(string text)
    {
        
        var line = Instantiate(TextLinePrefab, textLinesPanel.transform);
        if(!line.transform.GetChild(0).TryGetComponent(out TMP_Text lineTMP)) return;

        lineTMP.text = text;

    }

    #endregion
    
    
    
}
