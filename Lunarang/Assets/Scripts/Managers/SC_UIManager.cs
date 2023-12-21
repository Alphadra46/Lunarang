using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SC_UIManager : MonoBehaviour
{
    public static SC_UIManager instance;

    [SerializeField] private GameObject inventoryUIPrefab;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject pauseUIPrefab;
    
    [SerializeField] private GameObject forgeUIPrefab;

    #region Temporary References
    
    // Main
    private GameObject inventoryUI;
    private GameObject pauseUI;
    
    // Buildings
    private GameObject forgeUI;

    #endregion

    private void Awake()
    {
        if(instance != null) Destroy(this);
        instance = this;
    }

    /// <summary>
    /// Instanciate Inventory Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowInventory()
    {
        
        if (inventoryUI == null)
        {
            inventoryUI = Instantiate(inventoryUIPrefab);
            HUD.SetActive(false);
        }
        else
        {
            Destroy(inventoryUI);
            HUD.SetActive(true);
        }
        
    }
    
    /// <summary>
    /// Instanciate Pause Menu Prefab, disabling the render of the HUD.
    /// </summary>
    public void ShowPauseMenu()
    {
        
        if (pauseUI == null)
        {
            pauseUI = Instantiate(pauseUIPrefab);
            HUD.SetActive(false);
            
            EventSystem.current.SetSelectedGameObject(pauseUI.transform.GetChild(1).gameObject);
        }
        else
        {
            Destroy(pauseUI.gameObject);
            HUD.SetActive(true);
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
            HUD.SetActive(false);
        }
        else
        {
            Destroy(forgeUI);
            HUD.SetActive(true);
        }
        
    }
    
}
