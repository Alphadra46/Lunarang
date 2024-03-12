using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using XNode.Odin;
using Random = UnityEngine.Random;
public abstract class StateManager<EState> : MonoBehaviour where EState : System.Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    [ShowInInspector, PropertySpace(SpaceAfter = 10)] protected BaseState<EState> CurrentState;
    [ShowInInspector, PropertySpace(SpaceAfter = 10)] public EState NextState;
    
    [ShowInInspector] protected bool IsTransitionState = false;
    
    /// <summary>
    /// Call current state start function.
    /// </summary>
    private void Start()
    {
        CurrentState.EnterState();
    }

    /// <summary>
    /// Call current state update function.
    /// </summary>
    private void Update()
    {
        CurrentState.UpdateState();
    }

    /// <summary>
    /// Switch to another state.
    /// </summary>
    /// <param name="stateKey">Next State</param>
    public void TransitionToState(EState stateKey)
    {
        if(IsTransitionState) return;
    
        NextState = stateKey;

        print(gameObject.GetInstanceID() + " : " + NextState);
        
        IsTransitionState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitionState = false;
        
    }
    
    /// <summary>
    /// Get the next destination in an range.
    /// </summary>
    /// <param name="center">Center of the range</param>
    /// <param name="range">Radius of the range</param>
    /// <param name="result">Found Point</param>
    /// <returns>Return a boolean depending on whether he found a point or not.</returns>
    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        var randomPoint = center + Random.insideUnitSphere * range;

        if (NavMesh.SamplePosition(randomPoint, out var hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Get a random range for Patrol between a Min and a Max value.
    /// </summary>
    /// <returns>Random patrol radius</returns>
    public float RandomPatrolRange(float min, float max)
    {
        var patrolRadius = Random.Range(min, max);

        return patrolRadius;
    }
    
    /// <summary>
    /// Check if the target is in line of sight.
    /// </summary>
    /// <param name="target">Transform targeted.</param>
    /// <param name="start">Transform of the start.</param>
    /// <param name="sphereRadius">Radius of the sphere at the end of the line.</param>
    /// <param name="layersAttackable">Layer hittable by the line.</param>
    /// <returns>
    /// Boolean of has in line of sight.
    /// </returns>
    public bool hasLineOfSightTo(Transform target, Transform start, float sphereRadius, LayerMask layersAttackable)
    {
        return Physics.SphereCast(start.position + Vector3.up, 0.1f,
            ((target.position + Vector3.up) -
             (start.position + Vector3.up)).normalized, out var Hit,
            sphereRadius, layersAttackable) && Hit.collider.CompareTag("Player");
    }
    
}
