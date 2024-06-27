using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class SC_Lobby : MonoBehaviour
{

    public static SC_Lobby instance;

    public static Action<SC_BuildingButton, bool> currentBuilding;
    public Action<SC_BuildingButton> upgradeFB;
    public Action<SC_BuildingButton> interactFB;
    public SO_Archive archiveTutorial;
    public GameObject lobbyUI;
    public GameObject lobbyTutorialUIPrefab;

    private GameObject lobbyTutorialUI;
    private Selectable lastSelected;
    public SC_BuildingButton buildingSelected;

    private void OnEnable()
    {
        if (SC_InputManager.instance == null)
        {
            StartCoroutine(RetryInOneFrame());
            return;
        }

        if (SC_GameManager.isTutorialFinished)
        {
            SC_InputManager.instance.develop.started += UpgradeBuilding;
            SC_InputManager.instance.submit.started += InteractBuilding;
        }
        else
        {
            StartCoroutine(DelayTutorial());
        }
        
    }


    
    private IEnumerator DelayTutorial()
    {
        yield return new WaitForSeconds(0.1f);
        SC_GameManager.instance.SetPause();
        lobbyTutorialUI = Instantiate(lobbyTutorialUIPrefab);
        lobbyTutorialUI.GetComponent<SC_ArchiveDiscoveredUI>().Init(archiveTutorial);
        yield return new WaitForSecondsRealtime(1f);
        SC_InputManager.instance.cancel.started += QuitTutorial;
    }

    private void QuitTutorial(InputAction.CallbackContext context)
    {
        SC_InputManager.instance.cancel.started -= QuitTutorial;
        Destroy(lobbyTutorialUI);
        SC_GameManager.instance.SetPause();
        SC_GameManager.isTutorialFinished = true;
        
        SC_InputManager.instance.develop.started += UpgradeBuilding;
        SC_InputManager.instance.submit.started += InteractBuilding;
    }
    
    
    private IEnumerator RetryInOneFrame()
    {

        yield return new WaitForEndOfFrame();
        
        SC_InputManager.instance.develop.started += UpgradeBuilding;
        SC_InputManager.instance.submit.started += InteractBuilding;

    }
    
    private void OnDisable()
    {
        
        SC_InputManager.instance.develop.started -= UpgradeBuilding;
        SC_InputManager.instance.submit.started -= InteractBuilding;
        
    }

    private void Awake()
    {
        
        if(instance != null) Destroy(this);

        instance = this;
        
        currentBuilding += SelectBuilding;
        

    }

    private void SelectBuilding(SC_BuildingButton buildingButton, bool value)
    {
        
        buildingSelected = value ? buildingButton : null;
        
    }

    private void UpgradeBuilding(InputAction.CallbackContext ctx)
    {
        if(buildingSelected == null) return;
        
        if(!buildingSelected.interactable) return;
        
        if(!buildingSelected.building.levelUpCosts.ContainsKey(buildingSelected.building.currentLevel+1)) return;
        
        if(!SC_GameManager.instance.playerResourceInventory.CheckHasResources(buildingSelected.building.levelUpCosts[buildingSelected.building.currentLevel+1])) return;
        
        buildingSelected.building.Upgrade();
        buildingSelected.UpdateSprite();
        buildingSelected.UpdateUpgradeCosts();

    }

    private void InteractBuilding(InputAction.CallbackContext ctx)
    {

        if(buildingSelected == null) return;
        
        if(!buildingSelected.interactable) return;
        
        switch (buildingSelected.building.buildingName)
        {
            case "library":
                Library(buildingSelected);
                break;
            case "forge":
                Forge(buildingSelected);
                break;
            case "altar":
                Altar(buildingSelected);
                break;
            case "restaurant":
                break;
            case "merchant":
                break;
        }
        
        SC_InputManager.instance.develop.started -= UpgradeBuilding;
        SC_InputManager.instance.submit.started -= InteractBuilding;
        
    }

    public void ShowLobby()
    {
        
        lobbyUI.SetActive(true);
        lastSelected.Select();
        
        SC_InputManager.instance.develop.started += UpgradeBuilding;
        SC_InputManager.instance.submit.started += InteractBuilding;
        
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
        lobbyUI.SetActive(false);
        
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
