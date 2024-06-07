using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_FaceCamera : MonoBehaviour
{

    private void LateUpdate()
    {
        if (Camera.main != null)
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
    }
    
}