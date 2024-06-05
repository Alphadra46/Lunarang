using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_PatrolState : BaseState<AI_Archer_StateMachine.EnemyState>
{
    
    public AI_Archer_PatrolState(AI_Archer_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key , manager)
    {
        _aiStateMachine = manager;
    }

    #region Variables

    private readonly AI_Archer_StateMachine _aiStateMachine;
    private NavMeshAgent _agent;

    private bool canMove = true;

    private Collider[] objectsInArea;

    #endregion

    /// <summary>
    /// Initialize references
    /// </summary>
    public override void EnterState()
    {
        _agent = _aiStateMachine.agent;
        _agent.updateRotation = false;
        _agent.speed = _aiStateMachine.patrolSpeed;
        
        _aiStateMachine.canAttack = true;
    }

    public override void ExitState()
    {
        
    }
    
    /// <summary>
    /// Check if Player is in chase area, if yes enter Chase State, else set a random point in his range.
    /// </summary>
    public override void UpdateState()
    {
        
        objectsInArea = Physics.OverlapSphere(_aiStateMachine.centerPoint.position, _aiStateMachine.detectionAreaRadius);
        
        if (objectsInArea.Any(obj => obj.CompareTag("Player")))
        {
            _aiStateMachine.TryToTransition(AI_Archer_StateMachine.EnemyState.Chase);
            Debug.Log("!!");
        }
        else
        {
            if ((!(_agent.remainingDistance <= _agent.stoppingDistance))) return;
        
            if (!canMove) return;

            if (!_aiStateMachine.RandomPoint(_aiStateMachine.transform.position, _aiStateMachine.RandomPatrolRange(_aiStateMachine.patrolRadiusMin, _aiStateMachine.patrolRadiusMax),
                    out var point)) return;
            
            Debug.DrawRay(point, Vector3.up, Color.blue, 1f);
            _agent.SetDestination(point);

            _aiStateMachine.centerPoint.LookAt(new Vector3(point.x, _aiStateMachine.centerPoint.position.y, point.z));
            
            _aiStateMachine.StartCoroutine(DelayBeforeNextDestination());
        }
        
    }

    public override AI_Archer_StateMachine.EnemyState GetNextState()
    {
        return AI_Archer_StateMachine.EnemyState.Patrol;
    }
    
    /// <summary>
    /// Delay before find a new destination.
    /// </summary>
    IEnumerator DelayBeforeNextDestination()
    {
        canMove = false;
        yield return new WaitForSeconds(_aiStateMachine.patrolDelay);
        canMove = true;
    }
    
}
