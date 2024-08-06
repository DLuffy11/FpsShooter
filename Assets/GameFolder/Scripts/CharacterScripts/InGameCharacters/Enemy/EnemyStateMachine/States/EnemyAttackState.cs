using UnityEngine;
using UnityEngine.VFX;

public class EnemyAttackState : BaseState<EnemyBehaviorStates.EnemyState>
{
    #region Private Fields
    private EnemyBehaviorStates _enemy;
    private float _shootTimer;
    #endregion

    #region Constructor
    public EnemyAttackState(EnemyBehaviorStates.EnemyState key, EnemyBehaviorStates enemy) : base(key)
    {
        _enemy = enemy;
    }
    #endregion

    #region State Methods
    public override void EnterState()
    {
        _enemy.Animator.SetBool(HashStrings.Aim, true);
        _shootTimer = 1f;
    }

    public override void ExitState()
    {
        _enemy.Animator.SetBool(HashStrings.Aim, false);
    }

    public override void UpdateState()
    {
        _shootTimer -= Time.deltaTime;

        FacePlayer();

        if (_shootTimer <= 0f)
        {
            Shoot();
            _shootTimer = _enemy.WeaponUsed.FireRate;
        }
    }

    public override EnemyBehaviorStates.EnemyState GetNextState()
    {
        if (_enemy.CanSeePlayer())
        {
            if (Vector3.Distance(_enemy.transform.position, _enemy.Player.position) > _enemy.AttackRange)
                return EnemyBehaviorStates.EnemyState.Chasing;
        }
        else
        {
            return EnemyBehaviorStates.EnemyState.Patrol;
        }

        return EnemyBehaviorStates.EnemyState.Attack;
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other) { }
    public override void OnTriggerExit(Collider other) { }
    #endregion

    #region Private Methods
    private void Shoot()
    {
        if (Vector3.Distance(_enemy.transform.position, _enemy.Player.position) <= _enemy.AttackRange)
        {
            Vector3 targetPosition = _enemy.Player.position + Vector3.up * 1.5f; // Adjust the offset value as needed
            RaycastHit hit;
            Vector3 direction = (targetPosition - _enemy.BulletPos.position).normalized;

            if (Physics.Raycast(_enemy.BulletPos.position, direction, out hit, _enemy.AttackRange))
            {
                if (hit.transform.CompareTag(HashStrings.Player))
                {
                    Player playerHealth = hit.transform.GetComponent<Player>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(_enemy.WeaponUsed.Damage);
                    }

                    if (_enemy.WeaponUsed.HitParticlePrefab != null)
                    {
                        ParticleSystem effect = _enemy.WeaponUsed.HitParticlePrefab.GetComponent<ParticleSystem>();
                        var main = effect.main;

                        _enemy.InstantiateEffect(effect.gameObject, hit.point, Quaternion.identity, main.duration);
                    }
                }
            }

            // Play firing sound
            if (_enemy.WeaponUsed.FireSound != null)
            {
                _enemy.AudioSource.PlayOneShot(_enemy.WeaponUsed.FireSound);
            }
        }
    }

    private void FacePlayer()
    {
        Vector3 direction = (_enemy.Player.position - _enemy.EnemyCharacter.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        _enemy.EnemyCharacter.rotation = Quaternion.Slerp(_enemy.EnemyCharacter.rotation, lookRotation, Time.deltaTime * 5f); // Adjust the smoothing factor as needed
    }
    #endregion
}
