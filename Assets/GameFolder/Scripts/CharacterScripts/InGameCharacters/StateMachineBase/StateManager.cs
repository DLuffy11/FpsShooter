using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState , BaseState<EState>> _states = new Dictionary<EState, BaseState<EState>>();

    protected BaseState<EState> _currentState;

    protected bool _isTransitioningState = false;

    private void Start()
    {
        _currentState.EnterState();
    }

    protected void Update()
    {
        EState nextStateKey = _currentState.GetNextState();

        if (!_isTransitioningState && nextStateKey.Equals(_currentState.StateKey))
            _currentState.UpdateState();
        else if (!_isTransitioningState)
            TransitionToState(nextStateKey);
    }

    private void TransitionToState(EState nextStateKey)
    {
        _isTransitioningState = true;
        _currentState.ExitState();
        _currentState = _states[nextStateKey];
        _currentState.EnterState();
        _isTransitioningState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        _currentState.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    { 
        _currentState.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _currentState.OnTriggerExit(other);
    }
}
