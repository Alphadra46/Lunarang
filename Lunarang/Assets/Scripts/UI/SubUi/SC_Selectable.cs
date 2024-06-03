using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class SC_Selectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Delay")] 
    [SerializeField] private float selectionDelay = 0.8f;

    [Header("References")]
    [SerializeField] private Transform shakeParent;
    [SerializeField] private Transform cristalParent;
    
    [Header("Scale Parameters")] 
    [SerializeField] private float scaleOnHover = 1.15f;
    [SerializeField] private float scaleOnSelect = 1.25f;
    [SerializeField] private float scaleTransition = 0.15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [Header("Tilt Parameters")]
    [SerializeField] private float tiltAmount = 10;
    [SerializeField] private float tiltSpeed = 20;
    
    [Header("Scale Parameters")]
    [SerializeField] private float selectPunchAmount = 20;
    
    [Header("Hover Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = 0.15f;


    private bool interactable;
    private bool isHovering;
    private bool isSelected;

    private List<UIDissolve> dissolveControllers = new List<UIDissolve>();
    
    private Canvas canvas;
    private SC_RewardUI rewardUI;

    public static GameObject lastSelected;
    
    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        dissolveControllers = GetComponentsInChildren<UIDissolve>().ToList();
        rewardUI = GetComponentInParent<SC_RewardUI>();
        SC_InputManager.instance.navigate.started += Navigate;
        //shockWavePS.material = new Material(shockWavePS.material);
        //shockWavePS.startColor = borderImage.color;
    }

    private void Update()
    {
        TiltCristal();

        print(SC_InputManager.instance.lastDeviceUsed);
    }

    private void Navigate(InputAction.CallbackContext context)
    {
        if (SC_InputManager.instance.lastDeviceUsed == "Mouse")
            return;

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            print("Null");
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        
        if (EventSystem.current.currentSelectedGameObject == gameObject)
            OnPointerEnter(new PointerEventData(EventSystem.current));
        else
            OnPointerExit(new PointerEventData(EventSystem.current));
    }
    
    
    private void OnEnable()
    {
        interactable = true;
        isSelected = false;
    }

    public void TiltCristal()
    {
        var tiltEulerAngles = cristalParent.eulerAngles;


        float sine = Mathf.Sin(Time.unscaledTime);
        float cosine = Mathf.Cos(Time.unscaledTime);

        float lerpX = Mathf.LerpAngle(tiltEulerAngles.x, sine * tiltAmount, tiltSpeed * Time.unscaledDeltaTime);
        float lerpY = Mathf.LerpAngle(tiltEulerAngles.y, cosine * tiltAmount, tiltSpeed * Time.unscaledDeltaTime);
        float lerpZ = Mathf.LerpAngle(tiltEulerAngles.z, 0, tiltSpeed / 2 * Time.unscaledDeltaTime);

        cristalParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shakeParent.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase).SetUpdate(true);
        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetUpdate(true).SetId(3);
        isHovering = true;
        if (SC_InputManager.instance.lastDeviceUsed == "Mouse")
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DOTween.Kill(3, true);
        shakeParent.DOScale(1, scaleTransition).SetEase(scaleEase).SetUpdate(true);
        isHovering = false;
        if (SC_InputManager.instance.lastDeviceUsed == "Mouse")
        {
            lastSelected = gameObject;
            EventSystem.current.SetSelectedGameObject(null);
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable)
            return;

        if (!isHovering)
            return;
        
        canvas.overrideSorting = false;
        interactable = false;
        isSelected = true;
        rewardUI.DiscardOtherRewards(this);

        DOTween.Kill(2, true);
        shakeParent.DOPunchRotation(Vector3.forward * (hoverPunchAngle / 2), hoverTransition, 20, 1).SetId(2).SetUpdate(true);
        shakeParent.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase).SetUpdate(true);
        shakeParent.DOPunchPosition(shakeParent.up * selectPunchAmount, scaleTransition, 10, 1).SetUpdate(true);

        StartCoroutine(DelayedSelection());
    }

    public void Discard()
    {
        foreach (var c in dissolveControllers)
        {
            c.effectPlayer.duration = selectionDelay;
            c.Play();
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;

        shakeParent.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase).SetUpdate(true);
    }

    private IEnumerator DelayedSelection()
    {
        yield return new WaitForSecondsRealtime(selectionDelay);
        GetComponent<SC_RewardItemUI>().OnClick();
    }
}
