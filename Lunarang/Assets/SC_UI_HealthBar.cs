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
    [ShowInInspector, ReadOnly] private Slider anticipationSlider;
    
    [ShowInInspector, ReadOnly] private TextMeshProUGUI tmpHP;

    public float anticipationSpeed;

    #endregion

    private void Awake()
    {
        
        if(!transform.GetChild(2).TryGetComponent(out mainSlider)) return;
        
        if(!transform.GetChild(1).TryGetComponent(out anticipationSlider)) return;
        
        if(!transform.GetChild(3).TryGetComponent(out tmpHP)) return;
        
    }

    private void Update()
    {
        
        if (mainSlider.value != anticipationSlider.value)
        {
            anticipationSlider.value = Mathf.Lerp(anticipationSlider.value, currentHP, anticipationSpeed);
        }
        
    }

    public void HealthUpdate(float newCurrentHP, float newMaxHP)
    {
        currentHP = newCurrentHP;
        maxHP = newMaxHP;

        mainSlider.maxValue = maxHP;
        mainSlider.value = currentHP;
        
        anticipationSlider.maxValue = maxHP;

        tmpHP.text = $"{currentHP} / {maxHP}";

    }
    
    
}
