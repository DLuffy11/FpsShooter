using System.Collections.Generic;
using UnityEngine;
using static EnemyBehaviorStates;

[RequireComponent(typeof(CharacterController))]
public class EnemyBehaviorStates : StateManager<EnemyBehaviorStates.EnemyState>
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Detection,
        Chasing,
        Attack,
        Die,
    }

    // Properties
    public Transform Player => _player;
    public List<Transform> PatrolPoints => _patrolPoints;
    public CharacterController Controller => _controller;
    public Animator Animator => _animator;
    public WeaponSO WeaponUsed => _enemy.EnemyStats.WeaponUsed;
    public AudioSource AudioSource => _audioSource;
    public Transform EnemyCharacter => _enemyCharacter;
    public Transform BulletPos => _bulletPos;
    public float PatrolSpeed => _patrolSpeed;
    public float ChaseSpeed => _chaseSpeed;
    public float DetectionRange => _detectionRange;
    public float AttackRange => _attackRange;

    // Serialized Fields
    [SerializeField] private Transform _bulletPos;
    [SerializeField] private List<Transform> _patrolPoints = new List<Transform>();
    [SerializeField] private GameObject _patrolPointsParent;
    [ReadOnly, SerializeField] private float _patrolSpeed = 2.0f;
    [ReadOnly, SerializeField] private float _chaseSpeed = 4.0f;
    [ReadOnly, SerializeField] private float _detectionRange = 5.0f;
    [ReadOnly, SerializeField] private float _attackRange = 1.5f;

    // Components
    private CharacterController _controller;
    private Animator _animator;
    private Enemy _enemy;
    private Transform _player;
    private Transform _enemyCharacter;
    private AudioSource _audioSource;
    private bool _isDead; // Flag to track if the character is dead

    #region Init
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemy = GetComponent<Enemy>();
        _controller = GetComponent<CharacterController>();
        _enemyCharacter = gameObject.transform.GetChild(0);
        _player = GameObject.FindWithTag(HashStrings.Player).transform;
        _audioSource = GetComponent<AudioSource>();

        SetStatesValues();
        InitializeStates();
        InitializePatrolPoints();

        _currentState = _states[EnemyState.Idle];

        EventBus.OnCharacterDied += HandleCharacterDied; // Subscribe to the event
    }

    private void OnDestroy()
    {
        EventBus.OnCharacterDied -= HandleCharacterDied; // Unsubscribe from the event
    }

    private void SetStatesValues()
    {
        _patrolSpeed = _enemy.EnemyStats.PatrolSpeed;
        _chaseSpeed = _enemy.EnemyStats.ChaseSpeed;
        _detectionRange = _enemy.EnemyStats.DetectionRange;
        _attackRange = _enemy.EnemyStats.AttackRange;
    }

    private void InitializeStates()
    {
        _states.Add(EnemyState.Idle, new EnemyIdleState(EnemyState.Idle, this));
        _states.Add(EnemyState.Patrol, new EnemyPatrolState(EnemyState.Patrol, this));
        _states.Add(EnemyState.Detection, new EnemyDetectionState(EnemyState.Detection, this));
        _states.Add(EnemyState.Chasing, new EnemyChasingState(EnemyState.Chasing, this));
        _states.Add(EnemyState.Attack, new EnemyAttackState(EnemyState.Attack, this));
    }

    private void InitializePatrolPoints()
    {
        if (_patrolPointsParent != null)
        {
            _patrolPoints.Clear();
            foreach (Transform child in _patrolPointsParent.transform)
            {
                _patrolPoints.Add(child);
            }
        }
    }
    #endregion

    private new void Update()
    {
        base.Update();

        CanSeePlayer();
    }

    public bool CanSeePlayer()
    {
        if (_isDead)
            return false;

        Vector3 targetPosition = _player.position + Vector3.up * 1.5f; // Adjust the offset value as needed
        Vector3 direction = (targetPosition - _bulletPos.position).normalized;
        float distance = Vector3.Distance(_bulletPos.position, targetPosition);

        int layerMask = LayerMask.GetMask("Enemy");

        RaycastHit hit;
        if (Physics.Raycast(_bulletPos.position, direction, out hit, distance, ~layerMask))
        {
            if (hit.transform.CompareTag(HashStrings.Player))
                return true;
        }
        return false;
    }

    private void HandleCharacterDied()
    {
        _isDead = true; // Update the flag when the character dies
    }

    public void InstantiateEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation, float duration = 2f)
    {
        GameObject effectInstance = Instantiate(effectPrefab, position, rotation);

        if (effectInstance != null)
            Destroy(effectInstance, duration);
    }

}
