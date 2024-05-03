using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_UI_HealthBar : MonoBehaviour
{
    
    
    #region Variables

    [PropertySpace(SpaceBefore = 15f)]
    [ShowInInspector, ReadOnly] private float currentHP;
    [ShowInInspector, ReadOnly] private float maxHP;

    [PropertySpace(SpaceBefore = 15f)]
    [ShowInInspector, ReadOnly] private Slider mainSlider;
    [ShowInInspector, ReadOnly] private Slider anticipationSlider;
    [ShowInInspector, ReadOnly] private Slider shieldSlider;
    
    [PropertySpace(SpaceBefore = 15f)]
    [ShowInInspector, ReadOnly] private Image mainBorder;
    [ShowInInspector, ReadOnly] private Image anticipationBorder;
    
    [PropertySpace(SpaceBefore = 15f)]
    [ShowInInspector, ReadOnly] private TextMeshProUGUI tmpHP;
    
    public float anticipationSpeed;
    public float anticipationDuration;
    public float lerpDelay;

    private float timer;

    [ShowInInspector] private Coroutine lerpCoroutine;
    [ShowInInspector] private bool canLerp = false;
    private float actualAnticipationValue;
    private float actualMainValue;

    #endregion

    private void Awake()
    {
        SC_PlayerStats.onHealthInit += HealthInit;
        SC_PlayerStats.onHealthChange += HealthUpdate;
        SC_PlayerStats.onShieldHPChange += ShieldUpdate;
        
        if(!transform.GetChild(3).TryGetComponent(out mainSlider)) return;
        if(mainSlider.handleRect.TryGetComponent(out mainBorder)) mainSlider.onValueChanged.AddListener(OnHealthValueChanged);
        
        if(!transform.GetChild(2).TryGetComponent(out anticipationSlider)) return;
        if(anticipationSlider.handleRect.TryGetComponent(out anticipationBorder)) anticipationSlider.onValueChanged.AddListener(OnAnticipationValueChanged);
        
        if(!transform.GetChild(1).TryGetComponent(out shieldSlider)) return;
        
        if(!transform.GetChild(5).TryGetComponent(out tmpHP)) return;
    }

    private void OnHealthValueChanged(float value)
    {
        
        mainBorder.color = value == maxHP ? new Color32(255, 255, 255, 0) : new Color32(255, 255, 255, 255);

    }
    
    private void OnAnticipationValueChanged(float value)
    {
        
        anticipationBorder.color = value == maxHP ? new Color32(255, 255, 255, 0) : new Color32(255, 255, 255, 255);

    }

    private void FixedUpdate()
    {
        if (mainSlider.value == anticipationSlider.value)
        {
            canLerp = false;
            lerpCoroutine = null;
        }
        else
        {
            if(canLerp) 
            {
                if(timer > 0f) {
                    anticipationSlider.value = Mathf.Lerp(anticipationSlider.value, mainSlider.value, anticipationSpeed);
                    timer -= Time.fixedDeltaTime;
                }
                else
                {
                    anticipationSlider.value = mainSlider.value;
                    lerpCoroutine = null;
                }
            }

            if(lerpCoroutine == null) lerpCoroutine = StartCoroutine(LerpAnticipationSlider());
        }

    }

    private IEnumerator LerpAnticipationSlider()
    {
        
        print("Coroutine");
        canLerp = false;
        yield return new WaitForSeconds(lerpDelay);
        canLerp = true;

        timer = anticipationDuration;
        
        actualAnticipationValue = anticipationSlider.value;
        actualMainValue = mainSlider.value;
    }
    
    public void HealthInit(float newCurrentHP, float newMaxHP)
    {
        currentHP = newCurrentHP;
        maxHP = newMaxHP;

        mainSlider.maxValue = maxHP;
        mainSlider.value = currentHP;
        
        anticipationSlider.maxValue = maxHP;
        anticipationSlider.value = currentHP;

        tmpHP.text = $"{currentHP} / {maxHP}";
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
    
    public void ShieldUpdate(float newCurrentShieldValue, float newCurrentShieldMaxValue)
    {
        if(shieldSlider == null) return;
        
        shieldSlider.gameObject.SetActive(newCurrentShieldMaxValue > 0);
        
        shieldSlider.maxValue = newCurrentShieldMaxValue;
        shieldSlider.value = newCurrentShieldValue;
    }
    
}
