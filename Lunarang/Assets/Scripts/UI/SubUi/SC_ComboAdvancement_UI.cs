using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_ComboAdvancement_UI : MonoBehaviour
{
    private TextMeshProUGUI _tmp;

    private Quaternion rot;

    private Coroutine _coroutine;
    
    private void Awake()
    {
        rot = transform.rotation;
    }

    private void OnEnable()
    {
        SC_ComboController.ComboUpdated += Reset;
    }

    private void OnDisable()
    {
        SC_ComboController.ComboUpdated -= Reset;
    }


    private void LateUpdate()
    {
        transform.rotation = rot;
    }

    private void Reset(int comboCounter, int comboMaxLength, ParameterType attackParameter)
    {
        
        if (comboCounter != comboMaxLength) return;
        
        if(_coroutine != null) StopCoroutine(_coroutine);
        else
        {
            
            _coroutine = StartCoroutine(ResetCounter());

        }

    }

    private IEnumerator ResetCounter() //TODO - No reset after a dash that cancel an attack
    {
        yield return new WaitForSeconds(0.75f);
        
        SC_ComboController.instance.ManageComboVFX(SC_ComboController.instance.comboCounterVFX, 0);
        _coroutine = null;
    }
}
