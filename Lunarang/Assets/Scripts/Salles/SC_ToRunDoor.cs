using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_ToRunDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SC_GameManager.instance.ChangeState(GameState.RUN);

    }
}
