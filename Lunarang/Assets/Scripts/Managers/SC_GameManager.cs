using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

    public static Action clearRoom;
    
    #region Variables

    public GameState state = GameState.LOBBY;
    
    [Title("Settings")]
    [PropertySpace(SpaceBefore = 10)]
    public List<GameObject> prefabsEntities = new List<GameObject>();
    
    public bool isPause = false;

    [Title("Settings")]
    [ShowInInspector] public List<GameObject> allInteractables = new List<GameObject>();


    [HideInInspector] public SC_ResourcesInventory playerResourceInventory;
    [HideInInspector] public SO_SkillInventory playerSkillInventory;
    [HideInInspector] public SO_ConsumablesInventory playerConsumablesInventory;

    [HideInInspector] public SO_ArchiveInventory archivesInventory;
    [HideInInspector] public SO_WeaponInventory weaponInventory;
    
    [ShowInInspector] public List<SO_BaseSkill> allSkills = new List<SO_BaseSkill>();

    [Title("Settings"),PropertySpace(SpaceBefore = 15f)] 
    [ShowInInspector] public string gameVersion = "v0.0.5";
    #endregion
    
    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
        
        playerResourceInventory = Resources.Load<SC_ResourcesInventory>("ResourceInventory");
        playerSkillInventory = Resources.Load<SO_SkillInventory>("SkillInventory");
        playerConsumablesInventory = Resources.Load<SO_ConsumablesInventory>("ConsumableInventory");

        archivesInventory = Resources.Load<SO_ArchiveInventory>("ArchiveInventory");
        weaponInventory = Resources.Load<SO_WeaponInventory>("WeaponInventory");
    }

    private void Start()
    {
        SC_InputManager.instance.pause.started += OnPauseKey;
    }

    private void OnEnable()
    {
        playerSkillInventory.ClearInventory();
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
                SC_UIManager.instance.CreateLoadingScreen(1);
                SC_UIManager.instance.ResetTempReferences();
                playerSkillInventory.ClearInventory();
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
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    
    
    public void OpenInventory()
    {
        SC_UIManager.instance.ShowInventory();
    }

    public void OpenForge()
    {
        SC_UIManager.instance.ShowForge();
    }
    
    public void OpenLibrary()
    {
        SC_UIManager.instance.ShowLibrary();
    }
    
    public void OpenRewardChest()
    {
        //if(SC_SkillManager.instance.allCurrentRunSkills.Count < 1) return;
        
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
    
}
