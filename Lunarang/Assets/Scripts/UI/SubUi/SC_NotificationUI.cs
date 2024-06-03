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

    [PropertySpace(SpaceBefore = 5f)] private const float delay = 2.5f;

    private Coroutine destroyCoroutine;
    private int amount;

    private Animator _animator;

    private void Awake()
    {
        if(!TryGetComponent(out _animator)) return;
    }

    public void Init(Sprite icon, string promptText)
    {

        image.sprite = icon;
        promptTMP.text = promptText;

        destroyCoroutine = StartCoroutine(DestroyCoroutine());

    }
    
    public void Init(SC_Resource resource, int newAmount)
    {

        var finalPrompt = $"{resource.name} x{newAmount}";
        
        image.sprite = resource.sprite;
        promptTMP.text = finalPrompt;
        amount = newAmount;

        destroyCoroutine = StartCoroutine(DestroyCoroutine());

    }

    public void UpdateNotification(SC_Resource resource, int newAmount)
    {

        if(destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
        
        _animator.SetTrigger("Update");
        
        var finalPrompt = $"{resource.name} x{amount+newAmount}";
        
        promptTMP.text = finalPrompt;
        amount += newAmount;
        
        destroyCoroutine = StartCoroutine(DestroyCoroutine());

    }

    public void Destroy()
    {
        
        _animator.SetTrigger("Destroy");
        
        Destroy(gameObject, 1f);
        
    }

    private IEnumerator DestroyCoroutine()
    {
        
        yield return new WaitForSeconds(delay);
        SC_NotificationManager.removeNotification?.Invoke(this);
        destroyCoroutine = null;

    }
}
