using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SC_Lobby : MonoBehaviour
{

    public static SC_Lobby instance;
    
    public GameObject lobbyUI;

    private Selectable lastSelected;

    private void Awake()
    {
        
        if(instance != null) Destroy(this);

        instance = this;

    }

    public void ShowLobby()
    {
        
        lobbyUI.SetActive(true);
        lastSelected.Select();
        
        print("MARCHE");
        
    }

    public void TrainingRoom(Selectable selectable)
    {
        lastSelected = selectable;
        SC_GameManager.instance.ChangeState(GameState.TRAINING);
    }

    public void Temple(Selectable selectable)
    {
        lastSelected = selectable;
        SC_GameManager.instance.ChangeState(GameState.RUN);
    }
    
    public void MainMenu(Selectable selectable)
    {
        lastSelected = selectable;
        SC_UIManager.instance.CreateLoadingScreen(0);
    }

    public void Library(Selectable selectable)
    {
        lastSelected = selectable;
        SC_GameManager.instance.OpenLibrary();
        
    }
    
    public void Forge(Selectable selectable)
    {
        
        lastSelected = selectable;
        SC_GameManager.instance.OpenForge();
        lobbyUI.SetActive(false);
        
    }

    public void Altar(Selectable selectable)
    {
        
        lastSelected = selectable;
        SC_GameManager.instance.OpenAltar();
        lobbyUI.SetActive(false);
        
    }
}
