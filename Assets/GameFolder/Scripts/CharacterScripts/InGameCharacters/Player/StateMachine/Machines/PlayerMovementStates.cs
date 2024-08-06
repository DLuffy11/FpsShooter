using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementStates : StateManager<PlayerMovementStates.PlayerState>
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Land,
    }

    // Public Properties
    public bool IsWalkPressed => _isWalkPressed;
    public bool IsRunPressed => _isRunPressed;
    public bool IsJumpPressed => _isJumpPressed;
    public bool IsGrounded => _isGrounded;
    public bool IsFalling => _isFalling;
    public bool IsLanding => _isLanding;
    public bool IsAimPressed => _isAimPressed;
    public Animator Animator => _animator;

    // Serialized Fields
    [SerializeField] private GameObject _followTargetTransform;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _turnSmoothTime = 0.1f;
    [SerializeField] private float _rotationPower = 0.3f;

    //Components
    private Transform _camera;
    private CharacterController _controller;
    private CharacterControlls _input;
    private Player _player;
    private PlayerShooting _playerShooting;
    private Animator _animator;
    private GameObject _character;

    //Movement Speed
    private float _speed;
    private float _walkingSpeed = 1.5f;
    private float _runningSpeed = 5.5f;

    private float _gravity = -9.81f;

    //BooleanChecking
    private bool _isGrounded;
    private bool _isFalling;
    private bool _isLanding;
    private bool _isWalkPressed;
    private bool _isRunPressed;
    private bool _isJumpPressed;
    private bool _isAimPressed;

    //Input Values
    private Vector3 _velocity;
    private Vector2 _inputMovement;
    private float _turnSmoothVelocity;


    //Camera
    private float _currentCameraPitch = 0f;
    private Vector2 _look;


    #region Init
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();
        _playerShooting = GetComponent<PlayerShooting>();

        _character = transform.GetChild(0).gameObject;
        _camera = Camera.main.transform;

        _input = new CharacterControlls();

        InitializeStates();
        ConfigureInput();

        _currentState = _states[PlayerState.Idle];
        _speed = _walkingSpeed;
    }

    private void InitializeStates()
    {
        _states.Add(PlayerState.Idle, new IdleState(PlayerState.Idle, this));
        _states.Add(PlayerState.Walk, new WalkState(PlayerState.Walk, this));
        _states.Add(PlayerState.Run, new RunState(PlayerState.Run, this));
        _states.Add(PlayerState.Jump, new JumpState(PlayerState.Jump, this));
        _states.Add(PlayerState.Land, new LandState(PlayerState.Land, this));
    }

    private bool _wasRunPressedLastFrame;
    private bool _wasAimPressedLastFrame;

    private void ConfigureInput()
    {
        _input.Player.Sprint.performed += ctx =>
        {
            _wasRunPressedLastFrame = _isRunPressed;
            _isRunPressed = ctx.ReadValueAsButton();
            if (_isRunPressed && _wasAimPressedLastFrame)
            {
                _isAimPressed = false;
                EventBus.AimReleased();
            }
        };
        _input.Player.Sprint.canceled += ctx =>
        {
            _isRunPressed = false;
            if (_wasAimPressedLastFrame)
            {
                EventBus.AimPressed();
            }
        };

        _input.Player.Look.performed += ctx => _look = ctx.ReadValue<Vector2>();

        _input.Player.Movement.performed += ctx =>
        {
            _inputMovement = ctx.ReadValue<Vector2>();
            _isWalkPressed = _inputMovement.x != 0 || _inputMovement.y != 0;
        };
        _input.Player.Movement.canceled += ctx =>
        {
            _inputMovement = Vector2.zero;
            _isWalkPressed = false;
        };

        _input.Player.Jump.performed += ctx =>
        {
            if (!_isJumpPressed)
                _isJumpPressed = true;
        };
        _input.Player.Jump.canceled += ctx => _isJumpPressed = false;

        _input.Player.Aim.performed += ctx =>
        {
            _isAimPressed = ctx.ReadValueAsButton();
            if (_isAimPressed)
            {
                EventBus.AimPressed();
            }
            else
            {
                EventBus.AimReleased();
            }
        };

        _input.Player.Aim.canceled += ctx =>
        {
            _isAimPressed = false;
            EventBus.AimReleased();
        };
    }



    #endregion

    private new void Update()
    {

        if(!_player.IsDead)
            {
                base.Update();
                HandleGravity();
                Move();
                RotateCamera();
            }   
    }


    #region Scripts
    private void Move()
    {
        Vector3 cameraForward = _camera.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = _camera.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * _inputMovement.y + cameraRight * _inputMovement.x).normalized;

        if (_isAimPressed && !_isRunPressed)
        {
            float targetAngle = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(_character.transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            _character.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        else if (direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(_character.transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            _character.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        // Adjust speed based on whether running is pressed
        _speed = _isRunPressed ? _runningSpeed : _walkingSpeed;

        Vector3 movement = direction * _speed * Time.deltaTime;
        movement.y = _velocity.y * Time.deltaTime;
        _controller.Move(movement);
    }


    private void RotateCamera()
    {
        if (_look != Vector2.zero)
        {
            float mouseX = _look.x * _rotationPower * Time.deltaTime;
            float mouseY = _look.y * _rotationPower * Time.deltaTime;

            _followTargetTransform.transform.Rotate(Vector3.up, mouseX);

            _currentCameraPitch -= mouseY;
            _currentCameraPitch = Mathf.Clamp(_currentCameraPitch, -80f, 80f);

            _followTargetTransform.transform.localEulerAngles = new Vector3(_currentCameraPitch, _followTargetTransform.transform.localEulerAngles.y, 0f);

            // Reset _look after using it
            _look = Vector2.zero;
        }
    }

    private void HandleGravity()
    {
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (_isGrounded)
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
    }

    #endregion


    #region ChangingParams
    public void ChangeSpeedToRunning()
    {
        _speed = _runningSpeed;
    }

    public void ChangeSpeedToWalking()
    {
        _speed = _walkingSpeed;
    }

    public void ChangeFallingState()
    {
        _isFalling = !_isFalling;
    }

    public void ChangeLandingState()
    {
        _isLanding = !_isLanding;
    }

    public void ChangeJumpingState()
    {
        _isJumpPressed = !_isJumpPressed;
    }

    #endregion

    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }
}
