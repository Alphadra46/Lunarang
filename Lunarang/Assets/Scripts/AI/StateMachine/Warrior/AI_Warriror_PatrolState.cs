using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI_Warrior_PatrolState : BaseState<AI_Warrior_StateMachine.EnemyState>
{
    
    public AI_Warrior_PatrolState(AI_Warrior_StateMachine.EnemyState key, AI_Warrior_StateMachine manager) : base(key , manager)
    {
        _aiStateMachine = manager;
    }

    #region Variables

    private readonly AI_Warrior_StateMachine _aiStateMachine;
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
    }

    public override void ExitState()
    {
        
    }

    /// <summary>
    /// Check if Player is in chase area, if yes enter Chase State, else set a random point in his range.
    /// </summary>
    public override void UpdateState()
    {
        
        objectsInArea = Physics.OverlapSphere(_aiStateMachine.centerPoint.position, _aiStateMachine.chaseAreaRadius);
        
        if (objectsInArea.Any(obj => obj.CompareTag("Player")))
        {
            _aiStateMachine.TransitionToState(AI_Warrior_StateMachine.EnemyState.Chase);
        }
        else
        {
            if ((!(_agent.remainingDistance <= _agent.stoppingDistance))) return;
        
            if (!canMove) return;

            if (!_aiStateMachine.RandomPoint(_aiStateMachine.transform.position, 6,
                    out var point)) return;
            
            Debug.DrawRay(point, Vector3.up, Color.blue, 1f);
            _agent.SetDestination(point);

            _aiStateMachine.centerPoint.LookAt(new Vector3(point.x, _aiStateMachine.centerPoint.position.y, point.z));
            
            _aiStateMachine.StartCoroutine(DelayBeforeNextDestination());
        }
        
    }

    public override AI_Warrior_StateMachine.EnemyState GetNextState()
    {
        return AI_Warrior_StateMachine.EnemyState.Patrol;
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
