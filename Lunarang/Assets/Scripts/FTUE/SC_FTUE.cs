using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class SC_FTUE : MonoBehaviour
{
    [Title("UI Panels")]
    public GameObject uiMissionLetter;
    public GameObject uiCombos;
    public GameObject uiRessources;
    public GameObject uiUpgrades;

    private int ressourcesNumber = 0;
    private int roomNumber = 1;
    private void Start()
    {
        roomNumber = 1;
        ressourcesNumber = 0;
        LockTravel();
        LockWeapons();
        uiMissionLetter.SetActive(true);
    }

    #region Setup
    private void LockTravel()
    {
        //lock the possibility to move or dash
    }

    private void LockWeapons()
    {
        //lock the possibility to use the weapons
    }
    
    #endregion Setup

    #region FirstRoom

    /// <summary>
    /// Called when the mission UI panel in the first FTUE room is closed
    /// Unlock the travel key and open the door to the second ftue room
    /// </summary>
    public void OnMissionUIClosed()
    {
        UnlockTravel();
        OpenDoors();
    }

    private void UnlockTravel()
    {
        //make the travel and dash possible again
    }
    #endregion FirstRoom

    #region SecondRoom

    /// <summary>
    /// Called when the warrior start to chase the player in the second FTUE room
    /// </summary>
    public void OpenComboUI()
    {
        uiCombos.SetActive(true);
    }

    /// <summary>
    /// Called when the combos UI panel in the second FTUE room is closed
    /// </summary>
    public void OnCombosUIClosed()
    {
        UnlockWepons();
        NoDamageTaken();
    }

    private void UnlockWepons()
    {
        //make the attack possible again
    }

    private void NoDamageTaken()
    {
        //block any possibility of receiving damage
    }

    /// <summary>
    /// Called when the warrior in the second FTUE room is defeated
    /// </summary>
    public void OnWarriorDefeated()
    {
        uiRessources.SetActive(true);

        //drop 15 sylvanorites on the ground
    }

    public void OnUpgradesUIClosed()
    {
        OpenDoors();
    }

    #endregion SecondRoom

    /// <summary>
    /// Called when the player gather ressources
    /// </summary>
    public void OnRessourcesGathered()
    {
        if(ressourcesNumber == 15)
        {
            //ressources on the second room
            uiUpgrades.SetActive(true);
            return;
        }

        if(ressourcesNumber == 26)
        {
            //ressources on the third room
            return;
        }
    }

    #region ChangeRoom

    public void OpenDoors()
    {
        roomNumber++;

        if(roomNumber == 2)
        {
            //open the door for the second ftue room
            return;
        }

        if(roomNumber == 3)
        {
            //open the door for the third ftue room
            return;
        }
    }
    public void NextRoom()
    {
        //launch transition panel

        if(roomNumber == 2)
        {
            //tp the player to the second ftue room
            return;
        }

        if(roomNumber == 3)
        {
            //tp the player to the third ftue room
            return;
        }
    }

    #endregion ChangeRoom
}
