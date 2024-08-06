using UnityEngine;

public class EnemyIdleState : BaseState<EnemyBehaviorStates.EnemyState>
{
    private EnemyBehaviorStates _enemy;
    private float _idleTimer;
    private const float MaxIdleTime = 3.5f; // Time in seconds before transitioning to patrol

    public EnemyIdleState(EnemyBehaviorStates.EnemyState key, EnemyBehaviorStates enemy)
        : base(key)
    {
        _enemy = enemy;
    }

    public override void EnterState()
    {
        _idleTimer = 0f;
    }

    public override void ExitState() {}

    public override void UpdateState()
    {
        _idleTimer += Time.deltaTime;
    }

    public override EnemyBehaviorStates.EnemyState GetNextState()
    {
        if (_idleTimer >= MaxIdleTime)
        {
            return EnemyBehaviorStates.EnemyState.Patrol;
        }

        if (_enemy.CanSeePlayer())
        {
            float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _enemy.Player.position);

            if (distanceToPlayer <= _enemy.AttackRange)
            {
                return EnemyBehaviorStates.EnemyState.Attack;
            }

            if (distanceToPlayer <= _enemy.DetectionRange)
            {
                return EnemyBehaviorStates.EnemyState.Chasing;
            }
        }

        return EnemyBehaviorStates.EnemyState.Idle; // Default state
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}
