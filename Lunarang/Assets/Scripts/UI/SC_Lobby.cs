using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_Lobby : MonoBehaviour
{
    public void TrainingRoom()
    {
        SC_GameManager.instance.ChangeState(GameState.TRAINING);
    }

    public void Temple()
    {
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }
    
    public void MainMenu()
    {
        SC_UIManager.instance.CreateLoadingScreen(0);
    }

    public void Library()
    {
        
        SC_GameManager.instance.OpenLibrary();
        
    }
    
    public void Forge()
    {
        
        SC_GameManager.instance.OpenForge();
        
    }

    public void Altar()
    {
        SC_GameManager.instance.OpenAltar();
    }
}
