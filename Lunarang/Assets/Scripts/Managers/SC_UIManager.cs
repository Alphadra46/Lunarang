using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SC_UIManager : MonoBehaviour
{
    public static SC_UIManager instance;

    #region Variables

    public GameObject UIParent;
    
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject inventoryUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject HUD;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject pauseUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject rewardUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject gameOverUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject winUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject forgeUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject libraryUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject loadingScreenPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject settingsPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject altarUIPrefab;

    #region Temporary References
    
    // Main
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject hudUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject inventoryUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject pauseUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject rewardUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject gameOverUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject winUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject loadingScreenUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] public GameObject settingsUI;
    
    // Buildings
    [BoxGroup("Temporary References")]
    [ShowInInspector] private GameObject forgeUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] private GameObject libraryUI;

    [BoxGroup("Temporary References")] 
    [SerializeField] private GameObject altarUI;

    private GameObject statsUI;

#endregion

    #endregion

    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;

        if (UIParent == null)
        {
            UIParent = GameObject.FindWithTag("UIParent");
        }

        //if (SceneManager.GetActiveScene().buildIndex != 2 && SceneManager.GetActiveScene().buildIndex != 3) return;
        
        ResetTempReferences();
        InstantiateHUD();
    }
    private void OnDestroy()
    {
        ResetTempReferences();
    }
    
    private void InstantiateHUD()
    {
        if (hudUI == null)
        {
            hudUI = Instantiate(HUD, UIParent.transform);
            hudUI.name = "HUD";
        }
        else
        {
            Destroy(hudUI);
        }
    }
    
    public void ShowHUD()
    {
        hudUI.SetActive(!hudUI.activeInHierarchy);
    }

    /// <summary>
    /// Instanciate Inventory Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowInventory()
    {
        
        if (inventoryUI == null)
        {
            inventoryUI = Instantiate(inventoryUIPrefab, UIParent.transform);
            inventoryUI.name = "InventoryUI";
            ShowHUD();
        }
        else
        {
            Destroy(inventoryUI);
            ShowHUD();
        }
        
    }
    
    /// <summary>
    /// Instanciate Pause Menu Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowPauseMenu()
    {
        
        if (pauseUI == null)
        {
            
            
            pauseUI = Instantiate(pauseUIPrefab, UIParent.transform);
            pauseUI.name = "PauseUI";
            ShowHUD();
            
            EventSystem.current.SetSelectedGameObject(pauseUI.transform.GetChild(1).GetChild(0).gameObject);
            
        }
        else
        {
            
            Destroy(pauseUI);
            pauseUI = null;
            ShowHUD();
        }
        
    }
    
    /// <summary>
    /// Instanciate Reward Menu Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowRewardMenu()
    {

        if (rewardUI == null)
        {
            rewardUI = Instantiate(rewardUIPrefab, UIParent.transform);
            rewardUI.name = "RewardUI";
            SC_RewardManager.instance.ChestRewardSelection(rewardUI.GetComponent<SC_RewardUI>());
            
            SC_InputManager.instance.submit.Disable();
            ShowHUD();
            
            // EventSystem.current.SetSelectedGameObject(rewardUI.transform.GetChild(1).gameObject);
        }
        else
        {
            
            Destroy(rewardUI);
            rewardUI = null;
            ShowHUD();
        }
        
    }
    
    /// <summary>
    /// Instanciate Forge UI Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowForge()
    {
        
        if (forgeUI == null)
        {
            forgeUI = Instantiate(forgeUIPrefab);
            forgeUI.name = "ForgeUI";
        }
        else
        {
            Destroy(forgeUI);
        }
        
    }

    public void ShowAltar()
    {
        if (altarUI == null)
        {
            altarUI = Instantiate(altarUIPrefab);
            altarUI.name = "AltarUI";
        }
        else
        {
            Destroy(altarUI);
        }
    }
    
    /// <summary>
    /// Instanciate Forge UI Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowLibrary()
    {
        
        if (libraryUI == null)
        {
            libraryUI = Instantiate(libraryUIPrefab);
            libraryUI.name = "LibraryUI";
            if(hudUI != null)
                ShowHUD();
        }
        else
        {
            Destroy(libraryUI);
            if(hudUI != null)
                ShowHUD();
        }
        
    }

    public void ShowGameOverUI()
    {
        if (gameOverUI == null)
        {
            gameOverUI = Instantiate(gameOverUIPrefab, UIParent.transform);
            gameOverUI.name = "GameOverUI";
            ShowHUD();
            
            EventSystem.current.SetSelectedGameObject(gameOverUI.transform.GetChild(0).GetChild(2).GetChild(0).gameObject);
        }
        else
        {
            Destroy(gameOverUI);
            ShowHUD();
        }
    }

    public void ShowWinUI()
    {
        if (winUI == null)
        {
            winUI = Instantiate(winUIPrefab, UIParent.transform);
            winUI.name = "GameOverUI";
            ShowHUD();
            
            EventSystem.current.SetSelectedGameObject(winUI.transform.GetChild(0).GetChild(2).GetChild(0).gameObject);
        }
        else
        {
            Destroy(winUI);
            ShowHUD();
        }
    }
    
    public void CreateLoadingScreen(int sceneIndex)
    {

        loadingScreenUI = Instantiate(loadingScreenPrefab);
        if (loadingScreenUI.TryGetComponent(out SC_LoadingScene loadingScript))
        {
         
            loadingScript.LoadScene(sceneIndex);
            
        }
        if(hudUI != null) ShowHUD();

    }
    
    public void ShowSettings()
    {

        if (settingsUI == null)
        {
            settingsUI = Instantiate(settingsPrefab);
            settingsUI.name = "SettingsUI";
        }
        else
        {
            Destroy(settingsUI);
        }

    }

    public void DestroyLoadingScreen()
    {
        Destroy(loadingScreenUI);
        loadingScreenUI = null;
    }


    public void ShowStatsDebugUI()
    {
        
        if (statsUI == null)
        {
            statsUI = Instantiate(SC_DebugConsole.instance.StatsUI);
            var statsUIScript = statsUI.GetComponent<SC_StatsDebug>();
            SC_PlayerStats.instance.statsDebug = statsUIScript;
            statsUIScript.LoadStats();
            statsUIScript.InsantiateStats();
        }
        else
        {
            Destroy(statsUI);
        }
        
    }
    
    
    public void ResetTempReferences()
    {
        // UIParent = null;
        
        hudUI = null;
        inventoryUI = null;
        pauseUI = null;
        gameOverUI = null;
        forgeUI = null;
        libraryUI = null;
        settingsUI = null;

        statsUI = null;
    }
    
}
