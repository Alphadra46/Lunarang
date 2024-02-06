using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SC_InputPrompt : MonoBehaviour
{

    public Image[] images;
    public TextMeshProUGUI tmp;

    private string prompt;
    
    [NotNull]
    [ShowInInspector]
    public string promptText
    {
        get => prompt;
        set
        {
            prompt = value;
            SetText(value);
        }
    }
    public Sprite[] promptSwitchImages;
    public Sprite[] promptXboxImages;
    public Sprite[] promptPlaystationImages;
    public Sprite[] promptKeyboardImages;

    private void Awake()
    {
        SC_InputManager.newControllerUsed += Init;
    }

    public void Init(string controllerName)
    {
        for (var i = 0; i < images.Length; i++)
        {
            if(images[i] != null)
                images[i].sprite = controllerName switch
                {
                    "SwitchProControllerHID" => promptSwitchImages[i],
                    "Keyboard" => promptKeyboardImages[i],
                    _ => promptKeyboardImages[i]
                };
        }
    }

    public void SetText(string value)
    {
        tmp.text = value;
    }
    
}
