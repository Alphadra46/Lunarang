using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI_Archer_PatrolState : BaseState<AI_Archer_StateMachine.EnemyState>
{
    
    public AI_Archer_PatrolState(AI_Archer_StateMachine.EnemyState key, AI_Archer_StateMachine manager) : base(key , manager)
    {
        _aiArcherStateMachine = manager;
    }

    #region Variables

    private readonly AI_Archer_StateMachine _aiArcherStateMachine;
    private NavMeshAgent _agent;

    private bool canMove = true;

    private Collider[] objectsInArea;

    #endregion

    public override void EnterState()
    {
        Debug.Log("PATROL");
        _agent = _aiArcherStateMachine.agent;
        _agent.updateRotation = false;
    }

    public override void ExitState()
    {
        
    }

    public override void UpdateState()
    {
        
        objectsInArea = Physics.OverlapSphere(_aiArcherStateMachine.centerPoint.position, _aiArcherStateMachine.chaseAreaRadius);
        
        if (objectsInArea.Any(obj => obj.CompareTag("Player")))
        {
            _aiArcherStateMachine.TransitionToState(AI_Archer_StateMachine.EnemyState.Chase);
            Debug.Log("!!");
        }
        else
        {
            if ((!(_agent.remainingDistance <= _agent.stoppingDistance))) return;
        
            if (!canMove) return;

            if (!_aiArcherStateMachine.RandomPoint(_aiArcherStateMachine.transform.position, _aiArcherStateMachine.RandomPatrolRange(),
                    out var point)) return;
            
            Debug.DrawRay(point, Vector3.up, Color.blue, 1f);
            _agent.SetDestination(point);

            _aiArcherStateMachine.centerPoint.LookAt(new Vector3(point.x, _aiArcherStateMachine.centerPoint.position.y, point.z));
            
            _aiArcherStateMachine.StartCoroutine(DelayBeforeNextDestination());
        }
        
    }

    public override AI_Archer_StateMachine.EnemyState GetNextState()
    {
        return AI_Archer_StateMachine.EnemyState.Patrol;
    }

    IEnumerator DelayBeforeNextDestination()
    {
        canMove = false;
        yield return new WaitForSeconds(_aiArcherStateMachine.patrolDelay);
        canMove = true;
    }
    
}
