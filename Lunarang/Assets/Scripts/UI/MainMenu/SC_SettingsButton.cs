using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SettingsButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Select the settings menu and activate it
    /// </summary>
    public void SelectSettingsMenu()
    {
        //Adding te main menu panel to the stack
        SC_MainMenuManager.instance.previousSelectedGameObject.Push(SC_MainMenuManager.instance.mainMenu);
        
        //And de-activating the main menu panel while activating the setting panel
        SC_MainMenuManager.instance.mainMenu.SetActive(false);
    }
    
}
