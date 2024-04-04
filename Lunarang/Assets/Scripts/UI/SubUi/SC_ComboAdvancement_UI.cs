using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SC_ComboAdvancement_UI : MonoBehaviour
{
    private TextMeshProUGUI _tmp;

    private Quaternion rot;
    
    private void Awake()
    {
        rot = transform.rotation;
        
        if(!transform.GetChild(0).TryGetComponent(out _tmp))return;

        SC_ComboController.ComboUpdated += SetText;
    }

    private void LateUpdate()
    {
        transform.rotation = rot;
    }

    private void SetText(int comboCounter, int comboMaxLength, ParameterType attackParameter)
    {

        _tmp.text = comboCounter + "/" + comboMaxLength;

    }
}
