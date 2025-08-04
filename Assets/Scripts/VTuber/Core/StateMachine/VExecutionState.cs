using System.Collections.Generic;
using PrimeTween;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VExecutionState : VState
    {
        private VScheduleEvent _currentEvent;
        private bool shouldSwitchToModifySchedule = false;
        private List<VScheduleEvent> history;
        private VScheduleEvent skipToEvent;
        public VExecutionState()
        {
            stateType = VStateType.Execution;
            history = new List<VScheduleEvent>();
        }
        
        public override void Register(VStateMachine vStateMachine)
        {
            base.Register(vStateMachine);
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSkipEvent, OnSkipEvent);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }



        public override void Unregister()
        {
            base.Unregister();
            
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSkipEvent, OnSkipEvent);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
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
            history.Add(_currentEvent);
            VDebug.Log((bool)messagedict["IsTargetMet"]);
            var resultEvent = (_currentEvent as VStreamEvent).GetResultEvent((bool)messagedict["IsTargetMet"]);
            resultEvent.Execute(stateMachine.Character);
        }
        
        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            history.Add(_currentEvent);
            stateMachine.EventSystemRoot.SetActive(false);
            if (shouldSwitchToModifySchedule)
            {
                shouldSwitchToModifySchedule = false;
                stateMachine.SwitchState(VStateType.ScheduleModify);
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

        private void NextEvent()
        {
            var e = _currentEvent.GetNextEvent();
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
                    
                    VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventExecuted, new Dictionary<string, object>
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
                initialTurnCount, targetPopularity, initialViewers);
        }
        
        public void InitializeEvent(string node)
        {
            stateMachine.EventSystemRoot.SetActive(true);
            stateMachine.EventSystemSystem.InitializeEvent(stateMachine.Character, node);
        }
        
        private void OnEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VScheduleEvent;
            InitializeEvent((string)messagedict["DialogueNode"]);
        }
        
        private void OnStreamEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VScheduleEvent;
            var streamEvent = _currentEvent as VStreamEvent;
            InitializeBattle(streamEvent.InitialTurnCount, streamEvent.TargetPopularity, streamEvent.InitialViewers);
        }
        
        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            
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
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(false);
        }
    }
}