using System.Collections.Generic;
using PrimeTween;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class EventExcutioner
    {
        
    }
    
    public class VExecutionState : VState
    {
        private VScheduleEvent _currentEvent;
        private bool shouldSwitchToModifySchedule = false;
        private VScheduleEvent _skipToEvent;
        public VExecutionState()
        {
            stateType = VStateType.Execution;
        }
        
        private void OnSwitchToModifySchedule(Dictionary<string, object> messagedict)
        {
            shouldSwitchToModifySchedule = true;
        }

        private void OnSkipEvent(Dictionary<string, object> messagedict)
        {
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd, 
                new Dictionary<string, object>()
                {
                    {"Event", _currentEvent}
                });
            stateMachine.BattleRoot.SetActive(false);
            (_currentEvent as VStreamEvent).SetResultEvent((bool)messagedict["IsTargetMet"]);

            if (_currentEvent.FollowUpEvent is not null)
            {
                _currentEvent.FollowUpEvent.Execute(stateMachine.Character);
            }
        }
        
        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            stateMachine.EventSystemRoot.SetActive(false);
            
            if (_currentEvent.FollowUpEvent is not null)
            {
                _currentEvent.FollowUpEvent.Execute(stateMachine.Character);
                return;
            }
            
            if (shouldSwitchToModifySchedule)
            {
                shouldSwitchToModifySchedule = false;
                stateMachine.SwitchState(VStateType.ScheduleModify);
                return;
            }
            
            if (stateMachine.ShouldPauseSchedule)
            {
                _currentEvent.AdvanceTime();
                stateMachine.SwitchState(VStateType.Pause);
            }
            else
            {
                _currentEvent.AdvanceTime();
                NextEvent();
            }
            stateMachine.Script.OnEventExecuted(_currentEvent);
        }

        private void NextEvent()
        {
            var e = stateMachine.WeeklySchedule.NextEvent();
            if (e is null)
            {
                stateMachine.NextSchedule();
                return;
            }
            ExecuteEvent(e);
        }

        public void SkipEvent()
        {
            ExecuteEvent(_currentEvent);
        }

        public void ExecuteEvent(VScheduleEvent e)
        {
            if (e is null)
            {
                return;
            }

            if (stateMachine.Character.TestCost(e))
            {
                stateMachine.ScheduleUI.MoveIndicator(e.Coordinate).OnComplete(() =>
                {
                    stateMachine.Character.ApplyCost(e);
                    e.Execute(stateMachine.Character);
                    
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventBeginExecute, new Dictionary<string, object>
                    {
                        { "Event", e },
                        { "Coordinate", e.Coordinate }
                    });
                });
            }
            else
            {
                Tween.Delay(0.1f, () =>
                {
                    VDebug.Log(_currentEvent.Coordinate);
                    var staminaNotEnoughEvent = VResourcesManager.Instance.CreateDialogueEventByID(8);
                    staminaNotEnoughEvent.SetDaySchedule(_currentEvent.DaySchedule, _currentEvent.Coordinate);
                    staminaNotEnoughEvent.Execute(stateMachine.Character);
                });
            }
        }
        
        public void InitializeBattle(int initialTurnCount, int targetPopularity, int initialViewers)
        {
            stateMachine.BattleRoot.SetActive(true);
            stateMachine.Battle.InitializeBattle(stateMachine.Character.AttributeManager,
                stateMachine.Character.CardLibrary,
                initialTurnCount, targetPopularity, initialViewers,
                stateMachine.Character.CharacterRelicManager.GetBattleRelics());
        }
        
        public void InitializeEvent(VDialogueEvent e)
        {
            stateMachine.EventSystemRoot.SetActive(true);
            stateMachine.EventSystemSystem.InitializeEvent(stateMachine.Character, e);
        }
        
        private void OnEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VScheduleEvent;
            InitializeEvent(_currentEvent as VDialogueEvent);
        }
        
        private void OnStreamEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VScheduleEvent;
            var streamEvent = _currentEvent as VStreamEvent;
            InitializeBattle(streamEvent.InitialTurnCount, streamEvent.TargetPopularity, streamEvent.InitialViewers);
        }

        private void AddEventToCurrentEvent(VScheduleEventType eventType, uint id)
        {
            _currentEvent.AddFollowUpEvent(eventType, id);
        }
        
        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSkipEvent, OnSkipEvent);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
            
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(true);
            stateMachine.ScheduleUI.SwitchToExecution();
            if (state.StateType == VStateType.ScheduleCreation)
            {            
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToExecution().OnComplete(() =>
                {
                    stateMachine.ScheduleUI.ResetIndicatorPosition().OnComplete(() =>
                    {
                        var e = stateMachine.WeeklySchedule.BeginExecution();
                        ExecuteEvent(e);
                    });
                });
            }
            else if (state.StateType == VStateType.Pause)
            {
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToExecution().
                    OnComplete(() => NextEvent());
            }
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSkipEvent, OnSkipEvent);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
            
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(false);
        }
    }
}