using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MainMenuManager : MonoBehaviour
{

    public static SC_MainMenuManager instance;


    [Header("Menus")]
    public GameObject mainMenu; //The main menu panel
    [Space(3f)]
    public GameObject settingsMenu; //The settings panel

    //Used for the back button. Chose to use a Stack (FILO) so that the first selection the user make will be the last one when we use the back button.
    public Stack<GameObject> previousSelectedGameObject = new Stack<GameObject>(); 
    
    
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
