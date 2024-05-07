using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class SC_SkillButton : MonoBehaviour
{
    [HideInInspector] public GameObject tooltip;

    [SerializeField] private Sprite unknownSkill;
    [SerializeField] private Sprite unusableSkill;
    [SerializeField] private Sprite usableSkill;
    [SerializeField] private Sprite equippedSkill;

    private Image imageSlot;

    private bool isKnown; //TODO - Move it on SO_BaseSkill
    private bool isUsable;
    
    private void Start()
    {
        tooltip = transform.GetChild(0).gameObject;
        imageSlot = GetComponent<Image>();
        SC_InputManager.instance.develop.started += ToggleTooltip;
    }

    public void InitTooltip(SO_BaseSkill skill)
    {
        var nameText = tooltip.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        var descText = tooltip.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        var costText = tooltip.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>();

        nameText.text = skill.skillName;
        descText.text = skill.shortDescription;
        //costText.text = skill.SPCost;
    }

    private void ToggleTooltip(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
            return;
        
        if (tooltip.activeSelf)
            HideTooltip();
        else
            ShowTooltip();
    } 
    
    public void ShowTooltip()
    {
        tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
    
}
