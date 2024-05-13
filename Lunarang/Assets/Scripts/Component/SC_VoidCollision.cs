using System;
using UnityEngine;

public class SC_VoidCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            SC_PlayerController.instance.Teleport(SC_PlayerController.instance.lastPos);
    }
}
