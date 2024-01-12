using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_PauseMenu : MonoBehaviour
{

    public void ButtonQuit()
    {
        SC_GameManager.instance.QuitGame();        
    }

    public void ButtonReturn()
    {
        SC_GameManager.instance.SetPause();
        SC_UIManager.instance.ShowPauseMenu();
    }
    
}
