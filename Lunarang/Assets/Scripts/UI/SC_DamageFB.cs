using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DamageFB : MonoBehaviour
{
    public Animator _animator;
    
    private void Awake()
    {
        if(!TryGetComponent(out _animator)) return;
    }

    public void Destroy()
    {
        _animator.SetTrigger("Destroy");
    }
}
