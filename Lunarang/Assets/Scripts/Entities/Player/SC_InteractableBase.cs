using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SC_InteractableBase : MonoBehaviour, IInteractable
{
    
    [SerializeField] private List<UnityEvent> events = new List<UnityEvent>();

    private SC_InteractorComponent interactor;
    
    public bool interactableOnce;
    
    [ShowIf("interactableOnce")]
    public bool destroyOnInteractionEnded;
    
    private bool wasInteracted;
    
    public bool isInteractionEnded;

    public void Start()
    {
        SC_GameManager.instance.allInteractables.Add(gameObject);
    }

    public void Interact(SC_InteractorComponent newInteractor)
    {
        if(interactableOnce && wasInteracted) return;

        if (interactableOnce) wasInteracted = true;
        
        interactor = newInteractor;
        
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
        SC_InteractorComponent.onInteractionEnd?.Invoke(gameObject, interactableOnce && destroyOnInteractionEnded);
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
        SC_RewardManager.instance.ResourceDropSelection("Chest");
    }
    
    private IEnumerator WaitUntilEnded()
    {

        yield return new WaitUntil(CheckInteractionStatut);

        interactor.inInteraction = false;

    }
    
}
