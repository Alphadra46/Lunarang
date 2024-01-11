using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI_Summoner_PatrolState : BaseState<AI_StateMachine.EnemyState>
{
    
    public AI_Summoner_PatrolState(AI_StateMachine.EnemyState key, AI_Summoner_StateMachine manager) : base(key , manager)
    {
        _aiStateMachine = manager;
    }

    #region Variables

    private readonly AI_Summoner_StateMachine _aiStateMachine;
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
        
        objectsInArea = Physics.OverlapSphere(_aiStateMachine.centerPoint.position, _aiStateMachine.detectionAreaRadius);
        
        if (objectsInArea.Any(obj => obj.CompareTag("Player")))
        {
            _aiStateMachine.TransitionToState(AI_StateMachine.EnemyState.Chase);
        }
        
    }

    public override AI_StateMachine.EnemyState GetNextState()
    {
        return AI_StateMachine.EnemyState.Patrol;
    }
    
}
