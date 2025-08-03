using System.Collections.Generic;
using PrimeTween;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VExecutionState : VState
    {
        private VScheduleEvent _currentEvent;

        public VExecutionState()
        {
            stateType = VStateType.Execution;
        }
        
        public override void Register(VStateMachine vStateMachine)
        {
            base.Register(vStateMachine);
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }

        public override void Unregister()
        {
            base.Unregister();
            
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventStart, OnEventStart);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            
            if (stateMachine.ShouldPauseSchedule)
            {
                stateMachine.SwitchState(VStateType.Pause);
            }
            else
            {
                NextEvent(2.0f);
            }
        }
        
        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            stateMachine.EventSystemRoot.SetActive(false);
            if (stateMachine.ShouldPauseSchedule)
            {
                stateMachine.SwitchState(VStateType.Pause);
            }
            else
            {
                NextEvent(2.0f);
            }
        }

        private void NextEvent(float delay)
        {
            Tween.Delay(delay, () =>
            {
                _currentEvent.NextEvent();
            });
        }
        
        public void InitializeBattle(int initialTurnCount)
        {
            stateMachine.BattleRoot.SetActive(true);
            stateMachine.Battle.InitializeBattle(stateMachine.Character.AttributeManager,
                stateMachine.Character.CardLibrary,
                initialTurnCount);
        }
        
        public void InitializeEvent(string node)
        {
            stateMachine.EventSystemRoot.SetActive(true);
            stateMachine.EventSystem.InitializeEvent(stateMachine.Character, node);
        }
        
        private void OnEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VScheduleEvent;
            VDebug.Log((string)messagedict["DialogueNode"]);
            InitializeEvent((string)messagedict["DialogueNode"]);
        }
        
        private void OnStreamEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VScheduleEvent;
            InitializeBattle((_currentEvent as VStreamEvent).InitialTurnCount);
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
                    stateMachine.ScheduleUI.ResetIndicatorPosition().OnComplete(() => stateMachine.WeeklySchedule.BeginExecution());
                });
            }
            else if (state.StateType == VStateType.Pause)
            {
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToExecution().
                    OnComplete(() => NextEvent(0.0f));
            }
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(false);
        }
    }
}