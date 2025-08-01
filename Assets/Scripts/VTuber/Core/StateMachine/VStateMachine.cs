using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;

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

        public VScheduleUI ScheduleUI => _scheduleUI;
        private VScheduleUI _scheduleUI;
        
        public VWeeklySchedule WeeklySchedule => _weeklySchedule;
        private VWeeklySchedule _weeklySchedule; 
        
        public GameObject BattleRoot => _battleRoot;
        private GameObject _battleRoot;
        
        public VBattle Battle => _battle;
        private VBattle _battle;
        
        public GameObject EventSystemRoot => _eventSystemRoot;
        private GameObject _eventSystemRoot;
        
        public VEvent EventSystem => _eventSystem;
        private VEvent _eventSystem;
        
        public VCharacter Character => _character;
        private VCharacter _character;
        
        public bool ShouldPauseSchedule => shouldPauseSchedule;
        protected bool shouldPauseSchedule = false;
        
        public int WeekCount => _weekCount;
        private int _weekCount = 0;
        

        public VStateMachine(VScheduleUI scheduleUI,
            VWeeklySchedule weeklySchedule,
            GameObject battleRoot, VBattle battle,
            GameObject eventSystemRoot, VEvent eventSystem,
            VCharacter character)
        {
            _scheduleUI = scheduleUI;
            _weeklySchedule = weeklySchedule;
            _battleRoot = battleRoot;
            _battle = battle;
            _eventSystemRoot = eventSystemRoot;
            _eventSystem = eventSystem;
            _character = character;
            IsInitialized = true;
            preRegisterStates.ForEach(state => RegisterState(state));
        }

        public void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnScheduleExecuted, OnScheduleExecuted);
        }

        public void OnDisable()
        {
            UnregisterAll();
        }
        
        private void OnScheduleExecuted(Dictionary<string, object> messagedict)
        {
            NextSchedule();
        }
        
        public void PauseSchedule()
        {
            if (shouldPauseSchedule)
            {
                shouldPauseSchedule = false;
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetPauseText(false);
            }
            else
            {
                shouldPauseSchedule = true;
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetPauseText(true);
            }
        }
        
        public void ContinueSchedule()
        {
            SwitchState(VStateType.Execution);
        }
        
        public bool RegisterState(VState state)
        {
            if (state == null)
                return false;
            if (!IsInitialized)
                return false;
            if (RegisteredStateList.Exists(s => s.StateType == state.StateType))
                return false;
            
            RegisteredStateList.Add(state);
            state.Register(this);
            return false;
        }
        
        public void UnregisterAll()
        {
            if (!IsInitialized)
                return;
            
            foreach (var state in RegisteredStateList)
            {
                state.Unregister();
            }
            RegisteredStateList.Clear();
            currentState = null;
            lastState = null;
        }

        public bool UnRegisterState(VStateType vStateType)
        {
            if (!IsInitialized)
                return false;
            var state = RegisteredStateList.Find(s => s.StateType == vStateType);
            if (state is null)
                return false;

            RegisteredStateList.Remove(state);
            state.Unregister();
            return false;
        }

        public bool SwitchState(VStateType vStateType, params object[] args)
        {
            var state = RegisteredStateList.Find(s => s.StateType == vStateType);
            if (state is null)
                return false;
            
            if (currentState is not null)
                currentState.Exit(state);
            lastState = currentState;
            currentState = state;
            currentState.Enter(lastState, args);
            
            return false;
        }

        public void Update()
        {
            currentState.Update();
        }

        public void NextSchedule()
        {
            _weeklySchedule.Reset();
            ScheduleUI.ResetSchedule();
            _weekCount++;
            SwitchState(VStateType.ScheduleCreation);
            VRaisingUI.Instance.UpdateWeekCount(_weekCount + 1);
        }
    }
}
