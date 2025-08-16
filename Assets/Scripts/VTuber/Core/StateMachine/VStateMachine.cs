using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.EventSystem;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VStateMachine
    {
        public bool IsInitialized { get; private set; }
        
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
        
        public VEventSystem EventSystemSystem => _eventSystemSystem;
        private VEventSystem _eventSystemSystem;
        
        public VCharacter Character => _character;
        private VCharacter _character;
        
        public bool ShouldPauseSchedule => shouldPauseSchedule;
        protected bool shouldPauseSchedule = false;
        
        public int WeekIndex => _weekIndex;
        private int _weekIndex = 0;

        public VScript Script => _script;
        private VScript _script;
        

        public VStateMachine(VScheduleUI scheduleUI,
            VWeeklySchedule weeklySchedule,
            GameObject battleRoot, VBattle battle,
            GameObject eventSystemRoot, VEventSystem eventSystemSystem,
            VCharacter character, VScript script)
        {
            _scheduleUI = scheduleUI;
            _weeklySchedule = weeklySchedule;
            _battleRoot = battleRoot;
            _battle = battle;
            _eventSystemRoot = eventSystemRoot;
            _eventSystemSystem = eventSystemSystem;
            _character = character;
            _script = script;
            IsInitialized = true;
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
            UnregisterAll();
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

        public void SetShouldPauseSchedule(bool value)
        {
            shouldPauseSchedule = value;
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
            VDebug.Log("<color=green>Next Schedule</color>");
            _weeklySchedule.Reset(true);
            ScheduleUI.ResetSchedule();
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnScheduleEnd, new Dictionary<string, object>()
            {
                { "WeekIndex", _weekIndex },
            });
            _weekIndex++;
            VRaisingUI.Instance.UpdateWeekCount(_weekIndex + 1);
            var e = _script.NextWeek(_weekIndex);
            if (e is not null)
            {
                Tween.Delay(0.1f, () =>
                {
                    SwitchState(VStateType.PhaseStart, e);
                });
            }
            else
            {
                SwitchState(VStateType.ScheduleCreation);
            }
        }
    }
}
