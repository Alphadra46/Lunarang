using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SC_InteractableBase : MonoBehaviour, IInteractable
{

    public Action whenInteractable;
    public Action whenNotInteractable;
    
    [SerializeField] private List<UnityEvent> events = new List<UnityEvent>();

    private SC_InteractorComponent interactor;
    
    [PropertySpace(SpaceBefore = 15f)]
    public bool isInteractable = true;
    
    [PropertySpace(SpaceBefore = 15f)]
    public bool interactableOnce;
    
    [ShowIf("interactableOnce"), PropertySpace(SpaceBefore = 5f)]
    public bool destroyOnInteractionEnded;
    
    private bool wasInteracted;
    
    [PropertySpace(SpaceBefore = 15f)]
    public bool isInteractionEnded;

    [PropertySpace(SpaceBefore = 15f)]
    public float interactableRange;

    [PropertySpace(SpaceBefore = 15f)]
    public GameObject promptUI;

    public void Start()
    {
        SC_GameManager.instance.allInteractables.Add(gameObject);
    }

    private void OnEnable()
    {
        whenInteractable += ShowPrompt;
        whenNotInteractable += HidePrompt;
    }

    private void OnDisable()
    {
        whenInteractable -= ShowPrompt;
        whenNotInteractable -= HidePrompt;
    }

    public void Interact(SC_InteractorComponent newInteractor)
    {
        if(!isInteractable) return;
        
        if(interactableOnce && wasInteracted) return;

        if (interactableOnce) wasInteracted = true;
        
        interactor = newInteractor;
        interactor.inInteraction = true;
        if (events.Count <= 0)
        {
            print("Null Ref");
            return;
        }
        
        foreach (var e in events)
        {
            e.Invoke();
        }

        StartCoroutine(WaitUntilEnded());
    }

    public void EndInteraction()
    {
        isInteractionEnded = true;
        SC_InteractorComponent.onInteractionEnd?.Invoke(gameObject, interactableOnce, destroyOnInteractionEnded);
    }

    private bool CheckInteractionStatut()
    {
        return isInteractionEnded;
    }

    public void OpenRewardChest()
    {
        
        SC_GameManager.instance.OpenRewardChest();
        
    }

    public void OpenResourceChest()
    {
        SC_RewardManager.instance.ResourceDropSelection("Chest", out int a);
    }
    
    private IEnumerator WaitUntilEnded()
    {

        yield return new WaitUntil(CheckInteractionStatut);

        interactor.inInteraction = false;

    }

    private void ShowPrompt()
    {
        if(promptUI == null) return;
        promptUI.SetActive(true);
        
    }
    
    private void HidePrompt()
    {
        if(promptUI == null) return;
        
        promptUI.SetActive(false);
        
    }
    
}
