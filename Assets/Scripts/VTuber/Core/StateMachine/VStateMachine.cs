
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VTuber.Core.StateMachine
{
    public class VStateMachine
    {
        public bool IsInitialized { get; private set; }
        public VState DefaultState => defaultState;
        [SerializeField] private VState defaultState;
        [SerializeField] private List<VState> preRegisterStates = new List<VState>();
        
        private List<VState> RegisteredStateList => _registeredStateList;
        private List<VState> _registeredStateList = new List<VState>();

        public VState CurrentState => currentState;
        private VState currentState;
        
        public VState LastState => lastState;
        private VState lastState;

        public VStateMachine()
        {
            IsInitialized = true;
            preRegisterStates.ForEach(state => RegisterState(state));
        }

        public bool RegisterState(VState state)
        {
            if (state == null)
                return false;
            if (!IsInitialized)
                return false;
            if (RegisteredStateList.Exists(s => s.StateName == state.StateName))
                return false;
            
            RegisteredStateList.Add(state);
            state.Register(this);
            return false;
        }

        public bool UnRegisterState(string stateName)
        {
            if (string.IsNullOrEmpty(stateName))
                return false;
            if (!IsInitialized)
                return false;
            var state = RegisteredStateList.Find(s => s.StateName == stateName);
            if (state is null)
                return false;

            RegisteredStateList.Remove(state);
            state.Unregister();
            return false;
        }

        public bool SwitchState(string targetStateName, params object[] args)
        {
            if (string.IsNullOrEmpty(targetStateName))
                return false;
            var state = RegisteredStateList.Find(s => s.StateName == targetStateName);
            if (state is null)
                return false;
            
            if (currentState is not null)
                currentState.Exit(state);
            lastState = currentState;
            currentState = state;
            currentState.Enter(args);
            
            return false;
        }

        public void Update()
        {
            currentState.Update();
        }
        
    }
}
