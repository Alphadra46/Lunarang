using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class SC_Projectile : SerializedMonoBehaviour
{
    
    #region Variables

    public float autoDestroyTime = 5f;
    public float speed = 1f;
    
    public float distanceMax;
    
    [HideInInspector]  public float damage;
    [HideInInspector]  public bool isCrit;
    
    [HideInInspector] public WeaponType weaponType;

    public bool isAoE;
    
    public float areaSize;
    public int hitNumber;

    [HideInInspector] public Vector3 direction;
    
    public GameObject sender; 
    public List<string> tags = new List<string>();
    
    private Rigidbody _rb;

    private const string DESTROY_METHOD_NAME = "Destroy";
    
    #endregion

    /// <summary>
    /// Get Rigidbody.
    /// Invoke a timer to destroy this GameObject after a certain delay.
    /// </summary>
    private void Awake()
    {
        if (!TryGetComponent(out _rb)) return;

        Invoke(DESTROY_METHOD_NAME, autoDestroyTime);
    }

    private void Start()
    {
        transform.forward = direction;
        
        _rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Detect collision and if collide with Player, apply damage to Player.
    /// </summary>
    /// <param name="col"></param>
    public virtual void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("Obstacle"))
        {
            print("Destroy");
            Destroy(gameObject);
        }
        
        if (!col.TryGetComponent(out IDamageable damageable)) return;
        if (col.gameObject == sender) return;
        if (!tags.Contains(col.tag)) return;

        if (isAoE)
        {
            var ennemiesInAoE =
                Physics.OverlapSphere(transform.position, areaSize, LayerMask.GetMask("Player", "IA")); //TODO : Replace Pos by Weapon Hit Pos

            foreach (var e in ennemiesInAoE)
            {
                if (!e.TryGetComponent(out IDamageable aoeHitted)) continue;
                aoeHitted.TakeDamage(damage, isCrit, sender);

                if (hitNumber <= 1) continue;
                
                for (var i = 0; i < hitNumber-1; i++)
                {
                    aoeHitted.TakeDamage(damage, isCrit, sender);
                }

            }
        }
        
        else
        {
            for (var i = 0; i < hitNumber; i++)
            {
                
                if(col.CompareTag("Entity"))
                    damageable.TakeDamage(damage, isCrit, sender);
                else if (col.CompareTag("Player"))
                {
                    damageable.TakeDamage(damage, false, sender);
                }

            }
        }
        
        Destroy(gameObject);
        
    }

    /// <summary>
    /// Destroy this GameObject.
    /// Cancel internal cooldown for destroying.
    /// Reset the velocity.
    /// </summary>
    public void Destroy()
    {
        CancelInvoke(DESTROY_METHOD_NAME);
        _rb.velocity = Vector3.zero;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position,direction);
    }
}
