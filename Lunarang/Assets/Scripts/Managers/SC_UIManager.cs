using System;
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
    [SerializeField] private GameObject gameOverUIPrefab;
    [BoxGroup("Prefabs References")]
    [SerializeField] private GameObject forgeUIPrefab;

    #region Temporary References
    
    // Main
    [BoxGroup("Temporary References")]
    [ShowInInspector] private GameObject hudUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] private GameObject inventoryUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] private GameObject pauseUI;
    [BoxGroup("Temporary References")]
    [ShowInInspector] private GameObject gameOverUI;
    
    // Buildings
    [BoxGroup("Temporary References")]
    [ShowInInspector] private GameObject forgeUI;

#endregion

    #endregion

    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
        
        DontDestroyOnLoad(this);

        if (UIParent == null)
        {
            UIParent = GameObject.FindWithTag("UIParent");
        }

        if (SceneManager.GetActiveScene().buildIndex != 2 && SceneManager.GetActiveScene().buildIndex != 3) return;
        
        ResetTempReferences();
        InstantiateHUD();
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
        print(pauseUI);
        if (pauseUI == null)
        {
            pauseUI = Instantiate(pauseUIPrefab, UIParent.transform);
            pauseUI.name = "PauseUI";
            ShowHUD();
            
            EventSystem.current.SetSelectedGameObject(pauseUI.transform.GetChild(1).gameObject);
        }
        else
        {
            Destroy(pauseUI);
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
            forgeUI = Instantiate(forgeUIPrefab, UIParent.transform);
            forgeUI.name = "ForgeUI";
            ShowHUD();
        }
        else
        {
            Destroy(forgeUI);
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
        }
        else
        {
            Destroy(gameOverUI);
            ShowHUD();
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
    }
}
