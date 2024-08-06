using UnityEngine;

public class EnemyChasingState : BaseState<EnemyBehaviorStates.EnemyState>
{
    #region Private Fields
    private EnemyBehaviorStates _enemy;
    private float _extendedDetectionRange;
    #endregion

    #region Constructor
    public EnemyChasingState(EnemyBehaviorStates.EnemyState key, EnemyBehaviorStates enemy) : base(key)
    {
        _enemy = enemy;
        _extendedDetectionRange = _enemy.DetectionRange * 2; // Extended range is twice the detection range
    }
    #endregion

    #region State Methods
    public override void EnterState()
    {
        Debug.Log("EnemyChasing");
        _enemy.Animator.SetBool(HashStrings.Run, true);
    }

    public override void ExitState()
    {
        _enemy.Animator.SetBool(HashStrings.Run, false);
    }

    public override void UpdateState()
    {
        Vector3 direction = (_enemy.Player.position - _enemy.transform.position).normalized;
        Vector3 move = direction * _enemy.ChaseSpeed * Time.deltaTime;

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
        float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _enemy.Player.position);

        if (_enemy.CanSeePlayer())
        {
            if (distanceToPlayer <= _enemy.AttackRange)
                return EnemyBehaviorStates.EnemyState.Attack;

            if (distanceToPlayer > _extendedDetectionRange)
                return EnemyBehaviorStates.EnemyState.Patrol;
        }
        else
        {
            return EnemyBehaviorStates.EnemyState.Patrol;
        }

        return EnemyBehaviorStates.EnemyState.Chasing;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
    #endregion
}
