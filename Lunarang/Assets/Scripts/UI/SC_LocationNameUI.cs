using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class SC_LocationNameUI : MonoBehaviour
{

    private string _locName;
    [SerializeField] private TextMeshProUGUI tmp;
    
    [ShowInInspector] public string locName
    {
        get => _locName;
        set
        {
            _locName = value;
            SetText(value);
        }
    }

    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void SetText(string value)
    {
        
        tmp.text = _locName;

    }

}
