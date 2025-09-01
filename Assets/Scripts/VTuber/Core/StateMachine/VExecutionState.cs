using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Reincarnation;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VExecutionState : VState
    {
        private VScheduleEvent _currentEvent;
        private bool _shouldSwitchToModifySchedule = false;
        private VScheduleEvent _skipToEvent;
        private Queue<VScheduleEvent> _dayEndEvents;
        private bool _shouldEndGame = false;
        private int _lastStreamPopularity = 0;
        private bool _isLastStreamSuccess = false;
        
        public VExecutionState()
        {
            stateType = VStateType.Execution;
            _dayEndEvents = new Queue<VScheduleEvent>();
        }
        
        private void OnSwitchToModifySchedule(Dictionary<string, object> messagedict)
        {
            _shouldSwitchToModifySchedule = true;
        }

        private void OnSkipEvent(Dictionary<string, object> messagedict)
        {
            stateMachine.Character.SkipEventRecoverStamina();
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            stateMachine.BattleRoot.SetActive(false);
            _isLastStreamSuccess = (bool)messagedict["IsTargetMet"];
            (_currentEvent as VStreamEvent).SetResultEvent(_isLastStreamSuccess);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd, 
                new Dictionary<string, object>()
                {
                    {"Event", _currentEvent}
                });
            stateMachine.Character.ConsumableManager.SetBattle(null);
            VRaisingUI.Instance.SetConsumableToRaising();
            _lastStreamPopularity = messagedict["Popularity"] as int? ?? 0;
            if(_isLastStreamSuccess)
                stateMachine.Character.succeededStreams.Add(_currentEvent);
            
            VRaisingUI.Instance.SwitchAttributesUIBattle(true);
        }
        
        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            stateMachine.EventSystemRoot.SetActive(false); 
            _currentEvent.ExecuteCoopEvents(stateMachine.Character);
            
            if (_shouldEndGame)
            { 
                _shouldEndGame = false; 
                EndRun();
                return;
            }
            
            if (_currentEvent.FollowUpEvent is not null)
            {
                Tween.Delay(0.2f, () =>
                {
                    _currentEvent.FollowUpEvent.Execute(stateMachine.Character);
                });
                return;
            }
            
            if (_shouldSwitchToModifySchedule)
            {
                _shouldSwitchToModifySchedule = false;
                stateMachine.SwitchState(VStateType.ScheduleModify);
                return;
            }

            var temp = _currentEvent;
            _currentEvent = null;
            temp.AdvanceTime();
            if (_dayEndEvents.Count != 0)
            {
                var e = _dayEndEvents.Dequeue();
                ExecuteEvent(e);
                return;
            }

            if (stateMachine.ShouldPauseSchedule)
            {
                stateMachine.SwitchState(VStateType.Pause);
            }
            else
            {
                NextEvent();
            }
        }

        public void EndRun()
        {
            var result = stateMachine.Script.CalculateScore(stateMachine.Character, _lastStreamPopularity, _isLastStreamSuccess); 
            var account =VAccountCreator.CreateAccount(stateMachine.ReincarnationConfiguration,
                result.scoreLevelName, stateMachine.Character); 
            
            VRaisingUI.Instance.InitializeEndingUI(stateMachine.Character.Name, result.scoreLevelName, result.score, account);
            VRaisingUI.Instance.ShowEndingUI();
            stateMachine.Character.EndRun();
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEndRun, new Dictionary<string, object>());
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
                    var staminaNotEnoughEvent = VDataManager.Instance.CreateDialogueEventByID(8);
                    staminaNotEnoughEvent.SetDaySchedule(e.DaySchedule, -1 * Vector2Int.one);
                    staminaNotEnoughEvent.Execute(stateMachine.Character);
                    
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventBeginExecute, new Dictionary<string, object>
                    {
                        { "Event", staminaNotEnoughEvent },
                        { "Coordinate", e.Coordinate }
                    });
                    e.SetExecuted();
                });
                
            }
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

        private void AddEventToCurrentEvent(VEventType eventType, uint id)
        {
            if(_currentEvent is null)
                _dayEndEvents.Enqueue(VDataManager.Instance.CreateEvent(eventType, id));
            else
                _currentEvent.AddFollowUpEvent(eventType, id);
        }
        
        
        private void OnAddFollowUpEvent(Dictionary<string, object> messagedict)
        {
            AddEventToCurrentEvent((VEventType)messagedict["EventType"], (uint)messagedict["EventId"]);
        }
        
        private void OnBeginEnding(Dictionary<string, object> messagedict)
        {
            _shouldEndGame = true;
        }
        
        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSkipEvent, OnSkipEvent);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnAddFollowUpEvent, OnAddFollowUpEvent);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginEnding, OnBeginEnding);
            
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
            
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(true);
            stateMachine.ScheduleUI.SwitchToExecution();
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetPauseText(false);
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
            stateMachine.Character.ConsumableManager.SetCanUseConsumable(false);
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSkipEvent, OnSkipEvent);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginEnding, OnBeginEnding);
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
            
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(false);
            stateMachine.Character.ConsumableManager.SetCanUseConsumable(true);
        }
    }
}