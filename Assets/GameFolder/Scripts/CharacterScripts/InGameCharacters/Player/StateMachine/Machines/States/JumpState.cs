using UnityEngine;

public class JumpState : BaseState<PlayerMovementStates.PlayerState>
{
    #region Private Fields
    private PlayerMovementStates _playerMovement;
    #endregion

    #region Constructor
    public JumpState(PlayerMovementStates.PlayerState key, PlayerMovementStates playerMovementStates)
        : base(key)
    {
        _playerMovement = playerMovementStates;
    }
    #endregion

    #region State Methods
    public override void EnterState()
    {
        _playerMovement.Animator.SetTrigger(HashStrings.Jump);
        _playerMovement.ChangeJumpingState();
    }

    public override void ExitState()
    {
        _playerMovement.ChangeFallingState();
    }

    public override PlayerMovementStates.PlayerState GetNextState()
    {
        if (_playerMovement.IsFalling)
            return PlayerMovementStates.PlayerState.Land;

        return PlayerMovementStates.PlayerState.Jump;
    }

    public override void UpdateState() { }
    #endregion

    #region Trigger Methods
    public override void OnTriggerEnter(Collider other) { }

    public override void OnTriggerExit(Collider other) { }

    public override void OnTriggerStay(Collider other) { }
    #endregion
}
