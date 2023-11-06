using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_ButtonTypes : MonoBehaviour
{
    
    public enum Types //Depending on the type of button a different script will be added to fit the type
    {
        Play,
        Settings,
        Quit,
        Panel,
        Back
    }

    public Types buttonType;

    private Button button; //Get the instance of the button
    
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        
        StartCoroutine(DelayedTypeSelection());
    }

    /// <summary>
    /// Delay the Type selection of the button
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayedTypeSelection()
    {
        yield return null; //A little bit of delay just to be sure everything we need is instantiated so there is no null reference
        
        switch (buttonType)
        {
            case Types.Play:
                var playButton = gameObject.AddComponent<SC_PlayButton>();
                button.onClick.AddListener(playButton.Play);
                break;
            case Types.Settings:
                var settingsButton = gameObject.AddComponent<SC_SettingsButton>();
                //Go to the settings menu
                break;
            case Types.Quit:
                var quitButton = gameObject.AddComponent<SC_QuitButton>();
                button.onClick.AddListener(quitButton.QuitGame);
                break;
            case Types.Panel:
                var panelButton = gameObject.AddComponent<SC_PanelButton>();
                //Select a panel (mostly used in settings like audio, graphics or others), it will allow the user to access the settings of the selected panel
                break;
            case Types.Back:
                var backButton = gameObject.AddComponent<SC_BackButton>();
                //Go to previous UI (menu, panel, button, etc...)
                break;
            default:
                break;
        }
    }
}
