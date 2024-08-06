using UnityEngine;

public class EnemyDetectionState : BaseState<EnemyBehaviorStates.EnemyState>
{
    private EnemyBehaviorStates _enemy;

    public EnemyDetectionState(EnemyBehaviorStates.EnemyState key, EnemyBehaviorStates enemy) : base(key)
    {
        _enemy = enemy;
    }

    public override void EnterState() {}

    public override void ExitState() {}

    public override void UpdateState() {}

    public override EnemyBehaviorStates.EnemyState GetNextState()
    {

        if(_enemy.CanSeePlayer())
        {
        if (Vector3.Distance(_enemy.transform.position, _enemy.Player.position) <= _enemy.AttackRange)
            return EnemyBehaviorStates.EnemyState.Attack;

        else if (Vector3.Distance(_enemy.transform.position, _enemy.Player.position) <= _enemy.DetectionRange)
            return EnemyBehaviorStates.EnemyState.Chasing;
        }
        else
            return EnemyBehaviorStates.EnemyState.Patrol;


        return EnemyBehaviorStates.EnemyState.Detection; 
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
}
