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
    [BoxGroup("References")]
    public Image firstPropertyImage;
    [BoxGroup("References")]
    public Image secondPropertyImage;
    

    [PropertySpace(SpaceBefore = 15f)]
    [ReadOnly, ShowInInspector] private List<Image> _parameterTypesGO = new List<Image>();

    [PropertySpace(SpaceBefore = 15f)]
    [BoxGroup("Sprites")] public Sprite borderWhenFull;
    [BoxGroup("Sprites")] public Sprite borderWhenNotFull;
    [BoxGroup("Sprites")] public Sprite multihitIcon;
    [BoxGroup("Sprites")] public Sprite projectileIcon;
    [BoxGroup("Sprites")] public Sprite aoeIcon;
    
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
        
        if (comboCounter != comboMaxLength)
        {
            AddParameterIcon(attackParameter);
            return;
        }
        
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

    private void AddParameterIcon(ParameterType parameterType)
    {
        
        var sprite = parameterType switch
        {
            ParameterType.MultiHit => multihitIcon,
            ParameterType.AreaOfEffect => aoeIcon,
            ParameterType.Projectile => projectileIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(parameterType), parameterType, null)
        };

        var image = _parameterTypesGO.Count switch
        {
            0 => firstPropertyImage,
            1 => secondPropertyImage,
            _ => firstPropertyImage
        };

        if(image == null) return;
        
        image.sprite = sprite;
        
        var imageColor = image.color;
        imageColor.a = 1f;

        image.color = imageColor;
        
        _parameterTypesGO.Add(image);

    }

    private void ClearParameters()
    {

        foreach (var image in _parameterTypesGO)
        {
            image.sprite = null;
            
            var imageColor = image.color;
            imageColor.a = 0;

            image.color = imageColor;
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
