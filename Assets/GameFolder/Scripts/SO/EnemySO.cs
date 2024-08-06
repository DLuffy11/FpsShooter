using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "ScriptableObjects/Character/Enemy")]
public class EnemySO : CharacterStatsSO
{

    public float PatrolSpeed => _patrolSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;
    public WeaponSO WeaponUsed => _weaponUsed;

    [Header("Enemy Behavior Stats")]
    [SerializeField] private float _patrolSpeed;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _detectionRange;
    [SerializeField] private float _attackRange;

    [SerializeField] private WeaponSO _weaponUsed;

}
