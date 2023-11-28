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
    
    /// <summary>
    /// Initialize all references and inputs.
    /// </summary>
    private void Start()
    {
        SC_InputManager.instance.console.started += ShowConsole;
        commands = gameObject.AddComponent<SC_Commands>();
        commands.console = this;
    }

    /// <summary>
    /// Open the command text box and an command line log.
    /// </summary>
    /// <param name="ctx"></param>
    private void ShowConsole(InputAction.CallbackContext ctx)
    {
        
        if (currentUI != null)
        {
            Destroy(currentUI);
            SC_InputManager.instance.EnableGeneralInputs();
        }
        else
        {
            SC_InputManager.instance.DisableGeneralInputs();
            
            currentUI = Instantiate(UIPrefab);
            if(!currentUI.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).TryGetComponent(out commandline)) return;
            if (!currentUI.transform.GetChild(0).GetChild(0).GetChild(1).TryGetComponent(out textLinesPanel))
            {
                textLinesPanel.gameObject.SetActive(false);
                return;
            }
            
            commandline.onEndEdit.AddListener(commands.SendCommand);
            
            EventSystem.current.SetSelectedGameObject(commandline.gameObject);
        }
        
    }
    
    /// <summary>
    /// Force the Event System to focus on the CommandLine.
    /// </summary>
    private void Update()
    {
        
        if (currentUI == null) return;

        if (commandline.isFocused) return;
        
        EventSystem.current.SetSelectedGameObject(commandline.gameObject, null);
        commandline.OnPointerClick(new PointerEventData(EventSystem.current));

    }


    /// <summary>
    /// Create a new text line in the command line log.
    /// </summary>
    /// <param name="text">String that showed in the text line.</param>
    public void PrintLine(string text)
    {
        if(!textLinesPanel.gameObject.activeInHierarchy) textLinesPanel.gameObject.SetActive(true);
        
        var line = Instantiate(TextLinePrefab, textLinesPanel.transform);
        if(!line.transform.GetChild(0).TryGetComponent(out TMP_Text lineTMP)) return;

        lineTMP.text = text;

    }

    #endregion
    
    
    
}
