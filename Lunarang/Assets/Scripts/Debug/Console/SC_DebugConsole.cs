using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_DebugConsole : MonoBehaviour
{

    public static SC_DebugConsole instance;
    
    #region Variables
    
    public GameObject UIPrefab;
    public GameObject TextLinePrefab;
    public GameObject currentUI;
    private VerticalLayoutGroup textLinesPanel;
    public TMP_InputField commandline;

    [ShowInInspector] private List<string> history = new List<string>();
    
    public string[] separators = {" "};
    
    #endregion


    #region Init

    /// <summary>
    /// Convert to Singleton.
    /// </summary>
    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
        
        // DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Initialize all references and inputs.
    /// </summary>
    private void Start()
    {
        SC_InputManager.instance.console.started += ShowConsole;
        
    }

    private void OnDisable()
    {
        SC_InputManager.instance.console.started -= ShowConsole;
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
                return;
            
            if(history.Count > 0){
                UpdateLog();
            }
            else
            {
                textLinesPanel.gameObject.SetActive(false);
            }
            
            commandline.onSubmit.AddListener(SendCommand);
            
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
    /// Send current input to the CommandLineSystem.
    /// </summary>
    /// <param name="input"></param>
    public void SendCommand(string input)
    {
        SC_CommandLineSystem.Execute(input);
        commandline.text = "";
    }

    /// <summary>
    /// Create a new text line in the command line log.
    /// </summary>
    /// <param name="text">String that showed in the text line.</param>
    public void PrintLine(string text)
    {
        history.Add(text);
        UpdateLog();
    }

    public void UpdateLog()
    {
        
        if(!textLinesPanel.gameObject.activeInHierarchy) textLinesPanel.gameObject.SetActive(true);
        
        foreach (Transform child in textLinesPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var line in history)
        {
            var instantiateLine = Instantiate(TextLinePrefab, textLinesPanel.transform);
            
            if(!instantiateLine.transform.GetChild(0).TryGetComponent(out TMP_Text lineTMP)) return;
            lineTMP.text = line;
        }
        
    }

    public void ClearLog()
    {

        foreach (Transform child in textLinesPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
    }

    #endregion



}
