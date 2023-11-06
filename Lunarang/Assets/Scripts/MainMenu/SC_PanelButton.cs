using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SC_PanelButton : MonoBehaviour
{
    public GameObject attachedPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SelectPanel()
    {
        attachedPanel.SetActive(true); //Activate the panel attached to this button
        EventSystem.current.SetSelectedGameObject(attachedPanel.GetComponentInChildren<Selectable>().gameObject); //Select the first Selectable Ui element in this panel
        
        if (SC_MainMenuManager.instance.previousSelectedGameObject.Peek() != gameObject)
        {
            //Adding the attached panel to the Stack of previous selected game object
            SC_MainMenuManager.instance.previousSelectedGameObject.Push(gameObject); 
        }
    }
}
