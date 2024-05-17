using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SC_NotificationUI : MonoBehaviour
{

    public TextMeshProUGUI promptTMP;
    public Image image;

    [PropertySpace(SpaceBefore = 5f)] public float delay = 2.5f;

    private Coroutine destroyCoroutine;
    
    public void Init(Sprite icon, string promptText)
    {

        image.sprite = icon;
        promptTMP.text = promptText;

        destroyCoroutine = StartCoroutine(DestroyCoroutine());

    }

    public void UpdateNotification(string promptText)
    {

        if(destroyCoroutine != null) StopCoroutine(destroyCoroutine);
        
        promptTMP.text = promptText;
        
        destroyCoroutine = StartCoroutine(DestroyCoroutine());

    }

    private IEnumerator DestroyCoroutine()
    {
        
        yield return new WaitForSeconds(delay);
        SC_NotificationManager.removeNotification?.Invoke(gameObject);

    }
}
