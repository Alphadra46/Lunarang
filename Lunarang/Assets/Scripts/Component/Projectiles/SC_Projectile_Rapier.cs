using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


[RequireComponent(typeof(Rigidbody))]
public class SC_Projectile_Rapier : SC_Projectile
{
    private Transform target;

    private bool launched = false;

    public float delayBeforeLaunch = 1f;
    
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
        
        if(!sender.TryGetComponent(out SC_ComboController comboController)) return;


        if (comboController.currentEnemiesHitted.Length > 0)
        {
            target = comboController.currentEnemiesHitted[0].transform;
            direction = new Vector3(target.position.x, target.localScale.y*2, target.position.z) - _rb.position;
        }
        else
        {

            direction = new Vector3(sender.transform.GetChild(2).GetChild(0).position.x, -sender.transform.localScale.y/4, sender.transform.GetChild(2).GetChild(0).position.z) - _rb.position;

        }

        transform.forward = direction;

        StartCoroutine(LaunchProjectile());

    }

    private void FixedUpdate()
    {
        if(!launched) return;
        
        if(target != null){
            direction.Normalize();
     
            var rotateAmount = Vector3.Cross(direction, transform.forward).y;

            var rbAngularVelocity = _rb.angularVelocity;
            rbAngularVelocity.y = -rotateAmount * 10f;

            _rb.angularVelocity = rbAngularVelocity;
            _rb.velocity = transform.forward * ((speed * 100f) * Time.deltaTime);
        }
        else
        {
            _rb.velocity = transform.forward * ((speed * 100f) * Time.deltaTime);
        }
        
    }

    /// <summary>
    /// Detect collision and if collide with Player, apply damage to Player.
    /// </summary>
    /// <param name="col"></param>
    public override void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Obstacle"))
        {
            print("Destroy");
            Destroy(gameObject);
        }
        
        if (!col.TryGetComponent(out IDamageable damageable)) return;
        if (col.gameObject == sender) return;
        if(!col.CompareTag("Entity")) return;

        if (isAoE)
        {
            var ennemiesInAoE =
                Physics.OverlapSphere(transform.position, areaSize, LayerMask.GetMask("IA")); //TODO : Replace Pos by Weapon Hit Pos

            foreach (var e in ennemiesInAoE)
            {
                if (!e.TryGetComponent(out IDamageable aoeHitted)) continue;
                aoeHitted.TakeDamage(damage, isCrit, sender);

                if (additionalHits <= 1) continue;
                
                for (var i = 0; i < additionalHits-1; i++)
                {
                    aoeHitted.TakeDamage(damage, isCrit, sender);
                }

            }
        }
        
        else
        {
            for (var i = 0; i < additionalHits; i++)
            {
                if(col.CompareTag("Entity"))
                    damageable.TakeDamage(damage, isCrit, sender);
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

    public IEnumerator LaunchProjectile()
    {

        yield return new WaitForSeconds(delayBeforeLaunch);

        launched = true;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position,direction);
    }
}
