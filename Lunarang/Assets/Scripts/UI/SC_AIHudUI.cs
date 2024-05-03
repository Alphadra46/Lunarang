using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_AIHudUI : MonoBehaviour
{

    private Quaternion rot;
    
    private void Awake()
    {
        rot = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = rot;
    }
}
