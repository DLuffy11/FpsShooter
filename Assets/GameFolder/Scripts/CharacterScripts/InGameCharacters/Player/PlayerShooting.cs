using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public WeaponRuntimeData CurrentWeapon { get; private set; }
    public bool IsReloading => _isReloading;

    #region Serialized Fields
    [SerializeField] private AudioClip _equipSound;
    [SerializeField] private Transform _gunPlace;
    #endregion

    #region Private Fields
    private WeaponRuntimeData _currentWeapon;
    private float _nextFireTime;
    private bool _isShooting;
    private bool _isAiming;
    private bool _isReloading;

    private CharacterControlls _input;
    private PlayerMovementStates _movements;
    private Camera _mainCamera;
    private Inventory _inventory;
    private GameObject _currWeaponScene;
    private InventoryUI _inventoryUI;
    private Animator _animator;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        _input = new CharacterControlls();
        _movements = GetComponent<PlayerMovementStates>();
        _mainCamera = Camera.main;
        _inventory = GetComponent<Inventory>();
        _inventoryUI = FindObjectOfType<InventoryUI>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        EquipWeapon(0);
    }

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Shoot.performed += ctx => OnShootPressed(ctx.ReadValueAsButton());
        _input.Player.Shoot.canceled += ctx => _isShooting = false;

        _input.Player.InventorySelect1.performed += ctx => EquipWeapon(0);
        _input.Player.InventorySelect2.performed += ctx => EquipWeapon(1);
        _input.Player.InventorySelect3.performed += ctx => EquipWeapon(2);

        _input.Player.Reload.performed += ctx => Reload();

        EventBus.OnAimPressed += StartAiming;
        EventBus.OnAimReleased += StopAiming;
    }

    private void OnDisable()
    {
        _input.Player.Disable();
        _input.Player.Shoot.performed -= ctx => OnShootPressed(ctx.ReadValueAsButton());
        _input.Player.Shoot.canceled -= ctx => _isShooting = false;

        _input.Player.InventorySelect1.performed -= ctx => EquipWeapon(0);
        _input.Player.InventorySelect2.performed -= ctx => EquipWeapon(1);
        _input.Player.InventorySelect3.performed -= ctx => EquipWeapon(2);

        _input.Player.Reload.performed -= ctx => Reload();

        EventBus.OnAimPressed -= StartAiming;
        EventBus.OnAimReleased -= StopAiming;
    }

    private void Update()
    {
        if (_isAiming && _isShooting && Time.time >= _nextFireTime && !_isReloading)
            Shoot();
    }
    #endregion

    #region Input Handlers
    private void StartAiming()
    {
        _isAiming = true;
    }

    private void StopAiming()
    {
        _isAiming = false;
        _isShooting = false;
    }

    private void OnShootPressed(bool isPressed)
    {
        _isShooting = isPressed;
    }
    #endregion

    #region Weapon Management
    public void SetMainCamera(Camera camera)
    {
        _mainCamera = camera;
    }

    private void EquipWeapon(int index)
    {
        if (_isReloading)
        {
            Debug.Log("Cannot switch weapons while reloading!");
            return;
        }

        if (_inventory.Weapons.Count > index)
        {
            WeaponSO newWeaponSO = _inventory.Weapons[index];

            if (_currentWeapon != null && _currentWeapon.WeaponSO == newWeaponSO)
            {
                return;
            }

            if (_currentWeapon != null)
            {
                _inventory.SaveWeaponState(_currentWeapon.WeaponSO.Name, _currentWeapon.CurrentBullets, _currentWeapon.CurrentClips);
                Destroy(_currWeaponScene);
            }

            GameObject weaponObject = Instantiate(newWeaponSO.WeaponPrefab, _gunPlace);
            _currWeaponScene = weaponObject;
            WeaponScript weaponScript = weaponObject.GetComponent<WeaponScript>();

            if (weaponScript != null)
            {
                _currentWeapon = new WeaponRuntimeData(newWeaponSO);
                CurrentWeapon = _currentWeapon;

                WeaponState state = _inventory.LoadWeaponState(newWeaponSO.Name);
                if (state != null)
                {
                    _currentWeapon.CurrentBullets = state.CurrentBullets;
                    _currentWeapon.CurrentClips = state.CurrentClips;
                }
            }

            _inventoryUI.SetSelectedWeapon(newWeaponSO);

            if (_equipSound != null)
            {
                AudioSource.PlayClipAtPoint(_equipSound, transform.position);
            }
        }
    }
    #endregion

    #region Shooting Mechanics
    private void Shoot()
    {
        if (_currentWeapon != null && !_movements.IsRunPressed)
        {
            if (_currentWeapon.CurrentBullets > 0)
            {
                _nextFireTime = Time.time + 1f / _currentWeapon.WeaponSO.FireRate;
                RaycastShoot();

                if (_currentWeapon.WeaponSO.FireSound != null)
                {
                    AudioSource.PlayClipAtPoint(_currentWeapon.WeaponSO.FireSound, transform.position);
                }

                _currentWeapon.CurrentBullets--;
            }
            else
            {
                Debug.Log("Out of ammo!");
                Reload();
            }
        }
    }

    private void RaycastShoot()
    {
        if (_currentWeapon != null && _mainCamera != null)
        {
            Ray ray = _mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Vector3 hitPoint = ray.origin + ray.direction * 100f;

            if (Physics.Raycast(ray, out hit))
            {
                hitPoint = hit.point;

                if (_currentWeapon.WeaponSO.HitParticlePrefab != null)
                {
                    GameObject hitEffect = Instantiate(_currentWeapon.WeaponSO.HitParticlePrefab, hitPoint, Quaternion.identity);
                    float particleDuration = hitEffect.GetComponent<ParticleSystem>().main.duration;
                    Destroy(hitEffect, particleDuration);
                }

                if (_currentWeapon.WeaponSO.ImpactSound != null)
                {
                    AudioSource.PlayClipAtPoint(_currentWeapon.WeaponSO.ImpactSound, hitPoint);
                }

                var health = hit.collider.GetComponent<Enemy>();
                if (health != null)
                {
                    health.TakeDamage(_currentWeapon.WeaponSO.Damage);
                }
            }

            Debug.DrawLine(ray.origin, hitPoint, Color.red, 1.0f);
        }
    }
    #endregion

    #region Reload Mechanics
    private void Reload()
    {
        if (_currentWeapon != null && !_isReloading)
        {
            if (_currentWeapon.CurrentClips > 0 && _currentWeapon.CurrentBullets < _currentWeapon.WeaponSO.MaxBullets)
            {
                _isReloading = true;
                _animator.SetTrigger(HashStrings.StartReload);
                _animator.SetBool(HashStrings.Reloading, true);

                StartCoroutine(SmoothLayerWeight(1f, .5f));
                StartCoroutine(CompleteReload());
            }
            else
            {
                Debug.Log("No more clips!");
            }
        }
    }

    private IEnumerator SmoothLayerWeight(float targetWeight, float duration)
    {
        float currentWeight = _animator.GetLayerWeight(1);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float weight = Mathf.Lerp(currentWeight, targetWeight, elapsedTime / duration);
            _animator.SetLayerWeight(1, weight);
            yield return null;
        }

        _animator.SetLayerWeight(1, targetWeight);
    }

    private IEnumerator CompleteReload()
    {
        yield return new WaitForSeconds(2.27f);

        int bulletsToReload = _currentWeapon.WeaponSO.MaxBullets - _currentWeapon.CurrentBullets;
        _currentWeapon.CurrentClips--;
        _currentWeapon.CurrentBullets += bulletsToReload;

        if (_currentWeapon.CurrentBullets > _currentWeapon.WeaponSO.MaxBullets)
        {
            _currentWeapon.CurrentBullets = _currentWeapon.WeaponSO.MaxBullets;
        }

        _animator.SetBool(HashStrings.Reloading, false);

        StartCoroutine(SmoothLayerWeight(0f, .5f));

        _isReloading = false;
    }
    #endregion
}
