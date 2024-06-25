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
        Init("Keyboard");
    }

    public void Init(string controllerName)
    {
        var controller = controllerName switch
        {
            not null when controllerName.Contains("SwitchProControllerHID") => promptSwitchImages,
            not null when controllerName.Contains("XInputController") => promptXboxImages,
            not null when controllerName.Contains("DualShock4GamepadHID") => promptPlaystationImages,
            not null when controllerName.Contains("DualSenseGamepadHID") => promptPlaystationImages,
            not null when controllerName.Contains("Keyboard") => promptKeyboardImages,
            _ => promptKeyboardImages
        };
        
        if (controller.Length < 2)
        {
            
            // print(controllerName);
            
            foreach (var image in images)
            {
                if(image != null)
                    image.gameObject.SetActive(false);
            }

            if (images[0] == null) return;
            
            images[0].gameObject.SetActive(true);

            images[0].sprite = controller[0];

        }
        else
        {
            foreach (var image in images)
            {
                if(image != null)
                    image.gameObject.SetActive(true);
            }
            
            for (var i = 0; i < images.Length; i++)
            {
                if (images[i] != null)
                    images[i].sprite = controller[i];
            }
        }
        
    }

    public void SetText(string value)
    {
        tmp.text = value;
    }

    public void SetInteractable(bool value)
    {
        // print("Test");
        
        tmp.color = value ? Color.white : Color.grey;
        
    }
    
}
