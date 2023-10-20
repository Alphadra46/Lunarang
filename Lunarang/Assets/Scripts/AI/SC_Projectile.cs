using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SC_Projectile : MonoBehaviour
{
    #region Variables

    public float autoDestroyTime = 5f;
    public float speed = 1f;
    public int damage;
    public Rigidbody _rb;

    private const string DESTROY_METHOD_NAME = "Destroy";
    
    #endregion

    private void Awake()
    {
        if (!TryGetComponent(out _rb)) return;
        Invoke(DESTROY_METHOD_NAME, autoDestroyTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<SC_PlayerStats>().TakeDamage(damage);
        }
        
    }

    private void Destroy()
    {
        CancelInvoke(DESTROY_METHOD_NAME);
        _rb.velocity = Vector3.zero;
        Destroy(gameObject);
    }
    
}
