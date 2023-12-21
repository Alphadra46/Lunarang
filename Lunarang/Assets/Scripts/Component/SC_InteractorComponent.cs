using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SC_InteractorComponent : MonoBehaviour
{
    
    #region Variables
    
    [BoxGroup("Interaction")]
    [ShowInInspector] private List<GameObject> interactablesInArea = new List<GameObject>();

    [BoxGroup("Collider")]
    [SerializeField] private float colliderRadius = 1;
    [BoxGroup("Collider")]
    [SerializeField] private Vector3 colliderCenter = new Vector3(0, 1, 0);
    [BoxGroup("Collider")]
    [SerializeField] private LayerMask colliderLayer;

    [BoxGroup("Interaction")]
    public bool inInteraction;

    #endregion

    private void Reset()
    {
        UpdateColliderSettings();
    }
    
    [BoxGroup("Collider")]
    [Button("Update Collider")]
    public void UpdateColliderSettings()
    {
        var sphereCollider = GetComponent<SphereCollider>();
        
        sphereCollider.radius = colliderRadius;
        sphereCollider.center = colliderCenter;
        sphereCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
        if (other.TryGetComponent(out IInteractable interactable))
        {
            print(other.name);
            interactablesInArea.Add(other.gameObject);
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        
        if(other.TryGetComponent(out IInteractable interactable))
            interactablesInArea.Remove(other.gameObject);
        
    }

    public GameObject FindClosestInteractable()
    {
        float nearestDistance = 1;
        GameObject nearestInteractable = null;
        
        foreach (var interactable in interactablesInArea)
        {
            var distance = Vector3.Distance(this.transform.position, interactable.transform.position);

            if (!(distance < nearestDistance)) continue;
            
            nearestDistance = distance;
            nearestInteractable = interactable;

        }

        return nearestInteractable;

    }

    public void Interact()
    {
        
        if(inInteraction) return;

        inInteraction = true;
        FindClosestInteractable().GetComponent<IInteractable>().Interact(this);
        
    }

}
