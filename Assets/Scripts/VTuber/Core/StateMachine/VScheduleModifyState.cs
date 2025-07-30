using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.StateMachine
{
    public class VScheduleModifyState : VState
    {
        public VScheduleModifyState()
        {
            stateType = VStateType.ScheduleModify;
        }
        
        public override void Enter(VState state, params object[] enterParams)
        {
            base.Enter(state, enterParams);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(true);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToCreation();
            stateMachine.ScheduleUI.SwitchToModify();
        }

        public override void Exit(VState nextState)
        {
            base.Exit(nextState);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(false);
        }
    }
}