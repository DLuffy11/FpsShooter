using UnityEngine;

public class RunState : BaseState<PlayerMovementStates.PlayerState>
{
    private PlayerMovementStates _playerMovement;

    public RunState(PlayerMovementStates.PlayerState key, PlayerMovementStates playerMovementStates)
        : base(PlayerMovementStates.PlayerState.Run)
    {
        _playerMovement = playerMovementStates;
    }

    public override void EnterState()
    {
        _playerMovement.Animator.SetBool(HashStrings.Run, true);
        _playerMovement.ChangeSpeedToRunning();
    }

    public override void ExitState()
    {
        _playerMovement.Animator.SetBool(HashStrings.Run, false);
        _playerMovement.ChangeSpeedToWalking();
    }

    public override PlayerMovementStates.PlayerState GetNextState()
    {
        if (_playerMovement.IsAimPressed && _playerMovement.IsWalkPressed && !_playerMovement.IsRunPressed)
        {
            _playerMovement.Animator.SetBool(HashStrings.Aim, true);
            return PlayerMovementStates.PlayerState.Walk;
        }

        if (_playerMovement.IsJumpPressed)
            return PlayerMovementStates.PlayerState.Jump;

        if (_playerMovement.IsWalkPressed && !_playerMovement.IsRunPressed)
            return PlayerMovementStates.PlayerState.Walk;

        if (!_playerMovement.IsWalkPressed)
            return PlayerMovementStates.PlayerState.Idle;

        return PlayerMovementStates.PlayerState.Run;
    }

    public override void OnTriggerEnter(Collider other) { }

    public override void OnTriggerExit(Collider other) { }

    public override void OnTriggerStay(Collider other) { }

    public override void UpdateState() { }
}
