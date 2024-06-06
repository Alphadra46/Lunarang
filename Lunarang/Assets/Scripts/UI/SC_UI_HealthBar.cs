using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;

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

    [PropertySpace(SpaceBefore = 15f)]
    [SerializeField] private float scaleOnDamage = 1.15f;
    [SerializeField] private float punchAngle = 5;
    [SerializeField] private float transition = 0.15f;
    [SerializeField] private float scaleTransition = 0.15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    
    [PropertySpace(SpaceBefore = 15f)]
    [SerializeField] private Material lowLifeMaterial;
    [SerializeField] private float feTransitionDuration = 0.5f;


    [PropertySpace(SpaceBefore = 15f)]
    public float anticipationSpeed;
    public float anticipationDuration;
    public float lerpDelay;

    private float timer;

    [ShowInInspector] private Coroutine lerpCoroutine;
    [ShowInInspector] private bool canLerp = false;
    private float actualAnticipationValue;
    private float actualMainValue;

    [ShowInInspector] private Coroutine feEnableCoroutine;
    [ShowInInspector] private Coroutine feDisableCoroutine;

    private bool isInit;

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
        
        if(!transform.GetChild(5).GetChild(0).TryGetComponent(out tmpHP)) return;

        lerpCoroutine = null;
    }

    private void OnHealthValueChanged(float value)
    {
        if(!isInit) return;
        
        mainBorder.color = value == maxHP ? new Color32(255, 255, 255, 0) : new Color32(255, 255, 255, 255);
        transform.DOScale(scaleOnDamage, scaleTransition).SetEase(scaleEase);
        DOTween.Kill(2, true);
        transform.DOPunchRotation(Vector3.forward * punchAngle, transition, 20, 1).SetId(2);

        if (feDisableCoroutine == null && currentHP >= maxHP/3)
        {
            if (feEnableCoroutine != null)
                StopCoroutine(feEnableCoroutine);
            
            feDisableCoroutine = StartCoroutine(FEDisable());
        }

        if (feEnableCoroutine == null && currentHP < maxHP/3) 
        {
            if (feDisableCoroutine != null)
                StopCoroutine(feDisableCoroutine);
            
            feEnableCoroutine = StartCoroutine(FEEnable());
        }
    }

    private IEnumerator FEEnable()
    {
        feDisableCoroutine = null;
        float timer = feTransitionDuration;
        float baseIntensity = lowLifeMaterial.GetFloat("_VignetteIntensity");
        while (timer>0)
        {
            lowLifeMaterial.SetFloat("_VignetteIntensity", Mathf.Lerp(baseIntensity,1.45f,1-(timer/feTransitionDuration)));
            yield return null;
            timer -= Time.deltaTime;
        }

        feEnableCoroutine = null;
        yield return null;
    }

    private IEnumerator FEDisable()
    {
        feEnableCoroutine = null;
        float timer = feTransitionDuration;
        float baseIntensity = lowLifeMaterial.GetFloat("_VignetteIntensity");
        while (timer>0)
        {
            lowLifeMaterial.SetFloat("_VignetteIntensity", Mathf.Lerp(baseIntensity,0f,1-(timer/feTransitionDuration)));
            yield return null;
            timer -= Time.deltaTime;
        }

        feDisableCoroutine = null;
        yield return null;
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

            lerpCoroutine ??= StartCoroutine(LerpAnticipationSlider());
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

        isInit = true;
    }

    public void HealthUpdate(float newCurrentHP, float newMaxHP)
    {
        currentHP = newCurrentHP;
        maxHP = newMaxHP;

        mainSlider.maxValue = maxHP;
        mainSlider.value = currentHP;
        
        anticipationSlider.maxValue = maxHP;

        tmpHP.text = $"{currentHP} / {maxHP}";
        
        lerpCoroutine = null;
    }
    
    public void ShieldUpdate(float newCurrentShieldValue, float newCurrentShieldMaxValue)
    {
        if(shieldSlider == null) return;
        
        shieldSlider.gameObject.SetActive(newCurrentShieldMaxValue > 0);
        
        shieldSlider.maxValue = newCurrentShieldMaxValue;
        shieldSlider.value = newCurrentShieldValue;
    }
    
}
