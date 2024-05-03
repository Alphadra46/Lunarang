using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_BackButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Select the previous UI element that have been activated
    /// </summary>
    public void SelectPreviousUI()
    {
        var lastUI = SC_MainMenuManager.instance.previousSelectedGameObject.Pop();//Removing the last selected UI from the stack 
        
        //Selectable class can be replaced by Button class
        if (lastUI.TryGetComponent(out Selectable c)) //Check if the last UI selected is inheriting from the Selectable class
        {
            EventSystem.current.SetSelectedGameObject(lastUI); //and make it be the selected game object
            
            if (lastUI.TryGetComponent(out SC_PanelButton panelButton)) //If the last UI element has a SC_PanelButton script attached to it
            {
                panelButton.attachedPanel.SetActive(false); //then de activate the attached panel
            }
        }
        else
        {
            if (lastUI == SC_MainMenuManager.instance.mainMenu) //Check if the last UI is the main menu
            {
                SC_MainMenuManager.instance.mainMenu.SetActive(true); // If it's true, activate the main menu panel
            }
            EventSystem.current.SetSelectedGameObject(lastUI.transform.GetComponentInChildren<Button>().gameObject);
        }
    }
}
