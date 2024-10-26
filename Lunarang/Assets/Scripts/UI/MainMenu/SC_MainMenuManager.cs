using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_MainMenuManager : MonoBehaviour
{

    public static SC_MainMenuManager instance;


    [Header("Menus")]
    public GameObject mainMenu; //The main menu panel
    
    public GameObject settingsPrefab;
    public Canvas canvas;
    
    public TextMeshProUGUI versionText;
    
    //Used for the back button. Chose to use a Stack (FILO) so that the first selection the user make will be the last one when we use the back button.
    public readonly Stack<GameObject> previousSelectedGameObject = new Stack<GameObject>(); 
    
    
    private void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {

    }

    private void OnEnable()
    {

    }

    public void OpenSettings()
    {
        
        Instantiate(settingsPrefab, canvas.transform).name = "SettingsUI";
        
    }

    public void Play()
    {

        SC_GameManager.instance.ChangeState(SC_GameManager.isFirstLaunch? GameState.LOBBY : GameState.FTUE);
        
    }

    private void Update()
    {
        if(versionText.text != "v" + SC_GameManager.instance.gameVersion) versionText.text = "v" + SC_GameManager.instance.gameVersion;
    }
}
