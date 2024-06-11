using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_ComboAdvancement_UI : MonoBehaviour
{
    [BoxGroup("References")]
    public Image lightBackgroundImage;
    [BoxGroup("References")]
    public Image backgoundImage;
    [BoxGroup("References")]
    public Image insideImage;
    [BoxGroup("References")]
    public Image borderImage;

    [PropertySpace(SpaceBefore = 15f)]
    [ReadOnly, ShowInInspector] private List<GameObject> _parameterTypesGO = new List<GameObject>();

    [PropertySpace(SpaceBefore = 15f)]
    [BoxGroup("Sprites")] public Sprite borderWhenFull;
    [BoxGroup("Sprites")] public Sprite borderWhenNotFull;
    
    private TextMeshProUGUI _tmp;

    private Coroutine _coroutine;
    
    private void OnEnable()
    {
        SC_ComboController.ComboUpdated += UpdateComboTracker;
    }

    private void OnDisable()
    {
        SC_ComboController.ComboUpdated -= UpdateComboTracker;
    }

    private void UpdateComboTracker(int comboCounter, int comboMaxLength, ParameterType attackParameter)
    {
        
        FillInside(comboCounter);
        
        if (comboCounter != comboMaxLength) return;
        
        if(_coroutine != null) StopCoroutine(_coroutine);
        else
        {
            
            _coroutine = StartCoroutine(ResetCounter());

        }

    }

    private void FillInside(int comboCounter)
    {
        if(insideImage == null) return;
        
        insideImage.fillAmount = comboCounter switch
        {
            0 => 0,
            1 => 0.525f,
            2 => 1,
            _ => insideImage.fillAmount
        };
        
        UpdateBorderSprite();
        UpdateLightOpacity();
    }

    private void AddParameterIcon()
    {
        
        
        
    }

    private void ClearParameters()
    {

        foreach (var go in _parameterTypesGO)
        {
            Destroy(go);
        }
        
        _parameterTypesGO.Clear();
        
    }

    private void UpdateBorderSprite()
    {

        borderImage.sprite = insideImage.fillAmount == 1 ? borderWhenFull : borderWhenNotFull;

    }
    
    private void UpdateLightOpacity()
    {

        var color = lightBackgroundImage.color;

        color.a = insideImage.fillAmount;

        lightBackgroundImage.color = color;

    }

    private IEnumerator ResetCounter() //TODO - No reset after a dash that cancel an attack
    {
        yield return new WaitForSeconds(0.75f);
        
        FillInside(0);
        SC_ComboController.instance.ManageComboVFX(SC_ComboController.instance.comboCounterVFX, 0);
        ClearParameters();
        
        _coroutine = null;
    }
}
