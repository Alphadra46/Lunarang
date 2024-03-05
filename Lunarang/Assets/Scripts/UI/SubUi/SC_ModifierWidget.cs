using System;
using System.Collections.Generic;
using Enum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class SC_ModifierWidget : MonoBehaviour 
{

    [FoldoutGroup("Settings")]
    [ShowInInspector] private Image iconImg;
    [FoldoutGroup("Settings")]
    [ShowInInspector] private Image arrowImg;

    private void Awake()
    {

        if (!transform.GetChild(1).TryGetComponent(out arrowImg)) return;
        if (!transform.GetChild(0).TryGetComponent(out iconImg)) return;

    }

    public void Init(Sprite iconSpr, Sprite arrowSpr, Color32 color)
    {

        iconImg.sprite = iconSpr;
        arrowImg.sprite = arrowSpr;
        arrowImg.color = color;

    }

}
