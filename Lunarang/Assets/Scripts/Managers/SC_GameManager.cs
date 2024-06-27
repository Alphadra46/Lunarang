using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum GameState
{
    Menu,
    FTUE,
    LOBBY,
    TRAINING,
    RUN,
    DEFEAT,
    WIN
}

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager instance;

    public static Action clearRoom;
    
    public static bool isFirstLaunch;
    public static bool isTutorialFinished;
    
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

    [Title("Game Variables"), PropertySpace(SpaceBefore = 15f)]
    [ReadOnly, ShowInInspector] private float runTimer = 0;
    [ReadOnly, ShowInInspector] private float bestTimer = 0;
    [ReadOnly, ShowInInspector] private List<float> lastTimers = new List<float>();
    
    [ReadOnly, ShowInInspector] private int killCounter = 0;
    [ReadOnly, ShowInInspector] private int bestKillCounter;
    [ReadOnly, ShowInInspector] private List<int> lastKillCounters = new List<int>();
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
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += EditorStateChange;
#endif
    }

    private void Update()
    {
        
        if(state == GameState.RUN) IncrementingTimer();
        
        //Debug Inputs
        if(Input.GetKeyDown(KeyCode.Keypad9)) SC_PlayerStats.instance.TakeDamage(5, false, null, true);
        if(Input.GetKeyDown(KeyCode.Keypad8)) SC_PlayerStats.instance.Heal(10);
        if(Input.GetKeyDown(KeyCode.N)) SC_UIManager.instance.CreateLoadingScreen(0);
        
    }

#if UNITY_EDITOR
    private void EditorStateChange(PlayModeStateChange playModeState)
    {
        if (playModeState == PlayModeStateChange.EnteredEditMode)
        {
            isFirstLaunch = false;
            isTutorialFinished = false;
        }
    }
#endif
    

    #region Timer

    private void IncrementingTimer()
    {

        runTimer += Time.deltaTime;

    }

    public float GetRunTimeElapsed()
    {

        return runTimer;

    }

    private void ResetTimer()
    {

        if(runTimer < 1) return;
        
        if (runTimer < bestTimer) bestTimer = runTimer;
        
        lastTimers.Add(runTimer);

        runTimer = 0;

    }

    #endregion
    
    
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
            case GameState.FTUE:
                if (isPause) SetPause();
                
                if(SceneManager.GetActiveScene().buildIndex != 1)
                    SC_UIManager.instance.CreateLoadingScreen(1);
                
                if (SC_PlayerController.instance != null)
                {
                    SC_PlayerController.instance.FreezeMovement(false);
                    SC_PlayerController.instance.FreezeDash(false);
                }
                
                SC_AIStats.onDeath += IncrementingKillCounter;
                
                SC_UIManager.instance.ResetTempReferences();
                ResetTimer();
                ResetKillCounter();
                
                break;
            
            case GameState.LOBBY:
                if (isPause) SetPause();
                if(SceneManager.GetActiveScene().buildIndex != 2)
                    SC_UIManager.instance.CreateLoadingScreen(2);
                SC_UIManager.instance.ResetTempReferences();
                
                SC_AIStats.onDeath -= IncrementingKillCounter;
                
                ResetTimer();
                ResetKillCounter();
                break;
            case GameState.TRAINING:
                if (isPause) SetPause();
                
                if(SceneManager.GetActiveScene().buildIndex != 3)
                    SC_UIManager.instance.CreateLoadingScreen(3);
                
                if (SC_PlayerController.instance != null)
                {
                    SC_PlayerController.instance.FreezeMovement(false);
                    SC_PlayerController.instance.FreezeDash(false);
                }
                
                SC_AIStats.onDeath -= IncrementingKillCounter;
                
                SC_UIManager.instance.ResetTempReferences();
                ResetTimer();
                ResetKillCounter();
                break;
            case GameState.RUN:
                if (isPause) SetPause();
                Resources.Load<SO_SkillInventory>("SkillInventory").SavePreSelectedSkills();
                if(SceneManager.GetActiveScene().buildIndex != 4)
                    SC_UIManager.instance.CreateLoadingScreen(4);
                
                if (SC_PlayerController.instance != null)
                {
                    SC_PlayerController.instance.FreezeMovement(false);
                    SC_PlayerController.instance.FreezeDash(false);
                }
                
                SC_AIStats.onDeath += IncrementingKillCounter;
                
                SC_UIManager.instance.ResetTempReferences();
                ResetTimer();
                ResetKillCounter();
                
                break;
            case GameState.DEFEAT:
                
                if (!isPause) SetPause();
                
                SC_UIManager.instance.ShowHUD();
                SC_UIManager.instance.ShowGameOverUI();
                
                SC_AIStats.onDeath -= IncrementingKillCounter;
                
                ResetTimer();
                ResetKillCounter();
                break;
            case GameState.WIN:
                if (!isPause) SetPause();
                SC_UIManager.instance.ShowHUD();
                SC_UIManager.instance.ShowWinUI();
                
                SC_AIStats.onDeath -= IncrementingKillCounter;
                
                ResetTimer();
                ResetKillCounter();
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

    
    #region Kill Counter

    private void IncrementingKillCounter(SC_AIStats aiKilled)
    {
        print("ALLER TA MERE");
        
        killCounter += 1;
        print("Kill : " + killCounter);
        
    }
    private void ResetKillCounter()
    {

        // if(killCounter < 1) return;
        //
        // if (killCounter < bestKillCounter) bestKillCounter = killCounter;
        //
        // lastKillCounters.Add(killCounter);
        //
        // killCounter = 0;

    }

    public int GetKillCounter()
    {

        return killCounter;

    }
    
    

    #endregion
    
    
    public void OpenInventory()
    {
        if (SC_UIManager.instance.pauseUI != null || SC_UIManager.instance.rewardUI != null ||
            SC_UIManager.instance.winUI != null || SC_UIManager.instance.gameOverUI != null) return;
        
        SetPause();
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

    public void OpenAltar()
    {
        SC_UIManager.instance.ShowAltar();
    }
    
    public void OpenRewardChest()
    {
        
        SetPause();
        SC_UIManager.instance.ShowRewardMenu();
        
    }
    
    public void OpenArchiveUI(SO_Archive archive)
    {
        SetPause();
        if (SC_UIManager.instance.archiveDiscoveredUI != null)
        {
            SC_InputManager.instance.pause.started += OnPauseKey;
        }
        else
        {
            SC_InputManager.instance.pause.started -= OnPauseKey;
        }
        
        SC_UIManager.instance.ShowArchiveDiscovered(archive);
        
    }
    
    public void OnPauseKey(InputAction.CallbackContext obj)
    {
        if (state != GameState.TRAINING && state != GameState.RUN) return;
        
        if (SC_UIManager.instance.inventoryUI != null || SC_UIManager.instance.rewardUI != null ||
            SC_UIManager.instance.winUI != null || SC_UIManager.instance.gameOverUI != null) return;
        
        SetPause();
        SC_UIManager.instance.ShowPauseMenu();

    }
    
}
