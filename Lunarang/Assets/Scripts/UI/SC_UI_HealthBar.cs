using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class SC_UI_HealthBar : MonoBehaviour, IObserver
{
    
    
    #region Variables

    [ShowInInspector, ReadOnly] private float currentHP;
    [ShowInInspector, ReadOnly] private float maxHP;

    [ShowInInspector, ReadOnly] private Slider mainSlider;
    [ShowInInspector, ReadOnly] private Slider anticipationSlider;
    
    [ShowInInspector, ReadOnly] private TextMeshProUGUI tmpHP;

    public float anticipationSpeed;
    private SC_PlayerStats playerStats;

    #endregion

    private void Awake()
    {
        
        if(!transform.GetChild(2).TryGetComponent(out mainSlider)) return;
        
        if(!transform.GetChild(1).TryGetComponent(out anticipationSlider)) return;
        
        if(!transform.GetChild(3).TryGetComponent(out tmpHP)) return;

        playerStats = FindObjectOfType<SC_PlayerStats>();
        AddSelfToSubjectList();
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

    public void OnNotify()
    {
        throw new NotImplementedException();
    }

    public void OnNotify(string context, SC_Subject subjectReference)
    {
        throw new NotImplementedException();
    }

    public void OnNotify(float newCurrentHP, float newMaxHP)
    {
        HealthUpdate(newCurrentHP,newMaxHP);
    }

    /// <summary>
    /// Add this observer to the Subscribe list of the subject
    /// </summary>
    private void AddSelfToSubjectList()
    {
        playerStats.AddObserver(this);
    }
}
