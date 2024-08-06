using UnityEngine;

public class IdleState : BaseState<PlayerMovementStates.PlayerState>
{
    private PlayerMovementStates _playerMovement;

    public IdleState(PlayerMovementStates.PlayerState key, PlayerMovementStates playerMovementStates)
        : base(PlayerMovementStates.PlayerState.Idle)
    {
        _playerMovement = playerMovementStates;
    }

    public override void EnterState() { }

    public override void ExitState() { }

    public override PlayerMovementStates.PlayerState GetNextState()
    {
        if (_playerMovement.IsAimPressed && !_playerMovement.Animator.GetBool(HashStrings.Aim))
        {
            _playerMovement.Animator.SetBool(HashStrings.Aim, true);
        }
        else if (!_playerMovement.IsAimPressed && _playerMovement.Animator.GetBool(HashStrings.Aim))
        {
            _playerMovement.Animator.SetBool(HashStrings.Aim, false);
        }

        if (_playerMovement.IsWalkPressed)
            return PlayerMovementStates.PlayerState.Walk;

        if (_playerMovement.IsJumpPressed)
            return PlayerMovementStates.PlayerState.Jump;

        return PlayerMovementStates.PlayerState.Idle;
    }

    public override void OnTriggerEnter(Collider other) { }

    public override void OnTriggerExit(Collider other) { }

    public override void OnTriggerStay(Collider other) { }

    public override void UpdateState() { }
}
