using System.Collections.Generic;
using VTuber.Core.EventCenter;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VPhaseStartState : VState
    {
        public void InitializeEvent(VDialogueEvent e)
        {
            stateMachine.EventSystemRoot.SetActive(true);
            stateMachine.EventSystemSystem.InitializeEvent(stateMachine.Character, e);
        }

        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            
            VDialogueEvent e = enterParams[0] as VDialogueEvent;
            
            InitializeEvent(e);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
        }
        
        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            stateMachine.SwitchState(VStateType.ScheduleCreation);
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
        }
    }
}