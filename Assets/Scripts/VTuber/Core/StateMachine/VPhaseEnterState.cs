using System.Collections.Generic;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VPhaseStartState : VState
    {
        VDialogueEvent _currentEvent;
        public VPhaseStartState()
        {
            stateType = VStateType.PhaseStart;
        }

        public void InitializeEvent(VDialogueEvent e)
        {
            stateMachine.EventSystemRoot.SetActive(true);
            stateMachine.EventSystemSystem.InitializeEvent(stateMachine.Character, e, true);
        }

        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            
            _currentEvent = (VDialogueEvent)stateMachine.Script.CurrentPhase.GetStartEvent();
            
            InitializeEvent(_currentEvent);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            stateMachine.Character.ConsumableManager.SetCanUseConsumable(false);
        }
        
        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            stateMachine.EventSystemRoot.SetActive(false);
            stateMachine.SwitchState(VStateType.ScheduleCreation);
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
            stateMachine.Character.ConsumableManager.SetCanUseConsumable(true);
        }
    }
}