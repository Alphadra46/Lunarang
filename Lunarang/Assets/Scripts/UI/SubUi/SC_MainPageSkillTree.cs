using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SC_MainPageSkillTree : MonoBehaviour
{
    [SerializeField] private List<SC_Constellation> constellations = new List<SC_Constellation>();

    private TextMeshProUGUI descText;
    
    private int currentSelectedConstellationIndex;

    private List<GameObject> constellationSlot = new List<GameObject>();

    private void Start()
    {
        descText = transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        descText.text = constellations[0].name;
        
        var container = transform.GetChild(2);
        for (int i = 0; i < container.childCount; i++)
        {
            constellationSlot.Add(container.GetChild(i).gameObject);
        }

        foreach (var slot in constellationSlot)
        {
            slot.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                constellations[constellationSlot.IndexOf(slot)].name;
        }
    }
    
    private void OnEnable()
    {
        StartCoroutine(DelayedEnable());
    }

    private IEnumerator DelayedEnable()
    {
        yield return null;
        yield return null;
        currentSelectedConstellationIndex = 0;
        EventSystem.current.SetSelectedGameObject(constellationSlot[currentSelectedConstellationIndex].transform.GetChild(1).GetChild(0).gameObject);
        SC_InputManager.instance.switchToLeft.started += SwitchToLeft;
        SC_InputManager.instance.switchToRight.started += SwitchToRight;
        SC_InputManager.instance.cancel.started += CloseAltarUI;
    }
    
    private void OnDisable()
    {
        SC_InputManager.instance.switchToLeft.started -= SwitchToLeft;
        SC_InputManager.instance.switchToRight.started -= SwitchToRight;
        SC_InputManager.instance.cancel.started -= CloseAltarUI;
    }
    
    private void SwitchToLeft(InputAction.CallbackContext context)
    {
        
        currentSelectedConstellationIndex--;
        if (currentSelectedConstellationIndex < 0)
            currentSelectedConstellationIndex = transform.GetChild(2).childCount-1;

        EventSystem.current.SetSelectedGameObject(constellationSlot[currentSelectedConstellationIndex].transform.GetChild(1).GetChild(0).gameObject);
        descText.text = constellations[currentSelectedConstellationIndex].name;
    }

    private void SwitchToRight(InputAction.CallbackContext context)
    {
        currentSelectedConstellationIndex++;
        if (currentSelectedConstellationIndex > transform.GetChild(2).childCount - 1)
            currentSelectedConstellationIndex = 0;

        EventSystem.current.SetSelectedGameObject(constellationSlot[currentSelectedConstellationIndex].transform.GetChild(1).GetChild(0).gameObject);
        descText.text = constellations[currentSelectedConstellationIndex].name;
    }

    public void SelectPage(GameObject constellationPage)
    {
        constellationPage.SetActive(true);
        gameObject.SetActive(false);
    }

    private void CloseAltarUI(InputAction.CallbackContext context)
    {
        OnDisable();
        SC_Lobby.instance.ShowLobby();
        Destroy(SC_SkillTreeUI.instance.gameObject);
    }
}
