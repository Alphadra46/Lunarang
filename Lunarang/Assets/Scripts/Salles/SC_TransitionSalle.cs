using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TransitionSalle : SC_Subject
{
    [SerializeField] private GameObject nextRoomTP;
    [SerializeField] private GameObject cameraBounds;
    [SerializeField] private GameObject nextRoomCameraBounds;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (other.TryGetComponent(out CharacterController cc))
        {
            cameraBounds.SetActive(false);
            
            //TODO - Disable inputs instead of the component
            cc.enabled = false;
            
            other.transform.position = new Vector3(nextRoomTP.transform.position.x, other.transform.position.y, nextRoomTP.transform.position.z);
            
            NotifyObservers("enter");

            //TODO - Enable inputs instead of the component
            cc.enabled = true;
            
            nextRoomCameraBounds.SetActive(true);

            Debug.Log("La TP EST FAITE");
        }
        
        
    }
}
