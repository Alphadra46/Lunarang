using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MiniMapComponent : MonoBehaviour
{

    public Transform playerTransform;

    private void Update()
    {
        transform.position = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
    }
}
