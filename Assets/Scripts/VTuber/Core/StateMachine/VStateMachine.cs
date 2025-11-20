using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using Tutorial.Script;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.EventSystem;
using VTuber.RaisingAnimationSystem;
using VTuber.Reincarnation;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.StateMachine
{
    public class VStateMachineSaveData
    {
        public VStateType currentStateType;
        public VStateType lastStateType;
        public List<VStateSaveData> stateSaveDataList;
        public int weekIndex;
    }

    public class VStateMachine
    {
        public readonly bool isTutorial;
        protected bool shouldPauseSchedule;

        public bool IsInitialized { get; }

        private List<VState> RegisteredStateList { get; } = new();

        public VState CurrentState { get; private set; }

        public VState LastState { get; private set; }

        public VScheduleUI ScheduleUI { get; }

        public VWeeklySchedule WeeklySchedule { get; }

        public GameObject BattleRoot { get; }

        public GameObject EventSystemRoot { get; }

        public VEventSystem EventSystemSystem { get; }

        public VCharacter Character { get; }

        public bool ShouldPauseSchedule => shouldPauseSchedule;

        public VScript Script { get; }

        public VTutorialScript TutorialScript { get; }

        public VReincarnationConfiguration ReincarnationConfiguration { get; }

        public VStateMachine(bool isTutorial, VScheduleUI scheduleUI,
            VWeeklySchedule weeklySchedule,
            GameObject battleRoot,
            GameObject eventSystemRoot, VEventSystem eventSystemSystem,
            VCharacter character, VScript script, VReincarnationConfiguration reincarnationConfiguration)
        {
            this.isTutorial = isTutorial;

            if (isTutorial) TutorialScript = script as VTutorialScript;

            ScheduleUI = scheduleUI;
            WeeklySchedule = weeklySchedule;
            BattleRoot = battleRoot;
            EventSystemRoot = eventSystemRoot;
            EventSystemSystem = eventSystemSystem;
            Character = character;
            Script = script;
            IsInitialized = true;
            ReincarnationConfiguration = reincarnationConfiguration;
        }
        
        public VStateMachineSaveData Save()
        {
            return new VStateMachineSaveData
            {
                currentStateType = CurrentState.StateType,
                lastStateType = LastState?.StateType ?? VStateType.None,
                stateSaveDataList = RegisteredStateList.Select(state => state.Save()).ToList()
            };
        }

        public void Load(VStateMachineSaveData saveData)
        {
            LastState = RegisteredStateList.Find(state => state.StateType == saveData.lastStateType);

            foreach (var state in RegisteredStateList)
                state.Load(saveData.stateSaveDataList.Find(saveData => saveData.stateType == state.StateType));
            SwitchState(saveData.currentStateType);
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

            foreach (var state in RegisteredStateList) state.Unregister();
            RegisteredStateList.Clear();
            CurrentState = null;
            LastState = null;
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

            if (CurrentState is not null)
                CurrentState.Exit(state);
            LastState = CurrentState;
            CurrentState = state;
            CurrentState.Enter(LastState, args);

            return false;
        }

        public void Update()
        {
            CurrentState.Update();
        }

        public void NextSchedule()
        {
            if (isTutorial && !TutorialScript.CheckCurrentWeekConditions(Character))
            {
                VRaisingUI.Instance.ShowRestartWeekUI();
                return;
            }

            VDebug.Log("<color=green>Next Schedule</color>");
            WeeklySchedule.Reset(true);
            ScheduleUI.ResetSchedule();
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnScheduleEnd, new Dictionary<string, object>
            {
                { "WeekIndex", Script.WeekIndex }
            });
            
            Script.NextWeek();
            VRaisingUI.Instance.UpdateWeekCount(Script.WeekIndex + 1);
            var e = Script.NextWeek();
            if (e is not null)
            {
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnPhaseEnd, new Dictionary<string, object>
                {
                    { "Phase", Script.CurrentPhase }
                });
                VRaisingAnimationSystem.Instance.ExecuteAnimations(() =>
                {
                    Tween.Delay(0.1f, () => { SwitchState(VStateType.PhaseStart, e); });
                });
            }
            else
                SwitchState(VStateType.ScheduleCreation);
        }
    }
}