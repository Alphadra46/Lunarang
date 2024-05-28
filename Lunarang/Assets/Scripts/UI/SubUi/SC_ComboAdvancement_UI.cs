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

    private void Start()
    {
        if(!transform.GetChild(0).TryGetComponent(out _tmp))return;

        SC_ComboController.ComboUpdated += SetText;
    }


    private void LateUpdate()
    {
        transform.rotation = rot;
    }

    private void SetText(int comboCounter, int comboMaxLength, ParameterType attackParameter)
    {
        if(_tmp == null) return;
        
        _tmp.text = comboCounter + "/" + comboMaxLength;

        if (comboCounter != comboMaxLength) return;
        
        if(_coroutine != null) StopCoroutine(_coroutine);
        else
        {

            _coroutine = StartCoroutine(ResetCounter(comboMaxLength));

        }

    }

    private IEnumerator ResetCounter(int comboMaxLength) //TODO - No reset after a dash that cancel an attack
    {
        
        yield return new WaitForSeconds(0.75f);
        if (_tmp != null)
        {
            _tmp.text = 0 + "/" + comboMaxLength;
            
        }
        
        _coroutine = null;
    }
}
