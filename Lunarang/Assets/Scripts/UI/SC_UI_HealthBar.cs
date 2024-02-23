using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_HealthBar : MonoBehaviour
{
    
    
    #region Variables

    [ShowInInspector, ReadOnly] private float currentHP;
    [ShowInInspector, ReadOnly] private float maxHP;

    [ShowInInspector, ReadOnly] private Slider mainSlider;
    [ShowInInspector, ReadOnly] private Slider shieldSlider;
    [ShowInInspector, ReadOnly] private Image detail;
    
    [ShowInInspector, ReadOnly] private TextMeshProUGUI tmpHP;

    #endregion

    private void Awake()
    {

        SC_PlayerStats.onHealthChange += HealthUpdate;
        SC_PlayerStats.onShieldHPChange += ShieldUpdate;
        
        if(!transform.GetChild(2).TryGetComponent(out mainSlider)) return;
        if(mainSlider.handleRect.TryGetComponent(out detail)) mainSlider.onValueChanged.AddListener(OnHealthValueChanged);
        
        if(!transform.GetChild(1).TryGetComponent(out shieldSlider)) return;
        
        if(!transform.GetChild(4).TryGetComponent(out tmpHP)) return;
    }

    private void OnHealthValueChanged(float value)
    {
    
        print(value);
        detail.color = value == maxHP ? new Color32(255, 255, 255, 0) : new Color32(255, 255, 255, 255);

    }

    private void Update()
    {
        
        // if (mainSlider.value != anticipationSlider.value)
        // {
        //     anticipationSlider.value = Mathf.Lerp(anticipationSlider.value, currentHP, anticipationSpeed);
        // }
        
    }

    public void HealthUpdate(float newCurrentHP, float newMaxHP)
    {
        currentHP = newCurrentHP;
        maxHP = newMaxHP;

        mainSlider.maxValue = maxHP;
        mainSlider.value = currentHP;

        tmpHP.text = $"{currentHP} / {maxHP}";
    }
    
    public void ShieldUpdate(float newCurrentShieldValue, float newCurrentShieldMaxValue)
    {
        if(shieldSlider == null) return;
        
        shieldSlider.gameObject.SetActive(newCurrentShieldMaxValue > 0);
        
        shieldSlider.maxValue = newCurrentShieldValue;
        shieldSlider.value = newCurrentShieldMaxValue;
    }
    
}
