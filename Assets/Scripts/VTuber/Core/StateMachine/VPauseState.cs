using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VPauseState : VState
    {
        public VPauseState()
        {
            stateType = VStateType.Pause;
        }

        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            stateMachine.PauseSchedule();
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetPauseUIActive(true);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToPause();
            stateMachine.ScheduleUI.SwitchToExecution();
            
            stateMachine.Character.ConsumableManager.SetCanUseConsumable(true);
        }
        
        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            stateMachine.SetShouldPauseSchedule(false);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetPauseUIActive(false);
            stateMachine.Character.ConsumableManager.SetCanUseConsumable(false);
        }
    }
}