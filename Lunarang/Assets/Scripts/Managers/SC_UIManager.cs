using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SC_UIManager : MonoBehaviour
{
    public static SC_UIManager instance;

    [SerializeField] private GameObject inventoryUIPrefab;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject pauseUIPrefab;
    [SerializeField] private GameObject gameOverUIPrefab;
    
    [SerializeField] private GameObject forgeUIPrefab;

    #region Temporary References
    
    // Main
    [ShowInInspector] private GameObject inventoryUI;
    [ShowInInspector] private GameObject pauseUI;
    [ShowInInspector] private GameObject gameOverUI;
    
    // Buildings
    [ShowInInspector] private GameObject forgeUI;

    #endregion

    private void Awake()
    {
        if(instance != null) Destroy(this.gameObject);
        instance = this;
        
        DontDestroyOnLoad(this.gameObject);
    }

    public void ShowHUD()
    {
        HUD.SetActive(!HUD.activeInHierarchy);
    }

    /// <summary>
    /// Instanciate Inventory Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowInventory()
    {
        
        if (inventoryUI == null)
        {
            inventoryUI = Instantiate(inventoryUIPrefab);
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
            pauseUI = Instantiate(pauseUIPrefab);
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
            forgeUI = Instantiate(forgeUIPrefab);
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
            gameOverUI = Instantiate(gameOverUIPrefab);
            ShowHUD();
        }
        else
        {
            Destroy(gameOverUI);
            ShowHUD();
        }
    }
    
}
