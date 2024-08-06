using UnityEngine;

public class WalkState : BaseState<PlayerMovementStates.PlayerState>
{
    #region Private Fields
    private PlayerMovementStates _playerMovement;
    #endregion

    #region Constructor
    public WalkState(PlayerMovementStates.PlayerState key, PlayerMovementStates playerMovementStates)
        : base(key)
    {
        _playerMovement = playerMovementStates;
    }
    #endregion

    #region State Methods
    public override void EnterState()
    {
        _playerMovement.Animator.SetBool(HashStrings.Walk, true);
    }

    public override void ExitState()
    {
        _playerMovement.Animator.SetBool(HashStrings.Walk, false);
    }

    public override PlayerMovementStates.PlayerState GetNextState()
    {
        if (!_playerMovement.IsWalkPressed)
            return PlayerMovementStates.PlayerState.Idle;

        if (_playerMovement.IsJumpPressed)
            return PlayerMovementStates.PlayerState.Jump;

        if (_playerMovement.IsAimPressed)
        {
            if (_playerMovement.IsRunPressed)
            {
                _playerMovement.Animator.SetBool(HashStrings.Aim, false);
                return PlayerMovementStates.PlayerState.Run;
            }

            _playerMovement.Animator.SetBool(HashStrings.Aim, true);
            return PlayerMovementStates.PlayerState.Walk;
        }

        if (!_playerMovement.IsAimPressed && _playerMovement.IsWalkPressed)
        {
            _playerMovement.Animator.SetBool(HashStrings.Aim, false);
        }

        if (_playerMovement.IsRunPressed)
            return PlayerMovementStates.PlayerState.Run;

        return PlayerMovementStates.PlayerState.Walk;
    }

    public override void UpdateState() { }
    #endregion

    #region Trigger Methods
    public override void OnTriggerEnter(Collider other) { }

    public override void OnTriggerExit(Collider other) { }

    public override void OnTriggerStay(Collider other) { }
    #endregion
}
