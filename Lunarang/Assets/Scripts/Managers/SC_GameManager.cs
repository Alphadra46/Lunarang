﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum GameState
{
    LOBBY,
    RUN,
    DEFEAT,
    WIN
}

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager instance;
    
    #region Variables

    public GameState state = GameState.LOBBY;
    
    [Title("Settings")]
    [PropertySpace(SpaceBefore = 10)]
    public List<GameObject> prefabsEntities = new List<GameObject>();
    
    public bool isPause = false;

    [Title("Settings")]
    [ShowInInspector] public List<GameObject> allInteractables = new List<GameObject>();
    
    #endregion
    
    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
    }

    private void Start()
    {
        Debug.Log("Start");
        SC_InputManager.instance.pause.started += context => { SetPause(); SC_UIManager.instance.ShowPauseMenu(); };
    }

    public bool CheckEntityType(string id)
    {
        var allEntities = FindObjectsOfType<SC_AIStats>().ToList();

        return allEntities.Count(e => e.typeID == id) > 0;
    }
    
    public List<SC_AIStats> FindEntityType(string id)
    {
        var allEntities = FindObjectsOfType<SC_AIStats>().ToList();

        return CheckEntityType(id) ? allEntities.Where(e => e.typeID == id).ToList() : null;
    }


    public void SetPause()
    {
        
        isPause = !isPause;
        Time.timeScale = isPause ? 0 : 1;

    }

    public void ChangeState(GameState newState)
    {
        state = newState;

        switch (state)
        {
            case GameState.LOBBY:
                if (isPause) SetPause();
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                SC_UIManager.instance.ShowHUD();
                break;
            case GameState.RUN:
                break;
            case GameState.DEFEAT:
                if (!isPause) SetPause();
                SC_UIManager.instance.ShowHUD();
                SC_UIManager.instance.ShowGameOverUI();
                break;
            case GameState.WIN:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void OpenInventory()
    {
        SetPause();
        SC_UIManager.instance.ShowInventory();
    }

    public void OpenForge()
    {
        SetPause();
        SC_UIManager.instance.ShowForge();
    }
    
}
