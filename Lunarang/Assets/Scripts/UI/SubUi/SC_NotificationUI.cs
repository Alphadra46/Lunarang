using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_NotificationUI : MonoBehaviour
{

    public TextMeshProUGUI promptTMP;
    public Image image;

    public void Init(Sprite icon, string promptText)
    {

        image.sprite = icon;
        promptTMP.text = promptText;

    }

}
