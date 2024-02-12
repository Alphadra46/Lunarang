using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        SC_InputManager.instance.pause.started += OnPauseKey;
    }

    

    private void OnDisable()
    {
        SC_InputManager.instance.pause.started -= OnPauseKey;
    }

    private bool CheckEntityType(string id)
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
                SceneManager.LoadScene(1, LoadSceneMode.Single);
                SC_UIManager.instance.ResetTempReferences();
                break;
            case GameState.RUN:
                if (isPause) SetPause();
                if (SC_PlayerController.instance != null)
                {
                    SC_PlayerController.instance.FreezeMovement(false);
                    SC_PlayerController.instance.FreezeDash(false);
                }
                SC_UIManager.instance.ResetTempReferences();
                break;
            case GameState.DEFEAT:
                if (!isPause) SetPause();
                SC_UIManager.instance.ShowHUD();
                SC_UIManager.instance.ShowGameOverUI();
                break;
            case GameState.WIN:
                if (!isPause) SetPause();
                SC_UIManager.instance.ShowHUD();
                SC_UIManager.instance.ShowWinUI();
                break;
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
    
    public void OpenRewardChest()
    {
        if(SC_SkillManager.instance.allCurrentRunSkills.Count < 1) return;
        
        SetPause();
        SC_UIManager.instance.ShowRewardMenu();
    }
    
    public void OnPauseKey(InputAction.CallbackContext obj)
    {
        if (SC_UIManager.instance.inventoryUI != null || SC_UIManager.instance.rewardUI != null ||
            SC_UIManager.instance.winUI != null || SC_UIManager.instance.gameOverUI != null) return;
        SetPause();
        SC_UIManager.instance.ShowPauseMenu();
    }
    
    // public void OnEscapeUI(GameObject uiToLeave)
    // {
    //     SetPause();
    //     
    //     Destroy(uiToLeave);
    //     uiToLeave = null;
    //     SC_UIManager.instance.ShowHUD();
    // }
    
    
}
