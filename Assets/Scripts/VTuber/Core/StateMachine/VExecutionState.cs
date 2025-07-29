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
        private VStreamEvent _currentEvent;

        public VExecutionState()
        {
            stateType = VStateType.Execution;
        }
        
        public override void Register(VStateMachine vStateMachine)
        {
            base.Register(vStateMachine);
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }
        
        public override void Unregister()
        {
            base.Unregister();
            
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(0.75f).OnComplete(() => stateMachine.BattleRoot.SetActive(false));
            Tween.Delay(2.0f, () =>
            {
                _currentEvent.NextEvent();
            });
        }
        
        public void InitializeBattle()
        {
            stateMachine.BattleRoot.SetActive(true);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(1.0f);
            stateMachine.Battle.InitializeBattle(stateMachine.Character.AttributeManager,
                stateMachine.Character.CardLibrary,
                _currentEvent.InitialTurnCount);
        }
        
        private void OnStreamEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VStreamEvent;
            InitializeBattle();
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(1.0f);
        }      
        
        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            
            if (state.StateType == VStateType.ScheduleCreation)
            {            
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(true);
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToExecution().OnComplete(() =>
                {
                    stateMachine.ScheduleUI.ResetIndicatorPosition().OnComplete(() => stateMachine.WeeklySchedule.BeginExecution());
                });
            }
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            if (nextState.StateType == VStateType.ScheduleCreation)
            {
                VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(false);
            }
        }
    }
}