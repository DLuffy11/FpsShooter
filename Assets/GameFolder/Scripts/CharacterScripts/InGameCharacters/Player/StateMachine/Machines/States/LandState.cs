using UnityEngine;

public class LandState : BaseState<PlayerMovementStates.PlayerState>
{
    private PlayerMovementStates _playerMovement;

    public LandState(PlayerMovementStates.PlayerState key, PlayerMovementStates playerMovementStates)
        : base(PlayerMovementStates.PlayerState.Land)
    {
        _playerMovement = playerMovementStates;
    }

    public override void EnterState()
    {
        _playerMovement.Animator.SetBool(HashStrings.Land, true);
    }

    public override void ExitState()
    {
        _playerMovement.Animator.SetBool(HashStrings.Land, false);
        _playerMovement.ChangeLandingState();
    }

    public override PlayerMovementStates.PlayerState GetNextState()
    {
        if (_playerMovement.IsLanding)
        {
            if (_playerMovement.IsWalkPressed && _playerMovement.IsRunPressed)
                return PlayerMovementStates.PlayerState.Run;


            if (!_playerMovement.IsWalkPressed && !_playerMovement.IsRunPressed && _playerMovement.IsAimPressed)
            {
                _playerMovement.Animator.SetBool(HashStrings.Aim, true);
                return PlayerMovementStates.PlayerState.Idle;
            }

            if (!_playerMovement.IsWalkPressed && _playerMovement.IsRunPressed && _playerMovement.IsAimPressed)
            {
                _playerMovement.Animator.SetBool(HashStrings.Aim, true);
                return PlayerMovementStates.PlayerState.Idle;
            }

            if (!_playerMovement.IsWalkPressed && _playerMovement.IsRunPressed)
                return PlayerMovementStates.PlayerState.Idle;

            if (_playerMovement.IsWalkPressed && _playerMovement.IsAimPressed)
            {
                _playerMovement.Animator.SetBool(HashStrings.Aim, true);
                return PlayerMovementStates.PlayerState.Walk;
            }

            if (_playerMovement.IsWalkPressed)
                return PlayerMovementStates.PlayerState.Walk;

            return PlayerMovementStates.PlayerState.Idle;
        }

        return PlayerMovementStates.PlayerState.Land;
    }

    public override void OnTriggerEnter(Collider other) { }

    public override void OnTriggerExit(Collider other) { }

    public override void OnTriggerStay(Collider other) { }

    public override void UpdateState() { }
}
