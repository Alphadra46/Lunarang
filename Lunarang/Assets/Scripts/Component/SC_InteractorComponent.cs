using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SC_InteractorComponent : MonoBehaviour
{
    
    #region Variables

    public static Action<GameObject,bool> onInteractionEnd;

    [BoxGroup("Interaction")] public float nearInteractableRange = 15f;
    [BoxGroup("Interaction")] public float interactionRange = 2f;
    [BoxGroup("Interaction")]
    [ShowInInspector] private List<GameObject> nearInteractables = new List<GameObject>();
    [BoxGroup("Interaction")] public bool inInteraction;
    [BoxGroup("Interaction")] public GameObject nearestInteractable;
    
    #endregion

    private void Start()
    {
        SC_InputManager.instance.interaction.started += _ => Interact();
        onInteractionEnd += EndInteraction;
    }

    public void Update()
    {
        
        SortInteractableByDistance();

        nearestInteractable = FindClosestInteractable();
        
    }

    private void SortInteractableByDistance()
    {
        foreach (var interactable in SC_GameManager.instance.allInteractables)
        {
            var distance = Vector3.Distance(this.transform.position, interactable.transform.position);
            // print("near : " + distance);
            
            if ((distance < nearInteractableRange))
            {
                if (nearInteractables.Contains(interactable)) continue;
                
                nearInteractables.Add(interactable);
                interactable.SetActive(true);

            }
            else
            {
                if (!nearInteractables.Contains(interactable)) continue;
                
                nearInteractables.Remove(interactable);
                interactable.SetActive(false);

            }
            

        }
    }
    
    private GameObject FindClosestInteractable()
    {
        GameObject closestInteractable = null;
        
        foreach (var interactable in nearInteractables)
        {
            var distance = Vector3.Distance(this.transform.position, interactable.transform.position);
            // print("closest : " + distance);

            if ((!(distance < interactionRange))) continue;
            
            closestInteractable = interactable;

        }
        
        return closestInteractable;
        
    }

    private void Interact()
    {
        
        if(inInteraction) return;

        if(nearestInteractable == null) return;
        
        inInteraction = true;
        
        if(nearestInteractable.TryGetComponent(out IInteractable i))
        {
            i.Interact(this);
        }
        
    }
    
    
    private void EndInteraction(GameObject go, bool canDestroy)
    {
        nearInteractables.Remove(go);
        SC_GameManager.instance.allInteractables.Remove(go);
        if (canDestroy) Destroy(go,0.1f);
        inInteraction = false;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1, 0), nearInteractableRange);
        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1, 0), interactionRange);
    }
}
