using System;
using UnityEngine;

public class SC_VoidCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {

        if (other.collider.CompareTag("Player"))
        {
            SC_PlayerController.instance.Teleport(SC_PlayerController.instance.lastPos);
        }

    }
    
}
