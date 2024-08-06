using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : BaseState<EnemyBehaviorStates.EnemyState>
{
    private EnemyBehaviorStates _enemy;
    private Vector3 _currentDestination;
    private List<Transform> _patrolPoints;
    private int _currentPatrolIndex;

    public EnemyPatrolState(EnemyBehaviorStates.EnemyState key, EnemyBehaviorStates enemy) : base(key)
    {
        _enemy = enemy;
    }

    public override void EnterState()
    {
        _enemy.Animator.SetBool(HashStrings.Walk, true);
        InitializePatrolPoints();
        SetNewDestination();
    }

    public override void ExitState()
    {
        _enemy.Animator.SetBool(HashStrings.Walk, false);
    }

    public override void UpdateState()
    {
        MoveTowardsDestination();

        // Check if the enemy has reached the destination
        if (Vector3.Distance(_enemy.transform.position, _currentDestination) < 0.5f)
        {
            SetNewDestination();
        }
    }

    private void InitializePatrolPoints()
    {
        _patrolPoints = _enemy.PatrolPoints;
        _currentPatrolIndex = 0;
    }

    private void SetNewDestination()
    {
        if (_patrolPoints.Count > 0)
        {
            _currentDestination = _patrolPoints[_currentPatrolIndex].position;
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Count;
        }
        else
        {
            _currentDestination = _enemy.transform.position;
        }
    }

    private void MoveTowardsDestination()
    {
        Vector3 direction = (_currentDestination - _enemy.transform.position).normalized;
        Vector3 move = direction * _enemy.PatrolSpeed * Time.deltaTime;

        // Move the enemy using CharacterController
        _enemy.Controller.Move(move);

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            _enemy.EnemyCharacter.rotation = Quaternion.Slerp(_enemy.EnemyCharacter.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public override EnemyBehaviorStates.EnemyState GetNextState()
    {
        if (_enemy.CanSeePlayer())
        {
            if (Vector3.Distance(_enemy.transform.position, _enemy.Player.position) <= _enemy.DetectionRange)
                return EnemyBehaviorStates.EnemyState.Detection;
        }

        return EnemyBehaviorStates.EnemyState.Patrol;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}
